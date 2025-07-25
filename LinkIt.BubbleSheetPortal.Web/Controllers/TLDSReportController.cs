using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.TLDS;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS;
using S3Library;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using System.Net;
using LinkIt.BubbleSheetPortal.Web.Models.TLDS;
using Ionic.Zip;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using System.Threading;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.ViewModels.TLDSDigital;
using Twilio;
using Autofac.Core;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class TLDSReportController : BaseController
    {
        private readonly TLDSReportControllerParameters _parameters;
        private readonly TLDSDigitalSection23Service _tldsigitalSection23Service;
        private readonly IS3Service _s3Service;

        private List<TLDSDevelopmentOutcomeTypeViewModel> _tldsDevelopmentOutcomeTypeList = null;

        private List<TLDSDevelopmentOutcomeTypeViewModel> TLDSDevelopmentOutcomeTypeList
        {
            get
            {
                if (_tldsDevelopmentOutcomeTypeList == null)
                {
                    _tldsDevelopmentOutcomeTypeList = new List<TLDSDevelopmentOutcomeTypeViewModel>
                    {
                        new TLDSDevelopmentOutcomeTypeViewModel
                        {
                            DevelopmentOutcomeTypeId = 1,
                            Name = "IDENTITY",
                            Status = 1
                        },
                        new TLDSDevelopmentOutcomeTypeViewModel
                        {
                            DevelopmentOutcomeTypeId = 2,
                            Name = "COMMUNITY",
                            Status = 1
                        },
                        new TLDSDevelopmentOutcomeTypeViewModel
                        {
                            DevelopmentOutcomeTypeId = 3,
                            Name = "WELLBEING",
                            Status = 1
                        },
                        new TLDSDevelopmentOutcomeTypeViewModel
                        {
                            DevelopmentOutcomeTypeId = 4,
                            Name = "LEARNING",
                            Status = 1
                        },
                        new TLDSDevelopmentOutcomeTypeViewModel
                        {
                            DevelopmentOutcomeTypeId = 5,
                            Name = "COMMUNICATION",
                            Status = 1
                        },
                    };
                }
                return _tldsDevelopmentOutcomeTypeList;
            }

        }

        public TLDSReportController(TLDSReportControllerParameters parameters, TLDSDigitalSection23Service tldsigitalSection23Service, IS3Service s3Service)
        {
            _parameters = parameters;
            _tldsigitalSection23Service = tldsigitalSection23Service;
            _s3Service = s3Service;
        }

        public ActionResult Generate(TLDSReportDataViewModel model)
        {
            try
            {
                //check right
                if (!_parameters.VulnerabilityService.HasRightToAccessTLDSProfile(CurrentUser, model.ProfileId))
                {
                    //
                    return Json(new { IsSuccess = true, Error = "Has no right.", Url = string.Empty });
                }
                string url = string.Empty;
                var dateFormatCookie = System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultDateFormat];
                model.DateFormat = dateFormatCookie == null ? Constanst.DefaultDateFormatValue : dateFormatCookie.Value;

                var section1Pdf = Print(model, out url);
                var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(model.ProfileId);
                var profile = _parameters.TLDSService.GetTLDSProfile(model.ProfileId);
                var studentName = string.Format("{0} {1}", profile.LastName, profile.FirstName);
                var fileName = string.Format("{0}-{1}.pdf", studentName, model.ProfileId);
                fileName = StringUtils.GetValidFileName(fileName);
                fileName = fileName.AddTimestampToFileName();
                var s3FilePath = tldsS3Settings.GetFormPath(fileName);
                _s3Service.UploadRubricFile(tldsS3Settings.TLDSBucket, s3FilePath, new MemoryStream(section1Pdf));

                return Json(new { IsSuccess = true, Url = url, fileName = fileName });
            }
            catch (Exception)
            {
                //not let the debug session affect normal logic
            }

            return Json(new { IsSuccess = false });
        }

        [AjaxAwareAuthorize]
        public ActionResult GenerateForAdminReporting(TLDSReportDataViewModel model)
        {
            try
            {
                string url = string.Empty;
                var dateFormatCookie = System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultDateFormat];
                model.DateFormat = dateFormatCookie == null ? Constanst.DefaultDateFormatValue : dateFormatCookie.Value;

                var section1Pdf = Print(model, out url);

                var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(model.ProfileId);
                var profile = _parameters.TLDSService.GetTLDSProfile(model.ProfileId);
                var studentName = string.Format("{0} {1}", profile.LastName, profile.FirstName);
                var fileName = string.Format("{0}-{1}.pdf", studentName, model.ProfileId);
                fileName = fileName.AddTimestampToFileName();
                var s3FilePath = tldsS3Settings.GetFormPath(fileName);
                _s3Service.UploadRubricFile(tldsS3Settings.TLDSBucket, s3FilePath, new MemoryStream(section1Pdf));


                return Json(new { IsSuccess = true, Url = url, fileName = fileName });
            }
            catch (Exception)
            {
                //not let the debug session affect normal logic
            }

            return Json(new { IsSuccess = false });
        }
        private byte[] AppendUploadedDocumentInfoReport(TLDSReportDataViewModel model, byte[] section1Pdf)
        {
            var listPdfFiles = new List<byte[]>();
            listPdfFiles.Add(section1Pdf);
            var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(model.ProfileId);
            var uploadedDocuments = _parameters.TLDSService.GetTLDSUploadedDocumentByProfileId(model.ProfileId).OrderBy(x => x.UploadedDate);
            var webClient = new WebClient();
            foreach (var uploadedDocument in uploadedDocuments)
            {
                var s3Url = _s3Service.GetPublicUrl(tldsS3Settings.TLDSBucket,
                    tldsS3Settings.GetFormPath(uploadedDocument.S3FileName));
                if (!string.IsNullOrEmpty(s3Url))
                {
                    listPdfFiles.Add(webClient.DownloadData(s3Url));
                }
            }
            var fullPdf = PdfHelper.MergeFiles(listPdfFiles);
            webClient.Dispose();
            return fullPdf;
        }

        [HttpGet]
        [AjaxAwareAuthorize]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ReportItemTLDSManager)]
        public ActionResult ReportPrinting(TLDSReportDataViewModel model)
        {
            ProcessDateFormat(model);

            var masterModel = GetMasterModel(model);

            return View("ReportPrinting", masterModel);
        }

        public ActionResult ReportPrintingAuthenticated(TLDSReportDataViewModel model)
        {
            ProcessDateFormat(model);

            var masterModel = GetMasterModel(model);

            return View("ReportPrinting", masterModel);
        }

        private void ProcessDateFormat(TLDSReportDataViewModel model)
        {
            var defaultDateFormat = model.DateFormat;
            HttpCookie ckDefaultDateFormat = System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultDateFormat];
            if (ckDefaultDateFormat == null)
            {
                ckDefaultDateFormat = new HttpCookie(Constanst.DefaultDateFormat, defaultDateFormat);
                System.Web.HttpContext.Current.Response.Cookies.Add(ckDefaultDateFormat);
            }
            else
            {
                ckDefaultDateFormat.Value = defaultDateFormat;
                System.Web.HttpContext.Current.Response.Cookies.Set(ckDefaultDateFormat);
            }
        }

        private TDLSProfileViewModel GetMasterModel(TLDSReportDataViewModel model)
        {
            var masterModel = new TDLSProfileViewModel()
            {
                ProfileId = model.ProfileId
            };

            BuildGeneralData(masterModel);
            BuildTLDSAdditionalInformation(masterModel);
            BuildTLDSDevelopmentOutcomeProfile(masterModel);
            BuildTLDSEarlyABLESReport(masterModel);
            BuildTLDSOtherReportPlan(masterModel);
            BuildTLDSParentGuardian(masterModel);
            BuildTLDSProfessionalService(masterModel);
            BuildTLDSUploadPdfData(masterModel);
            BuildTLDSFormSection(masterModel);

            var eyaltProfile = masterModel.DevelopmentOutcomeProfiles.FirstOrDefault(x => x.DevelopmentOutcomeTypeId == 6
                                                                                        && !string.IsNullOrEmpty(x.OriginalFileName)
                                                                                        && !string.IsNullOrEmpty(x.S3FileName));
            if (eyaltProfile != null)
            {
                var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(model.ProfileId);
                var eyaltPdf = _s3Service.DownloadFile(LinkitConfigurationManager.GetS3Settings().TLDSBucket, tldsS3Settings.GetEYALTUploadedPath(model.ProfileId, eyaltProfile.S3FileName));
                if (eyaltPdf.IsSuccess)
                {
                    var stream = new MemoryStream(eyaltPdf.ReturnStream);
                    masterModel.EYALTFileImages.AddRange(PdfHelper.ConvertImage(stream, 300, false));
                }
            }

            return masterModel;
        }

        private void BuildTLDSFormSection(TDLSProfileViewModel model)
        {
            model.TLDSFormSectionExport = _tldsigitalSection23Service.GetTLDSProfileLinksByProfileId(model.ProfileId)
                .Select(x => new TLDSDigitalExportPDFViewModel()
                {
                    TLDSProfileLinkID = x.TLDSProfileLinkID,
                    TLDSFormSection2 = GetTLDSFormSection2(x.TLDSProfileLinkID, model.FullName),
                    TLDSFormSection3 = GetTLDSFormSection3(x.TLDSProfileLinkID)
                }).ToList();
        }

        private TldsFormSection2ViewModel GetTLDSFormSection2(Guid tldsProfileLinkId, string fullName)
        {
            var tldsFormSection2 = _tldsigitalSection23Service.GetTLDSFormSection2ByProfileLink(tldsProfileLinkId);
            if (tldsFormSection2 != null)
            {
                TldsFormSection2ViewModel tldsFormSection2ViewModel = new TldsFormSection2ViewModel()
                {
                    FullName = fullName,
                    GuardianName = tldsFormSection2.GuardianName,
                    Relationship = tldsFormSection2.Relationship,
                    Favourite = tldsFormSection2.Favourite,
                    Strengths = tldsFormSection2.Strengths,
                    Weaknesses = tldsFormSection2.Weaknesses,
                    Interested = tldsFormSection2.Interested,
                    Expected = tldsFormSection2.Expected,
                    Drawing = _s3Service.GetPublicUrl(LinkitConfigurationManager.GetS3Settings().TLDSBucket, tldsFormSection2.Drawing)
                };
                return tldsFormSection2ViewModel;
            }
            return null;
        }

        private TldsFormSection3ViewModel GetTLDSFormSection3(Guid tldsProfileLinkId)
        {
            var tldsFormSection3 = _tldsigitalSection23Service.GetTLDSFormSection3ByProfileLink(tldsProfileLinkId);
            if (tldsFormSection3 != null)
            {
                TldsFormSection3ViewModel tldsFormSection3ViewModel = new TldsFormSection3ViewModel()
                {
                    GuardianName = tldsFormSection3.GuardianName,
                    Relationship = tldsFormSection3.Relationship,
                    PreferredLanguage = tldsFormSection3.PreferredLanguage,
                    IsAborigial = tldsFormSection3.IsAborigial,
                    HaveSiblingInSchool = tldsFormSection3.HaveSiblingInSchool,
                    NameAndGradeOfSibling = tldsFormSection3.NameAndGradeOfSibling,
                    Wishes = tldsFormSection3.Wishes,
                    InformationSchool = tldsFormSection3.InformationSchool,
                    HelpInformation = tldsFormSection3.HelpInformation,
                    Interested = tldsFormSection3.Interested,
                    ConditionImprovement = tldsFormSection3.ConditionImprovement,
                    OtherInformation = tldsFormSection3.OtherInformation
                };
                return tldsFormSection3ViewModel;
            }
            return null;
        }

        private void BuildGeneralData(TDLSProfileViewModel model)
        {
            var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(model.ProfileId);
            //basic information
            var profile = _parameters.TLDSService.GetTLDSProfileIncludeMeta(model.ProfileId);
            if (profile != null)
            {
                var qualificationList = _parameters.TLDSService.GetTldsLevelQualifications();
                model.FirstName = profile.FirstName;
                model.LastName = profile.LastName;
                model.GenderId = profile.GenderId ?? 0;

                model.DateOfBirth = profile.DateOfBirth;
                model.PrimarySchool = profile.PrimarySchool;
                model.OutsideSchoolHoursCareService = profile.OutsideSchoolHoursCareService;
                model.PhotoURL = profile.PhotoURL;
                model.PhotoURL = _s3Service.GetPublicUrl(tldsS3Settings.TLDSBucket,
                    tldsS3Settings.GetPhotoPath(profile.PhotoURL));
                model.ECSName = profile.ECSName;
                model.ECSAddress = profile.ECSAddress;
                model.ECSApprovalNumber = profile.ECSApprovalNumber;
                model.ECSCompletingFormEducatorName = profile.ECSCompletingFormEducatorName;
                model.ECSCompletingFormEducatorPosition = profile.ECSCompletingFormEducatorPosition;
                model.ECSCompletingFormEducatorPhone = profile.ECSCompletingFormEducatorPhone;
                model.ECSCompletingFormEducatorEmail = profile.ECSCompletingFormEducatorEmail;
                model.ECSCompledDate = profile.ECSCompledDate;
                model.TLDSInformationHasBeenSaved = profile.TLDSInformationHasBeenSaved;
                model.ContextOfEarlyYearsSetting = ReplaceLineBreak(profile.ContextOfEarlyYearsSetting);
                model.SpecificInformation = ReplaceLineBreak(profile.SpecificInformation);
                model.ContextSpecificInforHasBeenSaved = profile.ContextSpecificInforHasBeenSaved;
                model.SectionChildInputFileUrl = profile.SectionChildInputFileUrl;
                model.SectionFamilyFileUrl = profile.SectionFamilyFileUrl;
                model.NoKWName = profile.NoKWName;
                model.NoKWPosition = profile.NoKWPosition;
                model.NoKWPhone = profile.NoKWPhone;
                model.NoKWEmail = profile.NoKWEmail;
                model.EARReportCompleted = profile.EARReportCompleted;
                model.EARReportDate = profile.EARReportDate;
                model.EARAttachmentURL = profile.EARAttachmentURL;
                model.EARAvailableUponRequest = profile.EARAvailableUponRequest;
                model.Section102HasBeenSaved = profile.Section102HasBeenSaved;
                model.UploadedChildFormFileName = profile.UploadedChildFormFileName;
                model.UploadedFamilyFormFileName = profile.UploadedFamilyFormFileName;
                model.UpcommingSchoolID = profile.UpcommingSchoolID;
                model.ParentConsentIsIncluded = profile.ParentConsentIsIncluded;
                model.SectionChildParentCompleted = profile.SectionChildParentCompleted;
                model.PrintAllSectionsFamily = profile.PrintAllSectionsFamily;
                model.WasAnEarlyABLESReportCompleted = profile.WasAnEarlyABLESReportCompleted;
                model.WillAttendASchoolInVictoria = profile.WillAttendASchoolInVictoria;
                model.HasParentSharedStatementWithSchool = profile.HasParentSharedStatementWithSchool;
                model.SchoolNotListed = profile.SchoolNotListed;
                model.Section2CheckedCompleted = profile.Section2CheckedCompleted;
                model.Section3CheckedCompleted = profile.Section3CheckedCompleted;
                model.Status = profile.Status ?? 0;
                model.LastStatusDate = profile.LastStatusDate;
                model.UserID = profile.UserID ?? 0;
                model.DateCreated = profile.DateCreated ?? DateTime.MinValue;
                model.DateUpdated = profile.DateUpdated ?? DateTime.MinValue;
                model.Section102IsNotRequired = profile.Section102IsNotRequired;
                model.ECSCompletingFormEducatorQualification = profile.ECSCompletingFormEducatorQualification;
                model.ECSCompletingFormEducatorQualificationId = profile.ECSCompletingFormEducatorQualificationId ?? 0;
                if (model.ECSCompletingFormEducatorQualificationId > 0)
                {
                    model.ECSCompletingFormEducatorQualification =
                        qualificationList.FirstOrDefault(x => x.TLDSLevelQualificationID == model.ECSCompletingFormEducatorQualificationId).Name;
                }
                model.DevelopmentOutcomeHasBeenSaved = profile.DevelopmentOutcomeHasBeenSaved;

                // 
                var genderList = _parameters.GenderService.GetAllGenders();
                var gender = genderList.FirstOrDefault(x => x.GenderID == model.GenderId);
                if (gender != null)
                {
                    model.Gender = gender.Name;
                }

                model.HasProvidedTransitionStatement = profile.HasProvidedTransitionStatement;
                model.IsAwareTransitionChildSchoolAndOSHC = profile.IsAwareTransitionChildSchoolAndOSHC;
                model.IsFamilyDidNotCompleteSection2 = profile.IsFamilyDidNotCompleteSection2;
                model.IsFamilyDidNotCompleteSection3 = profile.IsFamilyDidNotCompleteSection3;
                model.IsfamilyOptedOutTransitionStatement = profile.IsfamilyOptedOutTransitionStatement;
            }
        }

        private void BuildTLDSAdditionalInformation(TDLSProfileViewModel model)
        {
            model.AdditionalInformation = _parameters.TLDSService.GetTLDSAdditionalInformationOfProfile(model.ProfileId).Select(x => new TLDSAdditionalInformationViewModel()
            {
                AdditionalInformationId = x.AdditionalInformationID,
                ProfileId = x.ProfileID,
                AreasOfNote = x.AreasOfNote,
                StrategiesForEnhancedSupport = x.StrategiesForEnhancedSupport,
                DateCreated = x.DateCreated
            }).ToList();
        }

        private void BuildTLDSDevelopmentOutcomeProfile(TDLSProfileViewModel model)
        {
            model.DevelopmentOutcomeProfiles =
                _parameters.TLDSService.GetDevelopmentOutcomeProfileOfProfile(model.ProfileId)
                    .Select(x => new TLDSDevelopmentOutcomeProfileViewModel()
                    {
                        DevelopmentOutcomeProfileId = x.DevelopmentOutcomeProfileID,
                        DevelopmentOutcomeTypeId = x.DevelopmentOutcomeTypeID,
                        DevelopmentOutcomeTypeName = TLDSDevelopmentOutcomeTypeList.FirstOrDefault(o => o.DevelopmentOutcomeTypeId == x.DevelopmentOutcomeTypeID)?.Name,
                        DevelopmentOutcomeContent = ReplaceLineBreak(x.DevelopmentOutcomeContent),
                        StrategyContent = ReplaceLineBreak(x.StrategyContent),
                        S3FileName = x.S3FileName,
                        OriginalFileName = x.OriginalFileName,
                    }).ToList();
        }

        private void BuildTLDSEarlyABLESReport(TDLSProfileViewModel model)
        {
            model.TLDSEarlyABLESReport = _parameters.TLDSService.GetTLDSEarlyABLESReportOfProfile(model.ProfileId)
                .Select(x => new TLDSEarlyABLESReportViewModel()
                {
                    EarlyABLESReportId = x.EarlyABLESReportId,
                    ReportName = x.ReportName,
                    ReportDate = x.ReportDate,
                    LearningReadinessReportCompleted = x.LearningReadinessReportCompleted,
                    AvailableOnRequest = x.AvailableOnRequest
                }).ToList();
        }

        private void BuildTLDSOtherReportPlan(TDLSProfileViewModel model)
        {
            model.OtherReportPlans =
                _parameters.TLDSService.GetTLDSOtherReportPlanOfProfile(model.ProfileId)
                    .Select(x => new TLDSOtherReportPlanViewModel()
                    {
                        OtherReportPlanId = x.OtherReportPlanID,
                        ReportName = x.ReportName,
                        ReportDate = x.ReportDate,
                        AvailableOnRequest = x.AvailableOnRequest
                    }).ToList();

        }
        private void BuildTLDSParentGuardian(TDLSProfileViewModel model)
        {
            model.TLDSParentGuardians =
                _parameters.TLDSService.GetTLDSParentGuardianOfProfile(model.ProfileId)
                    .Select(x => new TLDSParentGuardian()
                    {
                        TLDSParentGuardianID = x.TLDSParentGuardianID,
                        TLDSProfileID = model.ProfileId,
                        ParentGuardianName = x.ParentGuardianName,
                        ParentGuardianRelationship = x.ParentGuardianRelationship,
                        ParentGuardianPhone = x.ParentGuardianPhone,
                        ParentGuardianEmail = x.ParentGuardianEmail
                    }).ToList();

        }
        private void BuildTLDSProfessionalService(TDLSProfileViewModel model)
        {
            model.ProfessionalServices =
                _parameters.TLDSService.GetTLDSProfessionalServiceOfProfile(model.ProfileId)
                    .Select(x => new TLDSProfessionalServiceViewModel()
                    {
                        ProfessionalServiceId = x.ProfessionalServiceID,
                        Name = x.Name,
                        Address = x.Address,
                        ContactPerson = x.ContactPerson,
                        Position = x.Position,
                        Phone = x.Phone,
                        Email = x.Email,
                        WrittenReportAvailable = x.WrittenReportAvailable,
                        ReportForwardedToSchoolDate = x.ReportForwardedToSchoolDate,
                        Attached = x.Attached,
                        AvailableUponRequested = x.AvailableUponRequested,
                    }).ToList();

        }

        private void BuildTLDSUploadPdfData(TDLSProfileViewModel model)
        {
            var uploadedDocuments = _parameters.TLDSService.GetTLDSUploadedDocumentByProfileId(model.ProfileId).OrderBy(x => x.UploadedDate);
            var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(model.ProfileId);
            var webClient = new WebClient();
            model.FileImages = new List<string>();
            foreach (var uploadedDocument in uploadedDocuments)
            {
                var s3Url = _s3Service.GetPublicUrl(tldsS3Settings.TLDSBucket,
                    tldsS3Settings.GetFormPath(uploadedDocument.S3FileName));
                if (!string.IsNullOrEmpty(s3Url))
                {
                    var data = webClient.DownloadData(s3Url);
                    var stream = new MemoryStream(data);
                    model.FileImages.AddRange(PdfHelper.ConvertImage(stream));
                }
            }

            webClient.Dispose();
        }
        #region Print Report TLDS
        private byte[] ExportToPDF(string url, int timezoneOffset, int profileId)
        {
            DateTime dt = DateTime.UtcNow.AddMinutes(timezoneOffset * (-1));

            string footerUrl = Url.Action("RenderFooter", "TLDSReport",
            new
            {
                profileId = profileId,
                dateTimeNow = dt
            } , HelperExtensions.GetHTTPProtocal(Request));

            string headerUrl = Url.Action("RenderHeader", "TLDSReport", new
            {
                profileId = profileId
            }, HelperExtensions.GetHTTPProtocal(Request));

            var loopIndex = 0;
            var marginValue = 0;
            byte[] buffer = null;
            while ((buffer == null || buffer.Length == 0) && loopIndex < 6)
            {
                marginValue += 5;
                loopIndex++;

                string args =
               string.Format("--footer-html \"{0}\" --header-html \"{2}\" --header-spacing {3} --footer-spacing {3} \"{1}\" - "
                   , footerUrl
                   , url
                   , headerUrl
                   , marginValue
                   );

                var startInfo = new ProcessStartInfo(Server.MapPath("~/PDFTool/wkhtmltopdf.exe"), args)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                };
                var proc = new Process { StartInfo = startInfo };
                proc.Start();

                string output = proc.StandardOutput.ReadToEnd();
                buffer = proc.StandardOutput.CurrentEncoding.GetBytes(output);
                proc.WaitForExit();
                proc.Close();
            }

            return buffer;
        }

        private byte[] Print(TLDSReportDataViewModel model, out string url)
        {
            byte[] pdf = null;
            url = string.Empty;

            url = Url.Action("ReportPrintingAuthenticated", "TLDSReport", model, HelperExtensions.GetHTTPProtocal(Request));
            pdf = ExportToPDF(url, model.TimezoneOffset, model.ProfileId);
            return pdf;
        }

        public FileStreamResult GetPDF()
        {
            FileStream fs = new FileStream("", FileMode.Open, FileAccess.Read);
            return File(fs, "application/pdf");
        }
        public ActionResult RenderFooter(int profileId, DateTime dateTimeNow)
        {
            var footerData = new TLDSReportFooterViewModel()
            {
                LeftLine1 = "Generated: " + dateTimeNow.DisplayDateWithFormat(true)
            };
            return PartialView("_Footer", footerData);
        }

        public ActionResult RenderHeader(int profileId)
        {
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);

            var headerData = new TLDSReportHeaderViewModel()
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName
            };
            return PartialView("_Header", headerData);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult GetTLDSReportS3File(string fileName, int profileId)
        {
            //check security
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);
            if (profile == null)
            {
                return Json(new { Result = false, ErrorMessage = "Profile does not exist." });
            }
            if (!_parameters.VulnerabilityService.HasRightToAccessTLDSProfile(CurrentUser, profileId))
            {
                return Json(new { Result = false, ErrorMessage = "Has no right." });
            }
            var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(profileId);
            var downloadFilePath = tldsS3Settings.GetFormPath(fileName);

            var result = _s3Service.DownloadFile(tldsS3Settings.TLDSBucket, downloadFilePath);

            if (result.IsSuccess)
            {
                var s3Url = _s3Service.GetPublicUrl(tldsS3Settings.TLDSBucket,
              downloadFilePath);
                return Json(new { Result = true, Url = s3Url });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        [HttpPost]
        [AjaxAwareAuthorize]
        public ActionResult GetTLDSReportS3FileForAdminReporting(string fileName, int profileId)
        {
            //check security
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);
            if (profile == null)
            {
                return Json(new { Result = false, ErrorMessage = "Profile does not exist." });
            }

            var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(profileId);
            var downloadFilePath = tldsS3Settings.GetFormPath(fileName);

            var result = _s3Service.DownloadFile(tldsS3Settings.TLDSBucket, downloadFilePath);

            if (result.IsSuccess)
            {
                var s3Url = _s3Service.GetPublicUrl(tldsS3Settings.TLDSBucket,
              downloadFilePath);
                return Json(new { Result = true, Url = s3Url });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion
        #region Download PDF (section 2,3,4) blank 

        public ActionResult GeneratePDFBlankForm(TLDSReportDataViewModel model)
        {
            try
            {
                //check right
                if (!_parameters.VulnerabilityService.HasRightToAccessTLDSProfile(CurrentUser, model.ProfileId))
                {
                    //
                    return Json(new { IsSuccess = true, Error = "Has no right.", Url = string.Empty });
                }
                string url = string.Empty;
                var dateFormatCookie = System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultDateFormat];
                model.DateFormat = dateFormatCookie == null ? Constanst.DefaultDateFormatValue : dateFormatCookie.Value;

                var blankPDFpdf = PrintBlankPDFForm(model, out url);

                var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(model.ProfileId);
                var fileName = string.Format("draft_{0}.pdf", model.ProfileId);
                fileName = fileName.AddTimestampToFileName();
                var s3FilePath = tldsS3Settings.GetFormPath(fileName);
                _s3Service.UploadRubricFile(tldsS3Settings.TLDSBucket, s3FilePath, new MemoryStream(blankPDFpdf));

                return Json(new { IsSuccess = true, Url = url, fileName = fileName });
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
            }

            return Json(new { IsSuccess = false });
        }
        private byte[] PrintBlankPDFForm(TLDSReportDataViewModel model, out string url)
        {
            byte[] pdf = null;
            url = string.Empty;

            url = Url.Action("BlankPDFForm", "TLDSReport", model, HelperExtensions.GetHTTPProtocal(Request));
            pdf = ExportToPDF(url, model.TimezoneOffset, model.ProfileId);

            return pdf;
        }
        [HttpGet]
        public ActionResult BlankPDFForm(TLDSReportDataViewModel model)
        {
            var defaultDateFormat = model.DateFormat;
            HttpCookie ckDefaultDateFormat = System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultDateFormat];
            if (ckDefaultDateFormat == null)
            {
                ckDefaultDateFormat = new HttpCookie(Constanst.DefaultDateFormat, defaultDateFormat);
                System.Web.HttpContext.Current.Response.Cookies.Add(ckDefaultDateFormat);
            }
            else
            {
                ckDefaultDateFormat.Value = defaultDateFormat;
                System.Web.HttpContext.Current.Response.Cookies.Set(ckDefaultDateFormat);
            }
            var masterModel = new TDLSProfileViewModel()
            {
                ProfileId = model.ProfileId
            };
            //get profile

            BuildGeneralData(masterModel);
            BuildTLDSAdditionalInformation(masterModel);
            BuildTLDSDevelopmentOutcomeProfile(masterModel);
            BuildTLDSEarlyABLESReport(masterModel);
            BuildTLDSOtherReportPlan(masterModel);
            BuildTLDSParentGuardian(masterModel);
            BuildTLDSProfessionalService(masterModel);

            var eyaltProfile = masterModel.DevelopmentOutcomeProfiles.FirstOrDefault(x => x.DevelopmentOutcomeTypeId == 6
                                                                                        && !string.IsNullOrEmpty(x.OriginalFileName)
                                                                                        && !string.IsNullOrEmpty(x.S3FileName));
            if (eyaltProfile != null)
            {
                var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(model.ProfileId);
                var eyaltPdf = _s3Service.DownloadFile(LinkitConfigurationManager.GetS3Settings().TLDSBucket, tldsS3Settings.GetEYALTUploadedPath(model.ProfileId, eyaltProfile.S3FileName));
                if (eyaltPdf.IsSuccess)
                {
                    var stream = new MemoryStream(eyaltPdf.ReturnStream);
                    masterModel.EYALTFileImages.AddRange(PdfHelper.ConvertImage(stream, 300, false));
                }
            }

            return View(masterModel);
        }
        #endregion

        private string ReplaceLineBreak(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            string replaceStr = str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "<br/>");

            return replaceStr;
        }

        public ActionResult GenerateBatchPdfZipFileName()
        {
            var zipFileName = "Batch_Profiles.zip";
            zipFileName = zipFileName.AddTimestampToFileName();
            return Json(new { IsSuccess = true, zipFileName = zipFileName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateBatchPdf(TLDSReportDataViewModel model)
        {
            HttpContext.Server.ScriptTimeout = model.ProfileIdList.Split(',').Length * 30;

            var dateFormatCookie =
                               System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultDateFormat];
            model.DateFormat = dateFormatCookie == null
                ? Constanst.DefaultDateFormatValue
                : dateFormatCookie.Value;

            _parameters.TLDSService.TLDSDownloadQueueInsertOrUpdate(new TLDSDownloadQueue
            {
                FileName = model.ZipFileName,
                ProfileIDs = model.ProfileIdList,
                Status = (int)TLDSDownloadStatus.InProgress,
                CompletedFiles = 0,
                Total = model.ProfileIdList.Split(',').Length,
                CreatedUserID = CurrentUser.Id
            });

            ProcessGenerateZipProfiles(model);
            return Json(new { IsSuccess = true, zipFileName = model.ZipFileName, Url = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        private void ProcessGenerateZipProfiles(TLDSReportDataViewModel model)
        {
            model.ZipFileName = model.ZipFileName.DecodeParameters();
            var logProfileId = 0;

            using (var zipStream = new MemoryStream())
            {
                using (var zipFile = new ZipFile())
                {
                    var profileIdList = model.ProfileIdList.ParseIdsFromString();
                    var studentNameList = _parameters.TLDSService.GetTLDSProfileNameOnly(profileIdList);

                    foreach (var profileId in profileIdList)
                    {
                        try
                        {
                            logProfileId = profileId;

                            var objStudentName = studentNameList.FirstOrDefault(x => x.ProfileId == profileId);
                            var studentName = string.Format("{0} {1}", objStudentName.LastName, objStudentName.FirstName);
                            var fileName = string.Format("{0}-{1}.pdf", studentName, profileId);
                            fileName = fileName.AddTimestampToFileName();
                            if (objStudentName.Status == (int)TLDSProfileStatusEnum.UploadedBySchool || objStudentName.TLDSProfileMetaes.Any(x => x.MetaName == "IsUploadedStatement" && x.MetaValue.ToLower() == "true"))
                            {
                                var meta =
                                    objStudentName.TLDSProfileMetaes.FirstOrDefault(
                                        x => x.MetaName == "UploadedStatementPdfFileName");
                                if (meta != null)
                                {
                                    var tldsS3Setting = TLDSS3Settings.GetTLDSS3Settings(profileId);
                                    var pdfFileName = meta.MetaValue;
                                    string uploadFileName = tldsS3Setting.GetStatementUploadedPath(pdfFileName);
                                    var result = _s3Service.DownloadFile(LinkitConfigurationManager.GetS3Settings().TLDSBucket, uploadFileName);
                                    if (result.IsSuccess)
                                    {
                                        zipFile.AddEntry(fileName, result.ReturnStream);
                                    }
                                }
                            }
                            else
                            {
                                string url = string.Empty;
                                model.ProfileId = profileId;
                                var section1Pdf = Print(model, out url);
                                zipFile.AddEntry(fileName, section1Pdf);
                            }

                            var downloadQueue = _parameters.TLDSService.GetTLDSDownloadQueueByFileName(model.ZipFileName);
                            if (downloadQueue != null)
                            {
                                downloadQueue.CompletedFiles++;
                                _parameters.TLDSService.TLDSDownloadQueueInsertOrUpdate(downloadQueue);
                            }
                        }
                        catch (Exception ex)
                        {
                            PortalAuditManager.LogException(ex);
                            var downloadQueue = _parameters.TLDSService.GetTLDSDownloadQueueByFileName(model.ZipFileName);
                            if (downloadQueue != null)
                            {
                                downloadQueue.CompletedFiles++;
                                if (!string.IsNullOrEmpty(downloadQueue.Errors))
                                {
                                    var listError = JsonConvert.DeserializeObject<List<TLDSDownloadError>>(downloadQueue.Errors);
                                    listError.Add(new TLDSDownloadError { ProfileID = logProfileId, Message = ex.Message });
                                    downloadQueue.Errors = JsonConvert.SerializeObject(listError);
                                }
                                _parameters.TLDSService.TLDSDownloadQueueInsertOrUpdate(downloadQueue);

                            }
                        }
                    }

                    zipFile.Save(zipStream);
                    zipStream.Position = 0;

                    var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(0);
                    var s3FilePath = tldsS3Settings.GetZipBatchReportPath(model.ZipFileName, CurrentUser.UserName);
                    _s3Service.UploadRubricFile(tldsS3Settings.TLDSBucket, s3FilePath, zipStream);
                }
            }
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult GetTLDSZipBatchReportS3File(string zipFileName)
        {
            zipFileName = zipFileName.DecodeParameters();
            var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(0);
            var downloadFilePath = tldsS3Settings.GetZipBatchReportPath(zipFileName, CurrentUser.UserName);

            var result = _s3Service.DownloadFile(tldsS3Settings.TLDSBucket, downloadFilePath);

            if (result.IsSuccess)
            {
                var s3Url = _s3Service.GetPublicUrl(tldsS3Settings.TLDSBucket,
              downloadFilePath);

                var downloadQueue = _parameters.TLDSService.GetTLDSDownloadQueueByFileName(zipFileName);
                if (downloadQueue != null)
                {
                    downloadQueue.Status = (int)TLDSDownloadStatus.Completed;
                    _parameters.TLDSService.TLDSDownloadQueueInsertOrUpdate(downloadQueue);
                }
                return Json(new { Result = true, Url = s3Url });
            }
            else
            {
                var downloadQueue = _parameters.TLDSService.GetTLDSDownloadQueueByFileName(zipFileName);

                if (downloadQueue != null)
                {
                    return Json(new { Result = false, Total = downloadQueue.Total, CompletedFiles = downloadQueue.CompletedFiles });
                }

                return Json(new { Result = false, Total = 0, CompletedFiles = 0 });
            }
        }

        public ActionResult GenerateSummaryReport(TLDSReportDataViewModel model)
        {
            string summaryReportAssignedFileName = "SummaryReportLinked";
            string summaryReportUnAssignedFileName = "SummaryReportUnlinked";
            string summaryReportZipFileName = "SummaryReport";
            if (model.EnrollmentYear.HasValue && model.EnrollmentYear.Value > 0)
            {
                summaryReportAssignedFileName += string.Format("-{0}", model.EnrollmentYear.Value.ToString());
                summaryReportUnAssignedFileName += string.Format("-{0}", model.EnrollmentYear.Value.ToString());
                summaryReportZipFileName += string.Format("-{0}", model.EnrollmentYear.Value.ToString());
            }

            //Add timestamp
            summaryReportAssignedFileName = summaryReportAssignedFileName.AddTimestampToFileName();
            summaryReportUnAssignedFileName = summaryReportUnAssignedFileName.AddTimestampToFileName();
            summaryReportZipFileName = summaryReportZipFileName.AddTimestampToFileName();

            using (var zipStream = new MemoryStream())
            {
                using (var zipFile = new ZipFile())
                {
                    string url = string.Empty;
                    var dateFormatCookie =
                        System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultDateFormat];
                    model.DateFormat = dateFormatCookie == null
                        ? Constanst.DefaultDateFormatValue
                        : dateFormatCookie.Value;

                    var summaryReportAssigned = PrintSummaryReportAssigned(model, out url);
                    var summaryReportUAssigned = PrintSummaryReportUnAssigned(model, out url);
                    summaryReportAssignedFileName = string.Format("{0}.pdf", summaryReportAssignedFileName);
                    summaryReportUnAssignedFileName = string.Format("{0}.pdf", summaryReportUnAssignedFileName);


                    zipFile.AddEntry(
                        summaryReportAssignedFileName,
                        summaryReportAssigned);
                    zipFile.AddEntry(
                        summaryReportUnAssignedFileName,
                        summaryReportUAssigned);

                    zipFile.Save(zipStream);
                    zipStream.Position = 0;
                    summaryReportZipFileName = string.Format("{0}.zip", summaryReportZipFileName);
                    var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(0);
                    var s3FilePath = tldsS3Settings.GetZipSummaryReportPath(summaryReportZipFileName,
                        CurrentUser.UserName);
                    _s3Service.UploadRubricFile(tldsS3Settings.TLDSBucket, s3FilePath, zipStream);

                    return Json(new { IsSuccess = true, zipFileName = summaryReportZipFileName },
                        JsonRequestBehavior.AllowGet);
                }
            }
        }

        private byte[] PrintSummaryReportAssigned(TLDSReportDataViewModel model, out string url)
        {
            model.CurrentUserId = CurrentUser.Id;
            model.CurrentUserDistrictId = CurrentUser.DistrictId;
            url = Url.Action("SummaryReportAssigned", "TLDSReport", model, HelperExtensions.GetHTTPProtocal(Request));
            var pdf = ExportSummaryReportToPDF(url, model.TimezoneOffset);
            return pdf;
        }
        [HttpGet]
        public ActionResult SummaryReportAssigned(TLDSReportDataViewModel model)
        {
            var defaultDateFormat = model.DateFormat;
            HttpCookie ckDefaultDateFormat = System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultDateFormat];
            if (ckDefaultDateFormat == null)
            {
                ckDefaultDateFormat = new HttpCookie(Constanst.DefaultDateFormat, defaultDateFormat);
                System.Web.HttpContext.Current.Response.Cookies.Add(ckDefaultDateFormat);
            }
            else
            {
                ckDefaultDateFormat.Value = defaultDateFormat;
                System.Web.HttpContext.Current.Response.Cookies.Set(ckDefaultDateFormat);
            }
            //Get data for the report
            TLDSFilterParameter p = new TLDSFilterParameter()
            {
                DistrictId = model.CurrentUserDistrictId,
                //ShowArchived = model.ShowArchived,
                ShowArchived = true, // get all
                //EnrollmentYear = model.EnrollmentYear
                EnrollmentYear = 0 //get all 
            };
            var data = _parameters.TLDSService.GetTLDSProfilesForSchoolAdmin(model.CurrentUserId ?? 0, p)
                  .Select(x => new TDLSProfileItemListSchoolAdmin
                  {
                      ProfileId = x.ProfileID,
                      LastName = x.LastName,
                      FirstName = x.FirstName,
                      DateOfBirth = x.DateOfBirth,
                      GenderID = x.GenderID,
                      ECSName = x.ECSName,
                      Section102IsNotRequired = x.Section102IsNotRequired,
                      ECSCompledDate = x.ECSCompledDate,
                      StudentID = x.StudentID,
                      EnrolmentYear = x.EnrolmentYear

                  }).Where(x => x.StudentID != null).ToList();
            //Now sort the data base on the column which user has choosen on UI
            var parser = new DataTableParser<TDLSProfileItemListSchoolAdmin>();//use the same parser when populating the datatable
            data = parser.SortData(data, model.SortingColumns);
            var reportModel = new SummaryReportAssignedModel()
            {
                Data = data
            };
            return View(reportModel);
        }

        private byte[] PrintSummaryReportUnAssigned(TLDSReportDataViewModel model, out string url)
        {
            model.CurrentUserId = CurrentUser.Id;
            model.CurrentUserDistrictId = CurrentUser.DistrictId;
            url = Url.Action("SummaryReportUnAssigned", "TLDSReport", model, HelperExtensions.GetHTTPProtocal(Request));
            var pdf = ExportSummaryReportToPDF(url, model.TimezoneOffset);
            return pdf;
        }
        [HttpGet]
        public ActionResult SummaryReportUnAssigned(TLDSReportDataViewModel model)
        {
            var defaultDateFormat = model.DateFormat;
            HttpCookie ckDefaultDateFormat = System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultDateFormat];
            if (ckDefaultDateFormat == null)
            {
                ckDefaultDateFormat = new HttpCookie(Constanst.DefaultDateFormat, defaultDateFormat);
                System.Web.HttpContext.Current.Response.Cookies.Add(ckDefaultDateFormat);
            }
            else
            {
                ckDefaultDateFormat.Value = defaultDateFormat;
                System.Web.HttpContext.Current.Response.Cookies.Set(ckDefaultDateFormat);
            }
            //Get data for the report
            TLDSFilterParameter p = new TLDSFilterParameter()
            {
                DistrictId = model.CurrentUserDistrictId,
                ShowArchived = model.ShowArchived,
                EnrollmentYear = model.EnrollmentYear
            };

            var data = _parameters.TLDSService.GetTLDSProfilesForSchoolAdmin(model.CurrentUserId ?? 0, p)
                  .Select(x => new TDLSProfileItemListSchoolAdmin
                  {
                      ProfileId = x.ProfileID,
                      LastName = x.LastName,
                      FirstName = x.FirstName,
                      DateOfBirth = x.DateOfBirth,
                      GenderID = x.GenderID,
                      ECSName = x.ECSName,
                      Section102IsNotRequired = x.Section102IsNotRequired,
                      ECSCompledDate = x.ECSCompledDate,
                      StudentID = x.StudentID,
                      EnrolmentYear = x.EnrolmentYear,
                      Status = x.Status
                  }).Where(x => x.StudentID == null).ToList();
            //Now sort the data base on the column which user has choosen on UI
            var parser = new DataTableParser<TDLSProfileItemListSchoolAdmin>();//use the same parser when populating the datatable
            data = parser.SortData(data, model.SortingColumns);
            var reportModel = new SummaryReportUnAssignedModel()
            {
                Data = data
            };
            return View(reportModel);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult GetTLDSZipSummaryReportS3File(string zipFileName)
        {
            zipFileName = zipFileName.DecodeParameters();
            var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(0);
            var downloadFilePath = tldsS3Settings.GetZipSummaryReportPath(zipFileName, CurrentUser.UserName);

            var result = _s3Service.DownloadFile(tldsS3Settings.TLDSBucket, downloadFilePath);

            if (result.IsSuccess)
            {
                var s3Url = _s3Service.GetPublicUrl(tldsS3Settings.TLDSBucket,
              downloadFilePath);
                return Json(new { Result = true, Url = s3Url });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        private byte[] ExportSummaryReportToPDF(string url, int timezoneOffset)
        {
            DateTime dt = DateTime.UtcNow.AddMinutes(timezoneOffset * (-1));
            string footerUrl = Url.Action("RenderFooterSummaryReport", "TLDSReport",
            new { dateTimeNow = dt }, HelperExtensions.GetHTTPProtocal(Request));

            string headerUrl = Url.Action("RenderHeaderSummaryReport", "TLDSReport",
                new { dateTimeNow = dt }, HelperExtensions.GetHTTPProtocal(Request));

            var loopIndex = 0;
            var marginValue = 0;
            byte[] buffer = null;
            while ((buffer == null || buffer.Length == 0) && loopIndex < 6)
            {
                marginValue += 5;
                loopIndex++;

                string args =
                string.Format("--footer-html \"{0}\" --header-html \"{2}\" --header-spacing {3} --footer-spacing {3} \"{1}\" - "
                    , footerUrl
                    , url
                    , headerUrl
                    , marginValue
                    );

                var startInfo = new ProcessStartInfo(Server.MapPath("~/PDFTool/wkhtmltopdf.exe"), args)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                };
                var proc = new Process { StartInfo = startInfo };
                proc.Start();

                string output = proc.StandardOutput.ReadToEnd();
                buffer = proc.StandardOutput.CurrentEncoding.GetBytes(output);
                proc.WaitForExit();
                proc.Close();
            }

            return buffer;
        }
        public ActionResult RenderFooterSummaryReport(DateTime dateTimeNow)
        {
            var footerData = new TLDSSummaryReportFooterViewModel()
            {
                LeftLine1 = "Generated: " + dateTimeNow.DisplayDateWithFormat(true)
            };
            return PartialView("_FooterSummaryReport", footerData);
        }
        public ActionResult RenderHeaderSummaryReport(DateTime dateTimeNow)
        {

            var model = new TLDSSummaryReportHeaderViewModel()
            {

            };
            return PartialView("_HeaderSummaryReport", model);
        }

        public int TestTimeOut(int second, bool extendTimeout = false)
        {
            if (extendTimeout)
            {
                HttpContext.Server.ScriptTimeout = second + 5;
            }

            Thread.Sleep(second * 1000);

            return 1;
        }
    }
}
