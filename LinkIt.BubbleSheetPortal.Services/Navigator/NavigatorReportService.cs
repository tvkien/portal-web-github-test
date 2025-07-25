using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Common.PDFHelper;
using LinkIt.BubbleSheetPortal.Common.ZipHelper;
using LinkIt.BubbleSheetPortal.Common.ZipHelper.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Data.Repositories.ManageParent;
using LinkIt.BubbleSheetPortal.Data.Repositories.NavigatorReport;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.DTOs.MessageQueue;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.ManageAccess;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.PublishByRole;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.Old.Configugration;
using LinkIt.BubbleSheetPortal.SimpleQueueService.Services;
using Newtonsoft.Json;
using S3Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class NavigatorReportService :
        INavigatorReportService
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly INavigatorReportRepository _navigatorReportRepository;
        private readonly INavigatorReportDetailRepository _navigatorReportDetailRepository;
        private readonly IClassRepository _classRepository;
        private readonly INavigatorReportLogRepository _navigatorReportLogRepository;
        private readonly INavigatorReportPublishRepository _navigatorReportPublishRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly INavigatorAttributeRepository _navigatorAttributeRepository;
        private readonly INavigatorConfigurationRepository _navigatorConfigurationRepository;
        private readonly IRepository<Preferences> _preferencesRepository;
        private readonly EmailService _emailService;
        private readonly DistrictDecodeService _districtDecodeService;
        private readonly IReadOnlyRepository<District> _districtRepository;
        private readonly IManageParentRepository _manageParentRepository;

        private readonly IS3Service _s3Service;
        private readonly IMessageQueueService _messageQueueService;

        public NavigatorReportService(ISchoolRepository schoolRepository
            , INavigatorReportRepository navigatorReportRepository
            , INavigatorReportDetailRepository navigatorReportDetailRepository
            , IClassRepository classRepository
            , INavigatorReportLogRepository navigatorReportLogRepository
            , INavigatorReportPublishRepository navigatorReportPublishRepository
            , IRepository<Student> studentRepository
            , INavigatorAttributeRepository navigatorAttributeRepository
            , INavigatorConfigurationRepository navigatorConfigurationRepository
            , IRepository<Preferences> preferencesRepository
            , IManageParentRepository manageParentRepository
            , EmailService emailService
            , DistrictDecodeService districtDecodeService
            , IReadOnlyRepository<District> districtRepository
            , IS3Service s3Service
            , IMessageQueueService messageQueueService)
        {

            this._schoolRepository = schoolRepository;
            this._navigatorReportRepository = navigatorReportRepository;
            this._navigatorReportDetailRepository = navigatorReportDetailRepository;
            this._classRepository = classRepository;
            this._navigatorReportLogRepository = navigatorReportLogRepository;
            this._navigatorReportPublishRepository = navigatorReportPublishRepository;
            this._studentRepository = studentRepository;
            this._navigatorAttributeRepository = navigatorAttributeRepository;
            this._navigatorConfigurationRepository = navigatorConfigurationRepository;
            this._preferencesRepository = preferencesRepository;
            this._manageParentRepository = manageParentRepository;
            this._emailService = emailService;
            this._districtDecodeService = districtDecodeService;
            this._districtRepository = districtRepository;
            _s3Service = s3Service;
            _messageQueueService = messageQueueService;
        }
        public string TempFolder
        {
            get
            {
                return HttpContext.Current?.Server?.MapPath("~/Content/Upload/NavigatorReport/Temp") ?? "/";
            }
        }

        public string Folder { get; set; }
        public string Bucket { get; set; }
        public BaseResponseModel<List<int>> ProcessUploadFiles(NavigatorReportUploadFileFormDataDTO formData, List<NavigatorReportFileDTO> files, int userId, DistributeSetting distributeSetting)
        {
            List<NavigatorReportLogDto> _lstReportDetailError = new List<NavigatorReportLogDto>();
            var errorMessage = string.Empty;
            try
            {
                formData.KeywordIds = GetKeywordByKeywordNames(formData.KeywordShortNames);
                var fileExtension = Path.GetExtension(files.FirstOrDefault().FileName);
                string fileNameByConvention = GetFileNameByConvention(formData, fileExtension);
                var navigatorReportIds = new List<int>();
                foreach (var item in files)
                {
                    // create master file record first
                    var _masterFileEntity = new NavigatorReportEntity()
                    {
                        SchoolID = formData.School,
                        S3FileFullName = fileNameByConvention,
                        DistrictID = formData.District,
                        Year = formData.SchoolYear,
                        KeywordIDs = formData.KeywordIds,
                        NavigatorConfigurationID = formData.ReportType,
                        ReportingPeriodID = formData.ReportingPeriod,
                        ReportSuffix = formData.ReportSuffix,
                        CreatedTime = DateTime.UtcNow,
                        CreatedBy = userId,
                        Status = TextConstants.REPORT_STATUS_NEW
                    };

                    BaseResponseModel<string> saveFileToS3Result = ProcessUploadFileToS3(item, this.Folder, fileNameByConvention);
                    if (saveFileToS3Result.IsSuccess)
                    {
                        _masterFileEntity = _navigatorReportRepository.CreateOrOverwrite(_masterFileEntity, out errorMessage);

                        // log error, it's will remove after fixed bug
                        SendEmailReportErrorLog(errorMessage, distributeSetting);

                        navigatorReportIds.Add(_masterFileEntity.NavigatorReportID);
                    }
                    else
                    {
                        _lstReportDetailError.Add(NavigatorReportLogDto.FromMessage(_masterFileEntity.NavigatorReportID, null, $"The file can't be uploaded {saveFileToS3Result.Message}"));
                        _masterFileEntity.Status = TextConstants.REPORT_STATUS_ERROR;
                        _navigatorReportRepository.Update(_masterFileEntity);
                        return BaseResponseModel<List<int>>.InstanceError(NavigatorReportLogDto.CombineMessage(_lstReportDetailError));
                    }
                }
                if (_lstReportDetailError?.Count > 0)
                {
                    return BaseResponseModel<List<int>>.InstanceSuccess(new List<int>(), NavigatorReportLogDto.CombineMessage(_lstReportDetailError));
                }
                else
                    return BaseResponseModel<List<int>>.InstanceSuccess(navigatorReportIds);
            }
            catch (Exception ex)
            {
                return BaseResponseModel<List<int>>.InstanceError(ex.Message);
            }
            finally
            {
                // log
                if (_lstReportDetailError?.Count > 0)
                    _navigatorReportLogRepository.AddRange(_lstReportDetailError);
            }
        }

        private string GetKeywordByKeywordNames(string keywordNames)
        {
            if (string.IsNullOrEmpty(keywordNames))
            {
                return string.Empty;
            }
            string[] keywordNamesSplitted = keywordNames
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
            var query = _navigatorAttributeRepository.Select()
                 .Where(c => c.AttributeType == Constanst.NAVIGATOR_ATTRIBUTE_KEYWORD)
                 .Where(c => keywordNamesSplitted.Contains(c.Name))
                 .Select(c => new
                 {
                     c.NavigatorAttributeID,
                     c.Name
                 });

            var keywordIds = query.ToArray();
            if (keywordIds.Length == 0)
            {
                return string.Empty;
            }
            var keywordIdsOrdered = (from kw in keywordNamesSplitted.Select((name, index) => new { name, index })
                                     join id in keywordIds
                                     on kw.name.ToLower() equals id.Name.ToLower()
                                     select new
                                     {
                                         kw.index,
                                         id.NavigatorAttributeID
                                     }
                ).OrderBy(c => c.index).ToArray();


            return string.Join(",", keywordIdsOrdered.Select(c => c.NavigatorAttributeID));
        }
        private string GetFileNameByConvention(NavigatorReportUploadFileFormDataDTO formData, string fileExtension)
        {
            string schoolName = string.Empty;
            if (formData.School > 0)
                schoolName = _schoolRepository.GetSchoolNameById(formData.School);
            string reportingPeriodSortName = GetAttributeShortNameById(formData.ReportingPeriod);

            string categoryShortName = GetAttributeShortNameById(formData.NavigatorCategory);
            string reportTypeShortName = _navigatorConfigurationRepository.Select().Where(c => c.NavigatorConfigurationID == formData.ReportType).Select(c => c.ShortName).FirstOrDefault();

            var firstKeyWordShortName = string.Empty;
            var firstKeyWordId = formData.KeywordIds.Split(new char[] { ',' }).FirstOrDefault();
            if (int.TryParse(firstKeyWordId, out int firstKeyWordIdAsInt))
            {
                firstKeyWordShortName = GetAttributeShortNameById(firstKeyWordIdAsInt);
            }

            var fileName = $"{formData.District}{IntegerToFileNameMacro(formData.School)}{(formData.School > 0 ? "_" + schoolName : "")}_{formData.SchoolYear}{StringToFileNameMacro(reportingPeriodSortName)}{StringToFileNameMacro(firstKeyWordShortName)}_{categoryShortName}_{reportTypeShortName}{StringToFileNameMacro(formData.ReportSuffix)}";

            fileName = fileName.RationalizeFileName(Constanst.NAVIGATOR_RATIONALIZEFILENAME_REPLACE_BY);
            fileName = $"{fileName}{fileExtension}";
            return fileName;
        }

        private string GetAttributeShortNameById(int attributeId)
        {


            return _navigatorAttributeRepository.Select().Where(c => c.NavigatorAttributeID == attributeId).Select(c => c.ShortName).FirstOrDefault();
        }

        private static string StringToFileNameMacro(string value)
        {
            return string.IsNullOrEmpty(value) ? "" : "_" + value;
        }
        private static string IntegerToFileNameMacro(int value)
        {
            return (value > 0 ? "_" + value : "");
        }

        private BaseResponseModel<string> ProcessUploadFileToS3(NavigatorReportFileDTO item, string folder, string fileName)
        {
            // upload to S3,save to local then split, read all splited files, save to s3
            string path = $"{folder}/{fileName}";
            var res = UploadRubricFile(Bucket, path, item.FileBinary);
            if (!res.IsSuccess)
            {
                return BaseResponseModel<string>.InstanceError(res.ErrorMessage, "error", null);
            }
            else
            {
                return BaseResponseModel<string>.InstanceSuccess(res.ReturnValue, "");
            }
        }

        private S3Result UploadRubricFile(string bucketName, string fileName, byte[] fileBinary)
        {
            // upload to S3
            using (MemoryStream ms = new MemoryStream(fileBinary))
            {
                return _s3Service.UploadRubricFile(bucketName, fileName, ms);
            }
        }

        public BaseResponseModel<IEnumerable<NavigatorRecordExistResultDto>> GetRecordsExist(IEnumerable<NavigatorReportUploadFileFormDataDTO> forms)
        {
            foreach (var form in forms)
            {
                form.KeywordIds = GetKeywordByKeywordNames(form.KeywordShortNames);
            }
            var recordsExist = _navigatorReportRepository.GetRecordsExist(forms);
            return BaseResponseModel<IEnumerable<NavigatorRecordExistResultDto>>.InstanceSuccess(recordsExist);
        }
        public BaseResponseModel<List<NavigatorUserDto>> GetAssociateUser(string navigatorReportIds, int currUserId, bool isPublished, bool isLoadStudent, bool isLoadTeacher, bool isLoadSchool, bool isLoadDistrictAdmin, string programIds, string gradeIds, int districtId, int roleId, bool selectUserOnly = false)
        {
            BaseResponseModel<List<NavigatorUserDto>> res = _navigatorReportRepository.GetAssociateUser(navigatorReportIds, isPublished, isLoadStudent, isLoadTeacher, isLoadSchool, isLoadDistrictAdmin, programIds, gradeIds, districtId, currUserId, roleId, selectUserOnly);
            return BaseResponseModel<List<NavigatorUserDto>>.InstanceSuccess(res.StrongData);
        }
        private string GetYearListAsString(int? districtId)
        {
            var years = this.GetSchoolYears(districtId);
            var yearsAsString = string.Join(",", years.Select(c => c.Name).ToArray());
            return yearsAsString;
        }

        public IQueryable<NavigatorReportUploadFileResponseDto> GetUploadedReportsInfo(int currUserId, int[] reportIds)
        {
            var filteredList = _navigatorReportRepository.GetUploadedReportsInfo(currUserId, reportIds);
            return filteredList.StrongData?.AsQueryable();
        }

        public BaseResponseModel<bool> Publish(NavigatorReportPublishRequestDto model, int userId, int roleId, int districtId, EmailCredentialSetting emailCredentialSetting)
        {
            var nodePaths = model.NodePath.Split(new string[] { "-_-" }, StringSplitOptions.RemoveEmptyEntries);
            var navigatorReportId = GetNavigatorReportIdsByNodePaths(nodePaths, userId, roleId, districtId).ToIntCommaSeparatedString();
            model.NavigatorReportIds = navigatorReportId;
            model.PublisherId = userId;
            model.PublishTime = DateTime.UtcNow;
            var publishResult = _navigatorReportPublishRepository.Publish(model, userId, roleId);
            if (!publishResult.IsSuccess)
            {
                return BaseResponseModel<bool>.InstanceError(publishResult.Message, "Error", default);
            }

            var sendNotifyEmailResult = SendEmailAfterPublishReports(new NavigatorSendEmailAfterPublishDto()
            {
                AlsoSendEmail = model.AlsoSendEmail,
                CustomNote = model.CustomNote,
                DistrictId = districtId,
                EmailCC = new string[0],
                EmailTo = new string[0],
                ExcludeUserIds = string.Empty,
                PublishResult = publishResult.StrongData,
                RoleId = roleId,
                UserId = userId,
                IgnoreEmailToCheck = true,
                GeneralUrl = model.GeneralUrl
            }, emailCredentialSetting);

            return BaseResponseModel<bool>.InstanceSuccess(true);
        }
        public BaseResponseModel<bool> UnPublish(NavigatorReportUnpublishRequestDto navigatorReportUnPublishRequest, int userId, int roleId, int districtId)
        {
            var nodePaths = navigatorReportUnPublishRequest.NodePath.Split(new string[] { "-_-" }, StringSplitOptions.RemoveEmptyEntries);
            var navigatorReportId = GetNavigatorReportIdsByNodePaths(nodePaths, userId, roleId, districtId).ToIntCommaSeparatedString();
            navigatorReportUnPublishRequest.NavigatorReportIds = navigatorReportId;
            navigatorReportUnPublishRequest.PublisherId = userId;
            navigatorReportUnPublishRequest.PublishTime = DateTime.UtcNow;
            return _navigatorReportPublishRepository.Unpublish(navigatorReportUnPublishRequest, userId, roleId);
        }

        public BaseResponseModel<NavigatorReportFileInfoResponseDto> GetFile(string nodePath, int currUserId, int districtId, int roleId, int classId = 0)
        {
            var nodePaths = nodePath.Split(new string[] { "-_-" }, StringSplitOptions.RemoveEmptyEntries);
            var navigatorReportIds = GetNavigatorReportIdsByNodePaths(nodePaths, currUserId, roleId, districtId);
            var navigatorReportId = string.Join(";", navigatorReportIds);
            BaseResponseModel<List<NavigatorReportGetFileFromDBDto>> fileInforResponse;
            var className = string.Empty;

            if (classId > 0)
            {
                fileInforResponse = _navigatorReportRepository.GetFilesByClass(navigatorReportIds.FirstOrDefault(), currUserId, classId);
                className = _classRepository.GetClassByID(classId)?.Name;
                className = className.RationalizeFileName(Constanst.NAVIGATOR_RATIONALIZEFILENAME_REPLACE_BY);
            }
            else
                fileInforResponse = _navigatorReportRepository.GetFiles(navigatorReportId, currUserId);

            if (fileInforResponse.IsSuccess && fileInforResponse.StrongData.Count > 0)
            {
                //check if multiplefile then zip
                var fileInfors = fileInforResponse.StrongData;
                if (fileInfors.Count > 1)
                {
                    // get all file
                    var multipleFileDatas = fileInforResponse.StrongData.Select(c => new
                    {
                        fileinfo = c,
                        fileFromS3 = new NavigatorReportFileInfoResponseDto()
                    }).ToList();

                    List<NavigatorReportFileInfoResponseDto> lstFileContent = new List<NavigatorReportFileInfoResponseDto>();
                    foreach (var file in multipleFileDatas)
                    {
                        string masterFileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.fileinfo.MasterFileName);
                        string path = $"{Folder}/{masterFileNameWithoutExtension}{(file.fileinfo.PageNumber == 0 ? "" : "_" + file.fileinfo.PageNumber)}{file.fileinfo.MasterFileName.GetExtension()}";
                        var downloadResult = _s3Service.DownloadFile(Bucket, path);
                        if (DownloadFail(downloadResult))
                        {
                            path = $"{Folder}/{masterFileNameWithoutExtension}_{file.fileinfo.PageNumber}_{file.fileinfo.PageNumber}{file.fileinfo.MasterFileName.GetExtension()}";
                            downloadResult = _s3Service.DownloadFile(Bucket, path);
                            if (DownloadFail(downloadResult))
                            {
                                file.fileFromS3.FileData = null;
                            }
                        }
                        if (downloadResult.IsSuccess)
                        {
                            file.fileFromS3.ContentType = TextConstants.CONTENT_TYPE_PDF;
                            file.fileFromS3.FileData = downloadResult.ReturnStream;
                            file.fileFromS3.FileName = file.fileinfo.S3FileFullName;
                        }
                    }

                    var groupedFiles = multipleFileDatas
                        .Where(x => x.fileFromS3.FileData != null)
                        .GroupBy(c => c.fileinfo.MasterFileName.ToLower())
                         .Select(c => new
                         {
                             masterFileName = c.First().fileinfo.MasterFileName,
                             files = c.OrderBy(cc => cc.fileinfo.PageNumber).Select(cc => cc.fileFromS3.FileData).ToList()
                         }).ToList();

                    var mergedFiles = groupedFiles.Select(c => new
                    {
                        masterFileName = $"{c.masterFileName.Split('.').First()}{(string.IsNullOrEmpty(className) ? string.Empty : "_" + className)}{c.masterFileName.GetExtension()}",
                        mergedfileBinary = c.files.Count > 1 ? PDFHelper.MergePDFFilesData(TempFolder, c.files) : c.files.First()
                    }).ToList();

                    // zip then return
                    if (mergedFiles.Count == 1)
                    {

                        return BaseResponseModel<NavigatorReportFileInfoResponseDto>.InstanceSuccess(new NavigatorReportFileInfoResponseDto()
                        {
                            ContentType = MimeMapping.GetMimeMapping(mergedFiles.First().masterFileName),
                            FileData = mergedFiles.First().mergedfileBinary,
                            FileName = mergedFiles.First().masterFileName
                        });
                    }

                    var modelZipHelper = mergedFiles.Select(c => new ZipModel()
                    {
                        FileData = c.mergedfileBinary,
                        FileRelativeName = c.masterFileName
                    }).ToList();
                    byte[] zipedFile = ZipHelper.Zip(TempFolder, modelZipHelper);
                    return BaseResponseModel<NavigatorReportFileInfoResponseDto>.InstanceSuccess(new NavigatorReportFileInfoResponseDto()
                    {
                        ContentType = TextConstants.CONTENT_TYPE_ZIP,
                        FileData = zipedFile,
                        FileName = $"NavigatorReport{DateTime.UtcNow:yyyyMMddHHmmssffff}.zip"
                    });
                    // group ~> merge ~> return
                }
                else
                {
                    // read from S3 first
                    var fileInfo = fileInfors.First();
                    string masterFileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.MasterFileName);
                    string path = $"{Folder}/{masterFileNameWithoutExtension}{(fileInfo.PageNumber == 0 ? "" : "_" + fileInfo.PageNumber)}{fileInfo.MasterFileName.GetExtension()}";
                    var downloadResult = _s3Service.DownloadFile(Bucket, path);
                    if (DownloadFail(downloadResult))
                    {
                        path = $"{Folder}/{masterFileNameWithoutExtension}_{fileInfo.PageNumber}_{fileInfo.PageNumber}{fileInfo.MasterFileName.GetExtension()}";
                        downloadResult = _s3Service.DownloadFile(Bucket, path);
                        if (DownloadFail(downloadResult))
                        {
                            return BaseResponseModel<NavigatorReportFileInfoResponseDto>.InstanceError(TextConstants.CANNOT_DOWNLOAD_FILE_FROM_S3);
                        }
                    }

                    return BaseResponseModel<NavigatorReportFileInfoResponseDto>.InstanceSuccess(new NavigatorReportFileInfoResponseDto()
                    {
                        ContentType = MimeMapping.GetMimeMapping(fileInfo.MasterFileName),
                        FileData = downloadResult.ReturnStream,
                        FileName = $"{fileInfo.MasterFileName.Split('.').First()}{(string.IsNullOrEmpty(className) ? string.Empty : "_" + className)}{fileInfo.MasterFileName.GetExtension()}"
                    });
                }
            }
            else
            {
                return BaseResponseModel<NavigatorReportFileInfoResponseDto>.InstanceError(TextConstants.REPORT_STATUS_NOTFOUND);
            }
            /*
            get all file name from db
            if master file then get master file.
            if separate page then get pages.
            if >2 pages then zip 
             */
        }

        private static bool DownloadFail(S3DownloadResult downloadResult)
        {
            return !downloadResult.IsSuccess || downloadResult.ReturnStream == null || downloadResult.ReturnStream.Length == 0;
        }

        public NavigatorReportFullDto GetNavigatorReportById(int navigatorReportId)
        {
            return _navigatorReportRepository.Select().FirstOrDefault(x => x.NavigatorReportID == navigatorReportId);
        }
        public List<StudentGrade> GetGradesStudent(string userIds)
        {
            return _navigatorReportRepository.GetGradesStudent(userIds);
        }

        public List<StudentProgram> GetProgramsStudent(string userIds)
        {
            return _navigatorReportRepository.GetProgramsStudent(userIds);
        }

        public IEnumerable<ListItem> GetNavigatorCategory()
        {
            return GetNavigatorAttributeAsListItem(Constanst.NAVIGATOR_ATTRIBUTE_CATEGORY);
        }

        private IEnumerable<ListItem> GetNavigatorAttributeAsListItem(string typeName)
        {
            return _navigatorAttributeRepository.Select().Where(c => c.AttributeType == typeName)
                .OrderBy(c => c.ListOrder)
                .Select(c => new ListItem()
                {
                    Id = c.NavigatorAttributeID,
                    Name = c.Name
                }).ToArray();
        }

        public IEnumerable<ListItem> GetKeywords()
        {
            return GetNavigatorAttributeAsListItem(Constanst.NAVIGATOR_ATTRIBUTE_KEYWORD);
        }

        public IEnumerable<ListItem> GetReportingPeriod()
        {
            return GetNavigatorAttributeAsListItem(Constanst.NAVIGATOR_ATTRIBUTE_REPORTINGPERIOD);
        }

        public IEnumerable<ListItem> GetReportTypes(int? navigatorCategoryID)
        {
            if (!navigatorCategoryID.HasValue)
            {
                return new ListItem[0];
            }
            return _navigatorConfigurationRepository.Select()
                    .Where(c => c.NavigatorCategoryID == navigatorCategoryID.Value)
                    .OrderBy(c => c.ReportName)
                    .Select(c => new ListItem()
                    {
                        Id = c.NavigatorConfigurationID,
                        Name = c.ReportName
                    }).ToArray();
        }

        public NavigatorConfigurationDTO GetConfigurationById(int? navigatorConfigurationId)
        {
            if (!navigatorConfigurationId.HasValue)
            {
                return null;
            }
            return _navigatorConfigurationRepository.Select().Where(c => c.NavigatorConfigurationID == navigatorConfigurationId.Value).FirstOrDefault();
        }

        public IEnumerable<ListItemStr> GetSchoolYears(int? districtId)
        {
            var startMonthString = districtId.HasValue ? _preferencesRepository
                .Select()
                .Where(c => c.Level == "district" && c.Id == districtId && c.Label == "ar_defaultStartMonth")
                .Select(c => c.Value).FirstOrDefault() : "";
            if (string.IsNullOrEmpty(startMonthString) || !int.TryParse(startMonthString, out _))
            {
                startMonthString = _preferencesRepository
                    .Select()
                    .Where(c => c.Level == "enterprise" && c.Label == "ar_defaultStartMonth")
                    .Select(c => c.Value).FirstOrDefault();
            }

            var yearCount = _districtDecodeService.GetDistrictDecodeOrConfigurationByLabel(districtId.GetValueOrDefault(), DistrictDecodeLabel.NavigatorNumberOfSchoolYear, 4);

            ListItemStr[] yearList = GetYearListByStartMonth(startMonthString, DateTime.Now, yearCount);
            return yearList;
        }

        public ListItemStr[] GetYearListByStartMonth(string startMonthString, DateTime now, int yearCount)
        {
            var startMonth = 8;
            int.TryParse(startMonthString, out startMonth);
            now = new DateTime(now.Year, now.Month, 1);
            var maxYear = now.Year;

            var nextStartSchoolYear = new DateTime(now.Year, startMonth, 1);
            if (nextStartSchoolYear <= now)
            {
                maxYear = now.Year + 1;
                nextStartSchoolYear = nextStartSchoolYear.AddYears(1);
            }

            if (MonthDiff(now, nextStartSchoolYear) is int _monthDiff && _monthDiff < 5 && _monthDiff > 0)
            {
                maxYear = nextStartSchoolYear.Year + 1;
                yearCount++;
            }
            var yearList = Enumerable.Range(0, yearCount).Select(index => new
            {
                start = maxYear - 1 - index,
                end = maxYear - index
            }).Select(c => $"{c.start}-{c.end}")
               .Select(c => new ListItemStr()
               {
                   Id = c,
                   Name = c
               }).ToArray();
            return yearList;
        }

        private int MonthDiff(DateTime startDate, DateTime endDate)
        {
            return (endDate.Year * 12 + endDate.Month) - (startDate.Year * 12 + startDate.Month);
        }
        public NavigatorConfigurationDTO GetMaxConfigurationByNavigatorReportIdsAndCurrentRole(IEnumerable<int> navigatorReportIds, int roleId)
        {
            //NavigatorConfigurationDTO
            string navigatorReportIdsAsString = string.Join(",", navigatorReportIds);
            var maxConfig = _navigatorReportRepository.GetMaxConfigurationByNavigatorReportIds(navigatorReportIdsAsString);
            return new NavigatorConfigurationDTO()
            {
                CanPublishDistrictAdmin = (maxConfig?.CanPublishDistrictAdmin ?? false)
                && CanPublishToRole((RoleEnum)roleId, RoleEnum.DistrictAdmin),

                CanPublishSchoolAdmin = (maxConfig?.CanPublishSchoolAdmin ?? false)
                && CanPublishToRole((RoleEnum)roleId, RoleEnum.SchoolAdmin),

                CanPublishStudent = (maxConfig?.CanPublishStudent ?? false)
                && CanPublishToRole((RoleEnum)roleId, RoleEnum.Student),

                CanPublishTeacher = (maxConfig?.CanPublishTeacher ?? false)
                && CanPublishToRole((RoleEnum)roleId, RoleEnum.Teacher)
            };
        }

        public bool CanPublishToRole(RoleEnum publisherRole, RoleEnum destinyRole)
        {
            switch (destinyRole)
            {
                case RoleEnum.DistrictAdmin:
                    return new RoleEnum[] { RoleEnum.Publisher, RoleEnum.NetworkAdmin, RoleEnum.DistrictAdmin }.Contains(publisherRole);
                case RoleEnum.Teacher:
                    return new RoleEnum[] { RoleEnum.Publisher, RoleEnum.NetworkAdmin, RoleEnum.DistrictAdmin, RoleEnum.SchoolAdmin }.Contains(publisherRole);
                case RoleEnum.SchoolAdmin:
                    return new RoleEnum[] { RoleEnum.Publisher, RoleEnum.NetworkAdmin, RoleEnum.DistrictAdmin, RoleEnum.SchoolAdmin }.Contains(publisherRole);
                case RoleEnum.Student:
                    return new RoleEnum[] { RoleEnum.Publisher, RoleEnum.NetworkAdmin, RoleEnum.DistrictAdmin, RoleEnum.SchoolAdmin }.Contains(publisherRole);
                default:
                    return false;
            }
        }

        public NavigatorManagePublishConfigurationDto GetManagePublishingConfiguration(string nodePath, int userId, int roleId, int districtId)
        {
            var nodePaths = nodePath.Split(new string[] { "-_-" }, StringSplitOptions.RemoveEmptyEntries);
            var navigatorReportIds = GetNavigatorReportIdsByNodePaths(nodePaths, userId, roleId, districtId);
            var navigatorReportId = navigatorReportIds.ToIntCommaSeparatedString();
            var configuration = GetMaxConfigurationByNavigatorReportIdsAndCurrentRole(navigatorReportIds, roleId);

            var associateUsers = GetAssociateUser(navigatorReportId, userId, false, true, false, false, false, string.Empty, string.Empty, districtId, roleId, true);
            var associateUserId = string.Join(",", associateUsers.StrongData.Select(o => o.UserID).ToArray());

            var studentGrade = GetGradesStudent(associateUserId);
            var studentProgram = GetProgramsStudent(associateUserId);
            var managePublishConfiguration = new NavigatorManagePublishConfigurationDto
            {
                NavigatorConfiguration = configuration,
                StudentGrade = studentGrade,
                StudentProgram = studentProgram
            };
            return managePublishConfiguration;
        }

        public IEnumerable<ViewableNavigatorReportAttributesDTO> GetNavigatorCheckboxesDataByStateIdAndDistrictId(int userId, int roleId, int? stateId, int? districtId)
        {
            string yearsAsString = GetYearListAsString(districtId);
            var attributesDatas = _navigatorReportRepository
                .GetNavigatorCheckboxesDataByStateIdAndDistrictId(userId, roleId, stateId, districtId, yearsAsString)
                .Select(c => new ViewableNavigatorReportAttributesDTO()
                {
                    Id = c.Id ?? 0,
                    Category = c.Category,
                    Ord = c.Ord,
                    Name = c.Name,
                    Type = c.Type
                }); ;
            return attributesDatas;
        }

        public IEnumerable<NavigatorReportDto> GetNavigatorReports(string nodePath, int userId, int roleId, int? districtId)
        {
            var rootNode = BuildNavigatorReportNodes(userId, roleId, districtId);
            var current = rootNode.FindNodeByPath(nodePath);
            if(current == null)
                return Enumerable.Empty<NavigatorReportDto>();

            var result = current.GetChildren()
                .Select(x => new NavigatorReportDto
                {
                    Name = x.Name,
                    NodePath = x.NodePath,
                    DocumentType = x.GetDocumentType(),
                    LastModifiedDate = x.GetLastModifiedDate(),
                });
            return result;
        }

        private NavigatorReportTreeDto BuildNavigatorReportNodes(int userId, int roleId, int? districtId)
        {
            var filter = new NavigatorReportByLevelFilterDTO()
            {
                UserId = userId,
                DistrictId = districtId,
                RoleId = roleId,
                AcceptedYears = GetYearListAsString(districtId)
            };
            var navigatorReports = _navigatorReportRepository.NavigatorGetDirectoryList(filter).ToList();
            var root = new NavigatorReportFolderNodeDto();
            foreach (var report in navigatorReports)
            {
                var navigatorPathway = JsonConvert.DeserializeObject<NavigatorPathwayDto>(report.Path);
                BuildTreeFromPath(root, navigatorPathway.Path, report);
            }
            return root;
        }

        private void BuildTreeFromPath(NavigatorReportTreeDto root, string path, NavigatorGetDirectoryListResult report)
        {
            var pathParts = path.Split('/');
            var current = root;
            for (int i = 0; i < pathParts.Length; i++)
            {
                var folderNode = GetNavigatorFolderNode(pathParts[i], report, current);

                if (string.IsNullOrEmpty(folderNode.Name))
                    continue;

                var nodeItem = current.Children
                    .Where(x => x.IsFolder() && string.Equals(x.GetUniqueKeyInFolder(), folderNode.Name, StringComparison.OrdinalIgnoreCase))
                    .Cast<NavigatorReportFolderNodeDto>()
                    .FirstOrDefault();

                if (nodeItem == null)
                {
                    nodeItem = folderNode;
                    current.Children.Add(folderNode);
                }
                nodeItem.NavigatorKeywords = nodeItem.NavigatorKeywords & folderNode.NavigatorKeywords;
                current = nodeItem;
            }
            var fileNode = GetNavigatorFileNode(report, current);
            if (!current.Children.Any(x => x.IsLeaf() &&
                string.Equals(x.GetUniqueKeyInFolder(), fileNode.GetUniqueKeyInFolder(), StringComparison.OrdinalIgnoreCase)))
            {
                current.Children.Add(fileNode);
            }
        }

        private NavigatorReportFolderNodeDto GetNavigatorFolderNode(string nodePath, NavigatorGetDirectoryListResult report, NavigatorReportTreeDto parent)
        {
            var listOfVariables = new List<(string keyword, string variable, NavigatorKeywords keywordType)>
                {
                    ("[Year]", report.Year, NavigatorKeywords.Year),
                    ("[Navigator_Category]", report.NavigatorCategory, NavigatorKeywords.Category),
                    ("[Keyword]", report.PrimaryKeyword, NavigatorKeywords.Keyword),
                    ("[Reporting_Period]", report.ReportingPeriod, NavigatorKeywords.Period),
                    ("[Report_Type]", report.ReportingType, NavigatorKeywords.Type),
                    ("[School]", report.School, NavigatorKeywords.School),
                    ("[Report_Suffix]", report.ReportSuffix, NavigatorKeywords.Suffix),
                };
            var listOfNavigatorKeywords = listOfVariables.Where(x => nodePath.Contains(x.keyword)).Select(x => x.keywordType);
            var navigatorKeywords = listOfNavigatorKeywords.Any() ?
                    listOfNavigatorKeywords.Aggregate((item, next) => item | next) : NavigatorKeywords.None;

            var nodeName = nodePath;
            foreach (var variable in listOfVariables)
            {
                nodeName = nodeName.Replace(variable.keyword, variable.variable);
            }
            nodeName = nodeName.Replace("\n", "").Replace("\r", "");

            var nodeItem = new NavigatorReportFolderNodeDto
            {
                NavigatorKeywords = navigatorKeywords,
                Name = nodeName,
                Parent = parent,
            };
            return nodeItem;
        }

        private NavigatorReportTreeDto GetNavigatorFileNode(NavigatorGetDirectoryListResult report, NavigatorReportTreeDto parent)
        {
            var fileNode = new NavigatorReportFileNodeDto()
            {
                Name = string.IsNullOrEmpty(report.ReportSuffix) ? report.ReportingType : $"{report.ReportingType} ({report.ReportSuffix})",
                LastModifiedDate = report.LastModifiedDate,
                NavigatorReportId = report.NavigatorReportId.Value,
                NavigatorConfigurationID = report.NavigatorConfigurationId,
                NavigatorReportCategoryId = report.NavigatorCategoryId,
                SchoolId = report.SchoolId,
                S3FileFullName = report.S3FileFullName,
                Parent = parent,
            };
            return fileNode;
        }

        public NavigatorReportDetailPanelDTO NavigatorGetReportDetail(string nodePath, int userId, int roleId, int districtId)
        {
            if (string.IsNullOrEmpty(nodePath))
            {
                return new NavigatorReportDetailPanelDTO();
            }

            var rootNode = BuildNavigatorReportNodes(userId, roleId, districtId);
            var current = rootNode.FindNodeByPath(nodePath);
            if (current == null)
                return new NavigatorReportDetailPanelDTO();

            if (current.IsFile() && current is NavigatorReportFileNodeDto leafNode)
            {
                return GetReportFileDetail(leafNode.NavigatorReportId, userId, roleId);
            }

            var navigatorReportId = current.FindAllLeafNodes()
                   .Select(x => x.NavigatorReportId)
                   .ToIntCommaSeparatedString();
            if (current.IsSchoolFolder())
            {
                return GetReportSchoolDetail(navigatorReportId, userId, districtId, roleId);
            }

            return GetReportFolderDetail(navigatorReportId, userId, districtId, roleId);
        }

        private NavigatorReportDetailPanelDTO GetReportSchoolDetail(string navigatorReportId, int userId, int districtId, int roleId)
        {
            NavigatorReportDetailPanelDTO reportDetail = new NavigatorReportDetailPanelDTO();
            List<NavigatorSchoolFolderDetailDTO> schoolFolderDetail = _navigatorReportRepository.NavigatorGetSchoolFolderDetail(userId, districtId, roleId, navigatorReportId)
                                .GroupBy(c => c.ReportName)
                                .Select(c => new NavigatorSchoolFolderDetailDTO()
                                {
                                    ReportName = c.Key,
                                    RoleDescriptions = c.OrderBy(r => r.ORD ?? 0).Select(r => new SchoolViewRolePublishingDesctiption()
                                    {
                                        PublishStatus = r.PublishStatus,
                                        RoleShortName = r.RoleShortName,
                                        RoleToolTip = r.RoleTooltip,
                                        RoleId = r.RoleId ?? (int)RoleEnum.Student,
                                    }).Where(rd => CanPublishToRole((RoleEnum)roleId, (RoleEnum)rd.RoleId))
                                    .ToList()
                                }).ToList();
            reportDetail.SchoolFolderDetail = schoolFolderDetail;
            reportDetail.HideManageAccessButton = schoolFolderDetail.SelectMany(c => c.RoleDescriptions).Count() == 0;
            return reportDetail;
        }

        private NavigatorReportDetailPanelDTO GetReportFolderDetail(string navigatorReportId, int userId, int districtId, int roleId)
        {
            NavigatorReportDetailPanelDTO reportDetail = new NavigatorReportDetailPanelDTO();

            var folderInformation = NavigatorGetReportFolderDetail(navigatorReportId, userId, districtId, roleId);
            var reportDetailCountOnly = new ReportDetailCountOnlyDTO()
            {
                ReportCount = folderInformation.ReportCount ?? 0,
                ReportPublishedCount = folderInformation.PublishedReportCount ?? 0
            };
            reportDetail.ReportDetailCountOnly = reportDetailCountOnly;
            var canPublish =
                ((folderInformation.CanPublishSchoolAdmin ?? false) && CanPublishToRole((RoleEnum)roleId, RoleEnum.SchoolAdmin))
                ||
                ((folderInformation.CanPublishTeacher ?? false) && CanPublishToRole((RoleEnum)roleId, RoleEnum.Teacher))
                ||
                ((folderInformation.CanPublishStudent ?? false) && CanPublishToRole((RoleEnum)roleId, RoleEnum.Student))
                ;
            reportDetail.HideManageAccessButton = !canPublish;
            return reportDetail;
        }

        private NavigatorReportDetailPanelDTO GetReportFileDetail(int navigatorReportId, int userId, int roleId)
        {
            NavigatorReportDetailPanelDTO reportDetail = new NavigatorReportDetailPanelDTO();
            NavigatorReportFileDetailDTO fileDetail = NavigatorGetReportDetailGetFileDetail(navigatorReportId, userId, roleId);
            if (fileDetail != null)
            {
                fileDetail.FilePublishStatus = fileDetail.FilePublishStatus.Where(CanViewPublishStatusForFileDetail(roleId)).ToList();
                fileDetail.HidePublishToSection = fileDetail.FilePublishStatus.Count == 0;
            }
            reportDetail.FileDetail = fileDetail;
            reportDetail.HideManageAccessButton = fileDetail.HidePublishToSection;
            reportDetail.KeywordMandatory = fileDetail.KeywordMandatory;
            return reportDetail;
        }

        private NavigatorReportFileDetailDTO NavigatorGetReportDetailGetFileDetail(int navigatorReportId, int userId, int roleId)
        {
            return _navigatorReportRepository.GetNavigatorReportFileDetail(navigatorReportId, userId, roleId)
                .Select(c => new NavigatorReportFileDetailDTO()
                {
                    PrimaryKeyword = c.PrimaryKeyword,
                    OtherKeywords = c.OtherKeywords,
                    KeywordMandatory = c.KeywordMandatory.GetValueOrDefault(),
                    FilePublishStatus = new List<NavigatorReportFileDetailRoleListDTO>()
                {
                         new NavigatorReportFileDetailRoleListDTO()
                         {
                              RoleName = "District Admin",
                              RoleId = (int)RoleEnum.DistrictAdmin,
                             Count = c.TotalDistrictAdmin ?? 0 ,
                             CanPublish =( c.CanPublishDistrictAdmin ?? false ) && CanPublishToRole((RoleEnum)roleId,RoleEnum.DistrictAdmin),
                             Published = c.TotalDistrictAdminPublished ?? 0
                         },
                         new NavigatorReportFileDetailRoleListDTO()
                         {
                              RoleName = "School Admin",
                              RoleId = (int)RoleEnum.SchoolAdmin,
                             Count = c.TotalSchoolAdmin ?? 0 ,
                             CanPublish =( c.CanPublishSchoolAdmin ?? false ) && CanPublishToRole((RoleEnum)roleId,RoleEnum.SchoolAdmin),
                             Published = c.TotalSchoolAdminPublished ?? 0
                         },
                           new NavigatorReportFileDetailRoleListDTO()
                         {
                             RoleName = "Teacher",
                              RoleId = (int)RoleEnum.Teacher,
                             Count = c.TotalTeacher?? 0 ,
                             CanPublish = c.CanPublishTeacher ?? false && CanPublishToRole((RoleEnum)roleId,RoleEnum.Teacher) ,
                             Published = c.TotalTeacherPublished ?? 0
                         },
                             new NavigatorReportFileDetailRoleListDTO()
                         {
                              RoleName = "Student",
                              RoleId = (int)RoleEnum.Student,
                             Count = c.TotalStudent?? 0 ,
                             CanPublish = c.CanPublishStudent ?? false && CanPublishToRole((RoleEnum)roleId,RoleEnum.Student) ,
                             Published = c.TotalStudentPublished ?? 0
                         },
                }
                }).FirstOrDefault();
        }

        private Func<NavigatorReportFileDetailRoleListDTO, bool> CanViewPublishStatusForFileDetail(int roleId)
        {
            return x => CanPublishToRole((RoleEnum)roleId, (RoleEnum)x.RoleId);
        }

        private NavigatorGetReportFolderDetailResult NavigatorGetReportFolderDetail(string navigatorReportId, int userId, int districtId, int roleId)
        {
            var rolesToBePublish = new RoleEnum[] { RoleEnum.DistrictAdmin, RoleEnum.Teacher, RoleEnum.SchoolAdmin, RoleEnum.Student }
                .Where(c => CanPublishToRole((RoleEnum)roleId, c))
                .Cast<int>()
                .ToArray();
            var rolesToBePublishString = string.Join(",", rolesToBePublish);

            var folderDetail = _navigatorReportRepository.NavigatorGetReportFolderDetail(navigatorReportId, rolesToBePublishString, userId, roleId, districtId).FirstOrDefault();
            return folderDetail;

        }

        public IEnumerable<int> GetNavigatorReportIdsByNodePaths(IEnumerable<string> nodePaths, int userId, int roleId, int? districtId)
        {
            var leafNodes = FindAllLeafNodes(nodePaths, userId, roleId, districtId);
            return leafNodes
                .Select(x => x.NavigatorReportId)
                .Distinct()
                .ToList();
        }

        private IEnumerable<NavigatorReportFileNodeDto> FindAllLeafNodes(IEnumerable<string> nodePaths, int userId, int roleId, int? districtId)
        {
            var rootNode = BuildNavigatorReportNodes(userId, roleId, districtId);
            var leafNodes = new List<NavigatorReportFileNodeDto>();
            foreach (var nodePath in nodePaths)
            {
                var current = rootNode.FindNodeByPath(nodePath);
                if (current != null)
                    leafNodes.AddRange(current.FindAllLeafNodes());
            }
            return leafNodes;
        }

        public IEnumerable<PublishByRoleRoleDefinitionDTO> GetRolesToPublishByNodePaths(string nodePath, int userId, int roleId, int? districtId)
        {
            var nodePaths = nodePath.Split(new string[] { "-_-" }, StringSplitOptions.RemoveEmptyEntries);
            var navigatorReportNodes = FindAllLeafNodes(nodePaths, userId, roleId, districtId);
            if (!navigatorReportNodes.Any())
                return Enumerable.Empty<PublishByRoleRoleDefinitionDTO>();

            var navigatorConfigurationIDs = navigatorReportNodes.Select(x => x.NavigatorConfigurationID).Distinct().ToList();
            var navigatorConfigurations = _navigatorConfigurationRepository.Select()
                .Where(x => navigatorConfigurationIDs.Contains(x.NavigatorConfigurationID))
                .ToList();

            var publishRoles = new List<(Func<NavigatorConfigurationDTO, bool> Predicate, int RoleId, string RoleName)>
            {
                (predicate => predicate.CanPublishDistrictAdmin, (int)RoleEnum.DistrictAdmin, "District Admin"),
                (predicate => predicate.CanPublishSchoolAdmin, (int)RoleEnum.SchoolAdmin, "School Admin"),
                (predicate => predicate.CanPublishTeacher, (int)RoleEnum.Teacher, "Teacher"),
                (predicate => predicate.CanPublishStudent, (int)RoleEnum.Student, "Student"),
            };

            var roleDefinitions = new List<PublishByRoleRoleDefinitionDTO>();

            foreach (var publishRole in publishRoles)
            {
                var configs = navigatorConfigurations.Where(publishRole.Predicate);
                if (configs.Any())
                {
                    var reports = navigatorReportNodes
                        .Where(x => configs.Any(y => x.NavigatorConfigurationID == y.NavigatorConfigurationID))
                        .DistinctBy(x => x.NavigatorReportId);
                    roleDefinitions.Add(new PublishByRoleRoleDefinitionDTO()
                    {
                        RoleId = publishRole.RoleId,
                        RoleName = publishRole.RoleName,
                        ReportTypesThatCanPublish = reports.Select(x => x.Name).ToArray(),
                    });
                }
            }

            roleDefinitions = roleDefinitions.Where(c => CanPublishToRole((RoleEnum)roleId, (RoleEnum)c.RoleId)).ToList();
            return roleDefinitions;
        }
        private IEnumerable<int> GetRolesThatCanBePublished(int publisherRoleId, IEnumerable<int> rolesAboutToBePublished)
        {
            return rolesAboutToBePublished.Where(c => CanPublishToRole((RoleEnum)publisherRoleId, (RoleEnum)c)).ToArray();
        }
        public BaseResponseModel<NavigatorReportPublishByRoleResultDTO> PublishByRoleAndNodePaths(NavigatorPublishByRoleDTO navigatorPublishByRole, EmailCredentialSetting emailCredentialSetting)
        {
            try
            {

                var roleToBePublish = string.Join(",", GetRolesThatCanBePublished(navigatorPublishByRole.RoleId, navigatorPublishByRole.RoleIds));

                var excludeUserIds = navigatorPublishByRole.IsNotSendingTo ? navigatorPublishByRole.ExcludeUserIds : string.Empty;

                excludeUserIds = string.Join(",", (excludeUserIds ?? "").ToIntArray().Concat(new int[] { navigatorPublishByRole.UserId }));
                navigatorPublishByRole.CustomNote = !string.IsNullOrEmpty(navigatorPublishByRole.CustomNote) ?
                                                    Regex.Replace(navigatorPublishByRole.CustomNote, @"\r\n?|\n|\\r\\n?|\\n", "<br>")
                                                    : string.Empty;
                navigatorPublishByRole.CustomNote = navigatorPublishByRole.CustomNote.Replace("\\", "");

                var navigatorReportIds = GetNavigatorReportIdsByNodePaths(navigatorPublishByRole.NodePaths, navigatorPublishByRole.UserId, navigatorPublishByRole.RoleId, navigatorPublishByRole.DistrictId);
                var navigatorReportId = navigatorReportIds.ToIntCommaSeparatedString();
                var publishResult = _navigatorReportPublishRepository.PublishByRoleAndIdentifiers(navigatorReportId, roleToBePublish, navigatorPublishByRole.UserId, navigatorPublishByRole.RoleId, navigatorPublishByRole.DistrictId, excludeUserIds);
                if (!publishResult.IsSuccess)
                {
                    return BaseResponseModel<NavigatorReportPublishByRoleResultDTO>.InstanceError(publishResult.Message, "Error", null);
                }

                var sendNotifyEmailResult = SendEmailAfterPublishReports(new NavigatorSendEmailAfterPublishDto()
                {
                    PublishResult = publishResult.StrongData,
                    UserId = navigatorPublishByRole.UserId,
                    RoleId = navigatorPublishByRole.RoleId,
                    AlsoSendEmail = navigatorPublishByRole.AlsoSendEmail,
                    CustomNote = navigatorPublishByRole.CustomNote,
                    DistrictId = navigatorPublishByRole.DistrictId,
                    EmailCC = navigatorPublishByRole.EmailCC,
                    EmailTo = navigatorPublishByRole.EmailTo,
                    ExcludeUserIds = navigatorPublishByRole.ExcludeUserIds,
                    IgnoreEmailToCheck = false,
                    GeneralUrl = navigatorPublishByRole.GeneralUrl
                }, emailCredentialSetting);
                return BaseResponseModel<NavigatorReportPublishByRoleResultDTO>.InstanceSuccess(new NavigatorReportPublishByRoleResultDTO()
                {
                    TotalRelatedReportCount = publishResult?.StrongData?.ReportCount ?? 0,
                    SendNotifyEmailResult = sendNotifyEmailResult
                });
            }
            catch (Exception ex)
            {
                return BaseResponseModel<NavigatorReportPublishByRoleResultDTO>.InstanceError(ex.Message, "Error", null);
            }
        }

        private BaseResponseModel<bool> SendEmailAfterPublishReports(NavigatorSendEmailAfterPublishDto model, EmailCredentialSetting emailCredentialSetting)
        {
            PublishResultDto publishResult = model.PublishResult;
            if (!model.AlsoSendEmail)
            {
                return BaseResponseModel<bool>.InstanceSuccess(true);
            }
            try
            {
                model.EmailCC = new string[0];
                var excludeUserMailIds = new int[0];
                if (!string.IsNullOrEmpty(model.ExcludeUserIds))
                {
                    excludeUserMailIds = model.ExcludeUserIds.ToIntArray();
                }

                var isPublisher = model.RoleId == (int)RoleEnum.Publisher;
                NavigatorNotifyPublishing(new NavigatorNotifyPublishingDto()
                {
                    DistrictId = model.DistrictId ?? 0,
                    PublisherUserId = model.UserId,
                    PublisherRoleId = model.RoleId,
                    CustomNote = model.CustomNote,
                    InitiatorName = publishResult.InitiatorName,
                    InitiatorEmail = publishResult.InitiatorEmail,
                    ExcludeUserMailIds = excludeUserMailIds,
                    PublishByRoleEmailList = publishResult?.NewPublishRecordInformation.ToPublishByRoleEmailList(isPublisher, model.EmailTo, model.EmailCC, model.IgnoreEmailToCheck),
                    GeneralUrl = model.GeneralUrl
                }, emailCredentialSetting);
                return BaseResponseModel<bool>.InstanceSuccess(true);
            }
            catch (Exception ex)
            {
                return BaseResponseModel<bool>.InstanceError(ex.Message, "error sending email", false);
            }
        }

        public NavigatorReportDownloadFilterDto GetFilterDownload(string nodePath, int userId, int roleId, int districtId)
        {
            var nodePaths = nodePath.Split(new string[] { "-_-" }, StringSplitOptions.RemoveEmptyEntries);
            var rootNode = BuildNavigatorReportNodes(userId, roleId, districtId);
            int navigatorReportId = 0;
            foreach (var path in nodePaths)
            {
                var current = rootNode.FindNodeByPath(path);
                if (current != null && current.IsLeaf() && current is NavigatorReportFileNodeDto leafNode)
                    navigatorReportId = leafNode.NavigatorReportId;
            }

            if (roleId == (int)RoleEnum.Parent)
            {
                var students = _manageParentRepository.GetStudentByNavigatorReportAndParent(navigatorReportId, userId)
                    .Select(x => new SelectListItemDTO()
                    {
                        Id = x.StudentID,
                        Name = x.FullName
                    }).ToList();
                return new NavigatorReportDownloadFilterDto
                {
                    Students = students,
                };
            }
            else
            {
                return _navigatorReportRepository.GetFilterDownloadFile(navigatorReportId, userId);
            }
        }

        public NavigatorConfigurationDTO GetNavigatorConfiguration(string nodePath, int currUserId, int roleId, int districtId)
        {
            var nodePaths = nodePath.Split(new string[] { "-_-" }, StringSplitOptions.RemoveEmptyEntries);
            var leafNodes = FindAllLeafNodes(nodePaths, currUserId, roleId, districtId);
            var navigatorConfigurationIds = leafNodes.Select(x => x.NavigatorConfigurationID).Distinct().ToList();

            var navigatorConfiguration = _navigatorConfigurationRepository.Select()
                                        .FirstOrDefault(x => navigatorConfigurationIds.Contains(x.NavigatorConfigurationID));
            return navigatorConfiguration;
        }

        public bool DontHaveRightToPublish(int roleId)
        {
            return !new RoleEnum[] { RoleEnum.DistrictAdmin, RoleEnum.Publisher, RoleEnum.NetworkAdmin, RoleEnum.SchoolAdmin, RoleEnum.Teacher }
            .Cast<int>().Contains(roleId);
        }

        public IEnumerable<NavigatorReportFillTableResultDto> OnFillTable(IEnumerable<NavigatorReportFillTableDto> forms)
        {
            ConcurrentBag<NavigatorReportFillTableResultDto> results = new ConcurrentBag<NavigatorReportFillTableResultDto>();

            var navigatorReports = _navigatorConfigurationRepository.Select().Where(x => forms.Select(s => s.NavigatorCategory).Contains(x.NavigatorCategoryID)).ToArray();

            foreach (var item in forms.Where(x => !string.IsNullOrEmpty(x.FilePath)).AsParallel())
            {
                if (item.FilePath.IndexOf("/") > 0)
                    item.FilePath = item.FilePath.Substring(item.FilePath.IndexOf("/", 1));

                var navigators = navigatorReports.Where(x => x.NavigatorCategoryID == item.NavigatorCategory && !string.IsNullOrEmpty(x.ReportTypePattern));

                foreach (var navigator in navigators)
                {
                    var reportTypePattern = ExtensionMethods.DeserializeObject<NavigatorReportPatternDto>(navigator.ReportTypePattern);

                    // District or Static name
                    bool pathMatched = reportTypePattern.PathName == item.FilePath;

                    // School
                    string schoolName = string.Empty;
                    if (!pathMatched && item.FilePath != "/District")
                    {
                        schoolName = item.FilePath.Split(new Char[] { '/' }).FirstOrDefault(x => !string.IsNullOrEmpty(x));

                        string defineSchoolPattern = item.FilePath.Replace(schoolName, "{schoolName}");
                        pathMatched = reportTypePattern.PathName == defineSchoolPattern;
                    }

                    if (pathMatched && Regex.IsMatch(item.FileName, reportTypePattern.FileName, RegexOptions.IgnoreCase))
                    {
                        var result = new NavigatorReportFillTableResultDto
                        {
                            DataRow = item.DataRow,
                            ReportTypeId = navigator.NavigatorConfigurationID,
                            SchoolName = navigator.UseSchool && !string.IsNullOrEmpty(navigator.SchoolPattern) ? navigator.SchoolPattern.Replace("{schoolName}", schoolName) : string.Empty
                        };

                        string suffixPatternMatched = ExtensionMethods.DeserializeObject<List<string>>(navigator.SuffixPattern)
                                                                     ?.FirstOrDefault(pattern => Regex.IsMatch(item.FileName, pattern, RegexOptions.IgnoreCase));

                        if (!string.IsNullOrEmpty(suffixPatternMatched))
                            result.ReportSuffix = Regex.Match(item.FileName, suffixPatternMatched, RegexOptions.IgnoreCase).Value;

                        results.Add(result);
                        break;
                    }
                }
            }

            return results.OrderBy(o => o.DataRow);
        }

        public IEnumerable<NavigatorUserEmailDto> GetAssociateEmailsWhichNotPublished(string nodePath, string selectedRoleIds, int userId, int roleId, int? districtId)
        {
            var result = new List<NavigatorUserEmailDto>();
            var excludesRoleGetData = new List<int>()
            {
                (int)RoleEnum.Teacher,
                (int)RoleEnum.Student
            };
            // restrict invalid roles
            var rolesThatCanPublish = this.GetRolesToPublishByNodePaths(nodePath, userId, roleId, districtId).Select(c => c.RoleId).ToArray();
            var roleIds = selectedRoleIds?.ToIntArray("-_-");

            roleIds = roleIds.Where(c => rolesThatCanPublish.Contains(c) && !excludesRoleGetData.Contains(c)).ToArray();

            bool showDistrictAdmin = roleIds.Contains(3);
            bool showSchoolAdmin = roleIds.Contains(8);
            bool showTeacher = roleIds.Contains(2);
            bool showStudent = roleIds.Contains(28);

            var nodePaths = nodePath.Split(new string[] { "-_-" }, StringSplitOptions.RemoveEmptyEntries);
            var navigatorReportId = GetNavigatorReportIdsByNodePaths(nodePaths, userId, roleId, districtId).ToIntCommaSeparatedString();
            var userEmails = _navigatorReportRepository.NavigatorGetAssociateEmailsWhichNotPublished(navigatorReportId, userId, roleId, districtId, showDistrictAdmin, showSchoolAdmin, showTeacher, showStudent)
                .Select(x => new NavigatorUserEmailDto
                {
                    UserId = x.UserID,
                    Email = x.Email,
                    RoleId = x.RoleID,
                    FirstName = x.NameFirst,
                    LastName = x.NameLast,
                    Code = x.Code
                }).Distinct();

            return GetAssociateEmails(userEmails.ToList());
        }

        private IEnumerable<NavigatorUserEmailDto> GetAssociateEmails(List<NavigatorUserEmailDto> userEmails)
        {
            var duplicateNames = userEmails.GroupBy(x => new { LastName = x.LastName.ToLower(), FirstName = x.FirstName.ToLower() }).Where(x => x.Count() > 1).Select(x => BuildUserFullName(x.Key.FirstName, x.Key.LastName).ToLower());

            return userEmails.Where(x => duplicateNames.Contains(BuildUserFullName(x.FirstName, x.LastName).ToLower()))
                                .Select(x => new NavigatorUserEmailDto
                                {
                                    UserId = x.UserId,
                                    Email = x.Email,
                                    RoleId = x.RoleId,
                                    DisplayName = BuildUserFullName(x.FirstName, x.LastName, x.Code)
                                })
                .Union(userEmails.Where(x => !duplicateNames.Contains(BuildUserFullName(x.FirstName, x.LastName).ToLower()))
                                .Select(x => new NavigatorUserEmailDto
                                {
                                    UserId = x.UserId,
                                    Email = x.Email,
                                    RoleId = x.RoleId,
                                    DisplayName = BuildUserFullName(x.FirstName, x.LastName)
                                }));
        }

        private string BuildUserFullName(string firstName, string lastName, string code = "")
        {
            return !string.IsNullOrEmpty(code) ? string.Format("{0} {1} ({2})", firstName, lastName, code) : string.Format("{0} {1}", firstName, lastName).Trim();
        }


        #region Build Email Report

        public void NavigatorNotifyPublishing(NavigatorNotifyPublishingDto navigatorNotifyPublishingDto, EmailCredentialSetting emailCredentialSetting)
        {
            if (navigatorNotifyPublishingDto != null)
            {
                var listNavigatorMailMessage = NavigatorBuildMailMessage(navigatorNotifyPublishingDto);
                if (listNavigatorMailMessage != null && listNavigatorMailMessage.Count > 0)
                {
                    try
                    {
                        Task.Factory.StartNew(() =>
                        {
                            listNavigatorMailMessage
                            .AsParallel()
                            .ForAll(itemEmailMessage =>
                            {
                                try
                                {
                                    _emailService.SendEmailNavigator(itemEmailMessage, emailCredentialSetting);
                                }
                                catch (Exception ex2)
                                {
                                }
                            });
                        },
                        System.Threading.Tasks.TaskCreationOptions.DenyChildAttach);
                    }
                    catch (Exception ex1)
                    {
                        ;
                    }
                }
            }
        }
        private List<NavigatorReportEmailModelDTO> BuildMailMessageReport(NavigatorNotifyPublishingDto navigatorNotifyPublishingDto)
        {
            var objConfig = _districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(navigatorNotifyPublishingDto.DistrictId, Constanst.NAVIGATOR_TEMPLATE_EMAIL_BODY_ALL_ROLE);
            string default_template = objConfig?.Value ?? "";

            var objSubjectSendMailOtherRole = _districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(navigatorNotifyPublishingDto.DistrictId, Constanst.NAVIGATOR_SUBJECT_EMAIL_KEY);

            if (navigatorNotifyPublishingDto.PublishByRoleEmailList.NormalUserEmailList?.Length > 0)
            {
                var objDistrict = _districtRepository.Select().FirstOrDefault(o => o.Id == navigatorNotifyPublishingDto.DistrictId);
                string strDistrictName = objDistrict == null ? string.Empty : objDistrict.Name;


                var emailList = navigatorNotifyPublishingDto.PublishByRoleEmailList.NormalUserEmailList
                    .Where(c => !string.IsNullOrEmpty(c.UserInfo.Email))
                        .GroupBy(c => c.UserInfo.RoleId)
                        .Select(group =>
                        {
                            string strEmailTemplate = GetNavigatorReportTemplateByRole(roleId: group.Key, districtId: navigatorNotifyPublishingDto.DistrictId);
                            if (string.IsNullOrEmpty(strEmailTemplate))
                            {
                                strEmailTemplate = default_template;
                            }
                            return new
                            {
                                group,
                                strEmailTemplate
                            };
                        })
                        .AsParallel()
                        .Select(group =>
                        {

                            return group.group.Select(normalUserEmail =>
                            {
                                return BuildEmailContent(navigatorNotifyPublishingDto, normalUserEmail, group.strEmailTemplate, objSubjectSendMailOtherRole, strDistrictName);
                            }).ToArray();
                        })
                        .Aggregate(new List<NavigatorReportEmailModelDTO>(), (seed, result) =>
                        {
                            var newSeed = seed.ToList();
                            newSeed.AddRange(result);
                            return newSeed;
                        });
                return emailList;
            }
            return new List<NavigatorReportEmailModelDTO>();
        }

        private string GetNavigatorReportTemplateByRole(int roleId, int districtId)
        {
            string templateId = string.Empty;
            switch (roleId)
            {
                case (int)RoleEnum.DistrictAdmin:
                case (int)RoleEnum.NetworkAdmin:
                    templateId = Constanst.NAVIGATOR_TEMPLATE_EMAIL_BODY_DISTRICT_ADMIN;
                    break;
                case (int)RoleEnum.SchoolAdmin:
                    templateId = Constanst.NAVIGATOR_TEMPLATE_EMAIL_BODY_SCHOOL_ADMIN;
                    break;
                case (int)RoleEnum.Teacher:
                    templateId = Constanst.NAVIGATOR_TEMPLATE_EMAIL_BODY_TEACHER;
                    break;
                case (int)RoleEnum.Student:
                    templateId = Constanst.NAVIGATOR_TEMPLATE_EMAIL_BODY_STUDENT;
                    break;
                default:
                    templateId = Constanst.NAVIGATOR_TEMPLATE_EMAIL_BODY_ALL_ROLE;
                    break;
            }
            return _districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId, templateId)?.Value ?? string.Empty;
        }

        private static NavigatorReportEmailModelDTO BuildEmailContent(NavigatorNotifyPublishingDto navigatorNotifyPublishingDto, PublishByRoleNormalUserEmailDto normalUserEmail, string strEmailTemplate, DistrictDecode objSubjectSendMailOtherRole, string strDistrictName)
        {
            NavigatorReportEmailModelDTO navigatorReportEmail = new NavigatorReportEmailModelDTO();

            var publishBy = string.Empty;
            var signFullName = "The Linkit! Team";
            if (navigatorNotifyPublishingDto.PublisherRoleId != (int)RoleEnum.Publisher)
            {
                publishBy = navigatorNotifyPublishingDto.InitiatorName.Replace(",", "");
                signFullName = $"The {strDistrictName} Team";
            }
            string strSubjectNavigatorOtherRole = objSubjectSendMailOtherRole == null ? Constanst.NAVIGATOR_SUBJECT_EMAIL_SUMMARY : objSubjectSendMailOtherRole.Value;
            navigatorReportEmail.Subject = strSubjectNavigatorOtherRole.Replace("[DistrictName]", strDistrictName);
            strEmailTemplate = strEmailTemplate.Replace("[FirstName_LastName]", normalUserEmail.UserInfo.UserFullName.Replace(",", ""));
            strEmailTemplate = strEmailTemplate.Replace("[PublishedBy]", publishBy);
            strEmailTemplate = strEmailTemplate.Replace("[CustomNote]", navigatorNotifyPublishingDto.CustomNote);
            strEmailTemplate = strEmailTemplate.Replace("[SignFullName]", signFullName);
            strEmailTemplate = strEmailTemplate.Replace("[SchoolName]", normalUserEmail.UserInfo.SchoolName);
            strEmailTemplate = strEmailTemplate.Replace("[DistrictURL]", navigatorNotifyPublishingDto.GeneralUrl.Replace(Constanst.NAVIGATOR_DISTRICT_CODE_SUB_DOMAIN, normalUserEmail.UserInfo.LICode));

            string breadCrumbsHtml = string.Join("<br>", normalUserEmail.Reports.Select(c => $"<span style='font-size:15px;'>&#8729;</span> <span>{c.Breadcrumb} ({c.FileType})</span>"));
            strEmailTemplate = strEmailTemplate.Replace("[BreadCrumbsHtml]", breadCrumbsHtml);

            navigatorReportEmail.Body = strEmailTemplate.Replace("\r\n<br>\r\n<br>\r\n<br>", "\r\n<br>").Replace("\r\n<br>\r\n<br>", "\r\n<br>");
            if (CommonUtils.IsValidEmail(normalUserEmail.UserInfo.Email) && !navigatorNotifyPublishingDto.ExcludeUserMailIds.Contains(normalUserEmail.UserInfo.UserId))
            {
                navigatorReportEmail.ArrEmailTo.Add(normalUserEmail.UserInfo.Email);
                return navigatorReportEmail;
            }
            return null;
        }

        private List<NavigatorReportEmailModelDTO> NavigatorBuildMailMessage(NavigatorNotifyPublishingDto navigatorNotifyPublishingDto)
        {
            if (navigatorNotifyPublishingDto.PublishByRoleEmailList?.NormalUserEmailList?.Length > 0)
            {
                var listEmailMessagePublishingOtherRole = BuildMailMessageReport(navigatorNotifyPublishingDto);
                return listEmailMessagePublishingOtherRole;
            }
            return new List<NavigatorReportEmailModelDTO>();
        }

        public IEnumerable<ManageAccessPublishDetailDto> GetManageAccessPublishDetail(string nodePath, string checkedUserIds, int userId, int roleId, int districtId)
        {
            var nodePaths = nodePath.Split(new string[] { "-_-" }, StringSplitOptions.RemoveEmptyEntries);
            var navigatorReportId = GetNavigatorReportIdsByNodePaths(nodePaths, userId, roleId, districtId).ToIntCommaSeparatedString();
            var publishDetails = _navigatorReportRepository.GetManageAccessPublishDetail(navigatorReportId, checkedUserIds, userId, roleId, districtId);

            var reports = publishDetails.GetResult<ManageAccessNavigatorReportNameDto>().ToArray();
            var users = publishDetails.GetResult<ManageAccessNavigatorUserMappingDto>().ToArray();

            var publishDetailMapped = (from user in users
                                       join report in reports
                                       on user.NavigatorReportID equals report.NavigatorReportID
                                       select new { user, report })
                                       .GroupBy(c => c.user.RoleId)
                                       .Select(c => new ManageAccessPublishDetailDto()
                                       {
                                           RoleId = c.Key,
                                           ReportNames = c.Select(cc => cc.report).Select(cc => cc.ReportName)
                                           .DistinctBy(cc => cc.ToLower())
                                           .ToArray()
                                       })
                                       .OrderBy(c => c.Order)
                                       .ToArray();

            return publishDetailMapped;
        }

        private void SendEmailReportErrorLog(string errorMessage, DistributeSetting distributeSetting)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                var messages = BuildMessageQueue(errorMessage);
                var queueUrl = ConfigurationManager.AppSettings["SendmaiQueueUrl"];
                if (messages != null && messages.Count > 0)
                {
                    var maxItemCount = 10;
                    var batchCount = messages.Count / maxItemCount;
                    if (messages.Count % maxItemCount > 0)
                        batchCount++;

                    try
                    {
                        for (int i = 0; i < batchCount; i++)
                        {
                            var batchMessages = messages.Skip(i * maxItemCount).Take(maxItemCount).ToList();
                            _messageQueueService.SendMessageBatch(queueUrl, batchMessages);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
        }

        private List<MessageQueueDto> BuildMessageQueue(string errorMessage)
        {
            var results = new List<MessageQueueDto>();
            var subjects = new Dictionary<string, string>()
                {
                    { "Subject", "Navigator Report Error Log"}
                };

            var contents = new Dictionary<string, string>()
                {
                    { "ErrorMessage", errorMessage}
                };
            var messageQueue = new MessageQueueDto()
            {
                ServiceType = MessageQueueServiceTypeEnums.EMAIL.DescriptionAttr(),
                Key = "NAVIGATOR_ERROR_LOG",
                DistrictId = 0,
                EmailTo = "thachtx@devblock.net",
                Data = new EmailData()
                {
                    Subjects = subjects,
                    Contents = contents
                }
            };
            results.Add(messageQueue);

            return results;
        }
        #endregion
    }
}
