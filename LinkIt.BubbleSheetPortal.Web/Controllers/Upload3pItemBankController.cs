using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.DataFileUpload;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Print;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using Ionic.Zip;
using LinkIt.BubbleSheetPortal.Models.DataFileUpload;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class Upload3pItemBankController : BaseController
    {
        private readonly QTI3pSourceService _qTi3pSourceService;
        private readonly DataFileUploadLogService _dataFileUploadLogService;
        private readonly UserService _userService;

        public Upload3pItemBankController(QTI3pSourceService qTI3pSourceService,
                                          DataFileUploadLogService dataFileUploadLogService,
                                          UserService userService)
        {
            _qTi3pSourceService = qTI3pSourceService;
            _dataFileUploadLogService = dataFileUploadLogService;
            _userService = userService;
        }
        
        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignUpload3pItemBank)]
        public ActionResult Index()
        {
            return View("Index");
        }
        
        [HttpGet]
        public ActionResult GetQTI3pSource()
        {
            var qTI3pSource = _qTi3pSourceService.GetAll().Where(x => x.QTI3pSourceId == (int)QTI3pSourceEnum.Progress || x.QTI3pSourceId == (int)QTI3pSourceEnum.Mastery)
                .OrderBy(x => x.QTI3pSourceId).Select(x => new ListItem { Id = x.QTI3pSourceId, Name = Enum.GetName(typeof(QTI3pSourceDisplayEnum), x.QTI3pSourceId) });
            return Json(qTI3pSource, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetUploadFileList(int qTI3pSourceId)
        {
            var uploadFileList = _dataFileUploadLogService.GetDataFileUpload3pLog(qTI3pSourceId).Select(x => new UploadDataFileListViewModel
            {
                DataFileUploadLogID = x.DataFileUploadLogId,
                ImportedDate = x.DateStart,
                ZipFileName = x.FileName,
                UploadUser = _userService.GetUserById(x.CurrentUserId).Name
            });
            var parser = new DataTableParser<UploadDataFileListViewModel>();
            return Json(parser.Parse(uploadFileList, true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowQTI3pItems(int dataFileUploadLogId)
        {
            ViewBag.DataFileUploadLogId = dataFileUploadLogId;
            var datafileuploadlog = _dataFileUploadLogService.GetDataFileUploadLogById(dataFileUploadLogId);
            if (datafileuploadlog != null)
                ViewBag.FileName = datafileuploadlog.FileName;
            return View("ItemListView");
        }

        public ActionResult GetQTI3pItems(int? dataFileUploadLogId)
        {
            if (!dataFileUploadLogId.HasValue)
            {
                dataFileUploadLogId = 0;
            }
            ViewBag.ThirdPartyItemMediaPath = System.Web.HttpUtility.HtmlEncode(ConfigHelper.ThirdPartyItemMediaPath);

            // var useS3Content = _districtDecodeService.UseS3Content(CurrentUser.DistrictId.GetValueOrDefault());
            var dataUploadLogList = _dataFileUploadLogService.GetQTI3pItems(dataFileUploadLogId.Value);

            var uploadItemList = new List<DataFileUploadResourceLogViewModel>();
            var index = 1;
            foreach (var dataUploadlog in dataUploadLogList)
            {
                var data = new DataFileUploadResourceLogViewModel
                {
                    QTI3pItemId = dataUploadlog.QTI3pItemId ?? 0,
                    ResourceFileName = dataUploadlog.ResourceFileName,
                    Content = AdjustXmlContent(dataUploadlog.XmlContent, true),
                    QuestionOrder = index
                };
                index++;

                uploadItemList.Add(data);
            }

            var parser = new DataTableParser<DataFileUploadResourceLogViewModel>();
            return Json(parser.Parse(uploadItemList.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        private string AdjustXmlContent(string xmlContent, bool useS3Content)
        {
            string result = xmlContent.ReplaceWeirdCharacters();
            result = Util.ReplaceTagListByTagOl(result);
            result = ItemSetPrinting.AdjustXmlContentFloatImg(result);
            if (useS3Content)
            {
                result = Util.UpdateS3LinkForItemMedia(result);
                result = Util.UpdateS3LinkForPassageLink(result);
            }
            result = Util.ReplaceVideoTag(result);
            return result;
        }

       
        #region Data File Upload
        [UploadifyPrincipal(Order = 1)]
        [HttpPost]
        public ActionResult UploadDataFile(HttpPostedFileBase postedFile, int? qti3pSourceId)
        {
            if (!qti3pSourceId.HasValue)
            {
                return Json(new { message = "There is no source specified.", success = false, type = "error" },
                          JsonRequestBehavior.AllowGet);
            }
            if (!IsValidPostedFile(postedFile))
            {
                return Json(new { message = "Invalid file, please try again.", success = false, type = "error" },
                            JsonRequestBehavior.AllowGet);
            }
            int dataFileUploadLogId = 0;
            try
            {
                var options = new ReadOptions { StatusMessageWriter = System.Console.Out };
                using (ZipFile zip = ZipFile.Read(postedFile.InputStream, options))
                {
                    try
                    {
                        var dataFileUploadPath = LinkitConfigurationManager.AppSettings.DataFileUploadPath;
                        if (string.IsNullOrEmpty(dataFileUploadPath))
                        {
                            return Json(new { message = "DataFileUploadPath wa not configed. Please contact admin.", success = false, type = "error" },
                           JsonRequestBehavior.AllowGet);
                        }
                        dataFileUploadPath = dataFileUploadPath.Replace("\\", "/");
                        if (dataFileUploadPath[dataFileUploadPath.Length - 1] == '/')
                        {
                            dataFileUploadPath = dataFileUploadPath.Substring(0, dataFileUploadPath.Length - 1);//make sure dataFileUploadPath does not end with '/'
                        }
                        var dataFileUploadPathLocalAppServer = LinkitConfigurationManager.AppSettings.DataFileUploadPathLocalAppServer;
                        if (string.IsNullOrEmpty(dataFileUploadPathLocalAppServer))
                        {
                            return Json(new { message = "DataFileUploadPathLocalAppServer wa not configed. Please contact admin.", success = false, type = "error" },
                           JsonRequestBehavior.AllowGet);
                        }

                        var thirdPartyItemMediaPathLocalAppServer = LinkitConfigurationManager.AppSettings.ThirdPartyItemMediaPathLocalAppServer;
                        if (string.IsNullOrEmpty(thirdPartyItemMediaPathLocalAppServer))
                        {
                            return Json(new { message = "ThirdPartyItemMediaPathLocalAppServer wa not configed. Please contact admin.", success = false, type = "error" },
                           JsonRequestBehavior.AllowGet);
                        }

                        //var timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
                        //var fileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
                        var fileName = postedFile.FileName;
                        var directory = Path.GetDirectoryName(fileName);
                        //Add timestamp to file name
                        fileName = fileName.AddTimestampToFileName();
                        fileName = Path.GetFileNameWithoutExtension(fileName);

                        var tempFolder = string.Format("{0}/tmp/{1}", dataFileUploadPath, fileName);
                        //try to create this temp folder
                        try
                        {
                            Directory.CreateDirectory(tempFolder);

                        }
                        catch
                        {
                            return Json(new { message = "Can not create temporary folder for extracting file, please contact admin.", success = false, type = "error" },
                           JsonRequestBehavior.AllowGet);
                        }

                        try
                        {
                            zip.ExtractAll(tempFolder);
                        }
                        catch
                        {
                            return Json(new { message = "Can not extract file, please try again.", success = false, type = "error" },
                           JsonRequestBehavior.AllowGet);
                        }
                        try
                        {
                            //check if wrong file
                            if (qti3pSourceId.Value == (int)QTI3pSourceEnum.Progress)
                            {
                                if (!System.IO.File.Exists(string.Format("{0}/{1}", tempFolder, "assessment.xml")))
                                {
                                    return
                                        Json(
                                            new
                                            {
                                                message = "The file you are trying to upload is not PROGRESS type. Please try again.",
                                                success = false,
                                                type = "error"
                                            },
                                            JsonRequestBehavior.AllowGet);
                                }
                            }
                           
                           //Build a relative location for saving uploaded item
                            var subFolderPath = DataFileUploader.CreateSubFolderStoring(qti3pSourceId.Value,
                                fileName);
                           
                            var itemPathLocal = string.Empty;
                            //Get itemPath for local saving
                            //if (ConfigHelper.SaveUploadItemToLocal)
                            //{
                            //    var thirdPartyItemMediaPath = ConfigHelper.ThirdPartyItemMediaPath;
                            //    if (string.IsNullOrEmpty(thirdPartyItemMediaPath))
                            //    {
                            //        return Json(new { message = "Can not find config thirdPartyItemMediaPath, please contact Admin.", success = false, type = "error" }, JsonRequestBehavior.AllowGet);
                            //    }
                            //    thirdPartyItemMediaPath = thirdPartyItemMediaPath.Replace("\\", "/");
                            //    itemPath = string.Format("{0}/{1}", thirdPartyItemMediaPath.RemoveEndSlash(), subFolderPath.RemoveStartSlash());
                                itemPathLocal = string.Format("{0}\\{1}",
                                    thirdPartyItemMediaPathLocalAppServer.RemoveEndSlash(),
                                    subFolderPath.RemoveStartSlash().Replace("/", "\\"));
                            //    itemPath = itemPath.Replace("//", "/");
                            //    try
                            //    {
                            //        if (!Directory.Exists(itemPath))
                            //        {
                            //            Directory.CreateDirectory(itemPath);
                            //        }
                            //    }
                            //    catch
                            //    {
                            //        return Json(new { message = "Can not create folder , please contact Admin", success = false, type = "error" }, JsonRequestBehavior.AllowGet);
                            //    }
                            //}
                            //S3 absolute link
                            var AUVirtualTestFolder = LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder;
                            
                          
                            if (AUVirtualTestFolder == null)
                            {
                                return Json(new { message = "Can not find config AUVirtualTestFolder, please contact Admin.", success = false, type = "error" }, JsonRequestBehavior.AllowGet);
                            }

                            //Now storing media on S3 only
                            tempFolder = string.Format("{0}\\tmp\\{1}", dataFileUploadPathLocalAppServer.RemoveEndSlash(), fileName);
                            //add to queue
                            var dataFileUploadLog = new DataFileUploadLog()
                            {
                                CurrentUserId = CurrentUser.Id,
                                ExtractedFoler = tempFolder,
                                //ItemSetPath = itemPath,
                                ItemSetPath = itemPathLocal,
                                //QtiGroupId = parameter.QtiGroupId,
                                FileName = postedFile.FileName,
                                DateStart = DateTime.UtcNow,
                                DataFileUploadTypeId = 0,
                                DateEnd = DateTime.UtcNow,
                                QTI3pSourceId = qti3pSourceId ?? 0,
                                Status = (int)DataFileUploadProcessingEnum.NotProcess // not yet processing
                                
                            };
                            _dataFileUploadLogService.CreateDataFileUploadLog(dataFileUploadLog);

                            dataFileUploadLogId = dataFileUploadLog.DataFileUploadLogId;                            

                        }
                        catch (Exception ex)
                        {
                            PortalAuditManager.LogException(ex);
                            return
                                Json(
                                    new
                                    {
                                        message = "There was some errors happened, please try again.",
                                        success = false,
                                        type = "error"
                                    },
                                    JsonRequestBehavior.AllowGet);
                        }

                    }
                    catch
                    {
                        return Json(new { message = "There was some errors happened, please try again.", success = false, type = "error" },
                           JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch
            {
                return Json(new { message = "Can not read file, please try again.", success = false, type = "error" },
                            JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, dataFileUploadLogId = dataFileUploadLogId, JsonRequestBehavior.AllowGet });
        }
        [HttpGet]
        public ActionResult CheckStatusUploadDataFile(int dataFileUploadLogId)
        {
            var dataFileUploadLog = _dataFileUploadLogService.GetDataFileUploadLogById(dataFileUploadLogId);
            if(dataFileUploadLog != null)
            {
                if(dataFileUploadLog.Status == (int)DataFileUploadProcessingEnum.Finish)
                {
                    var resourceLogSuccess = _dataFileUploadLogService.GetQTI3pItems(dataFileUploadLogId);
                    var fileNameFails = _dataFileUploadLogService.Get3PFilesFail(dataFileUploadLogId).Select(x=>x.ResourceFileName);
                    return Json(new { success = true, processingStatus = "finish", fileSuccessCount = resourceLogSuccess.Count(), fileFails = fileNameFails }, JsonRequestBehavior.AllowGet);
                }
                else if (dataFileUploadLog.Status == (int)DataFileUploadProcessingEnum.Error)
                {
                    return Json(new { success = false, message = dataFileUploadLog.Result}, JsonRequestBehavior.AllowGet);
                }                
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        private bool IsValidPostedFile(HttpPostedFileBase file)
        {
            if (file.IsNull())
            {
                return false;
            }
            return !string.IsNullOrEmpty(file.FileName) && file.InputStream.IsNotNull();
        }

       
        #endregion Data File Upload

    }
}
