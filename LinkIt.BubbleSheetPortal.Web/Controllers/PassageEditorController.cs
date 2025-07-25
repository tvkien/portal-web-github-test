using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

//using System.Web.Http;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using System.Web.Script.Serialization;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.ViewModels.PassageEditor;
using S3Library;
using LinkIt.BubbleSheetPortal.Models.DTOs.QTIRefObject;
using LinkIt.BubbleSheetPortal.Web.Models;
using DevExpress.Utils.OAuth;
using LinkIt.BubbleSheetPortal.Models.DTOs.PassageEditor;
using DevExpress.XtraReports.Templates;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestMaker;
using LinkIt.BubbleSheetPortal.Web.Print;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class PassageEditorController : BaseController
    {
        private readonly IS3Service _s3Service;
        private readonly PassageEditorControllerParameters parameters;

        public PassageEditorController(PassageEditorControllerParameters parameters, IS3Service s3Service)
        {
            this.parameters = parameters;
            _s3Service = s3Service;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignPassageNew)]
        public ActionResult Index(string nameSearch, int? gradeId, string subject, int? textTypeId, int? textSubTypeId, int? fleschKincaidId, string searchBox, int? passageNumber)
        {
            ViewBag.NameSearch = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(nameSearch));
            ViewBag.GradeId = gradeId ?? 0;
            ViewBag.Subject = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(subject));
            ViewBag.TextTypeId = textTypeId ?? 0;
            ViewBag.TextSubTypeId = textSubTypeId ?? 0;
            ViewBag.FleschKincaidId = fleschKincaidId ?? 0;
            ViewBag.SearchBox = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(searchBox));
            ViewBag.PassageNumberSearch = passageNumber;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.StateId = CurrentUser.StateId;
            ViewBag.DistrictId = CurrentUser.DistrictId;
            ViewBag.HasPermission = true;
            return View();
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignPassageNew)]
        public ActionResult Create(bool? fromItemSetEditor)
        {
            PassageCreateViewModel model = new PassageCreateViewModel();
            model.FromPassageEditor = true;
            if (fromItemSetEditor.HasValue && fromItemSetEditor == true)
            {
                model.FromItemSetEditor = true;
            }
            return View(model);
        }

        public ActionResult GetPassageList(GetQtiRefObjectFilterRequest request)
        {
            var filter = MappingRequest(request);

            // Filter only assigned passages
            if (!string.IsNullOrEmpty(request.QtiItemIdString))
            {
                var qtiItemIds = request.QtiItemIdString.ToIntArray();
                var refObjectIds = qtiItemIds.Count() > 0 ? parameters.QtiItemRefObjectService.GetRefObjectIdsByQtiItemIds(string.Join(",", qtiItemIds)) : new List<int>();

                // Only load the assigned passages otherwise empty (refObjectIds.Count() == 0)
                if(!refObjectIds.Any())
                {
                    var dataNotFound = new List<QtiRefObjectListModel>().AsQueryable();
                    var parserNotFound = new DataTableParser<QtiRefObjectListModel>();
                    return Json(parserNotFound.Parse(dataNotFound, true), JsonRequestBehavior.AllowGet);
                }

                filter.QTIRefObjectIDs = string.Join(",", refObjectIds);
            }

            var data = parameters.QtiRefObjectService.GetQtiRefObject(filter);

            var parser = new DataTableParser<QtiRefObjectListModel>();
            var totalRecord = data.FirstOrDefault()?.TotalCount ?? 0;
            var pagedResults = data.Select(x => new QtiRefObjectListModel
            {
                FleschKincaid = x.FleschKincaidID,
                Grade = x.GradeName,
                Name = Server.HtmlEncode(x.Name),
                QTIRefObjectFileRef = x.QTIRefObjectFileRef,
                Subject = Server.HtmlEncode(x.Subject),
                TextSubType = Server.HtmlEncode(x.TextSubType),
                TextType = Server.HtmlEncode(x.TextType),
                PassageId = x.QTIRefObjectID,
                FleschKinkaidName = Server.HtmlEncode(x.FleschKinkaidName ?? string.Empty),
                GradeOrder = x.GradeOrder ?? 0
            }).AsQueryable();

            return Json(parser.Parse(pagedResults, totalRecord), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult Create(EditPassageViewModel model)
        {
            model.UserId = CurrentUser.Id;
            model.TypeId = 1;
            model.SetValidator(parameters.EditPassageViewModelValidator);

            if (!model.IsValid)
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }
            try
            {
                var qtiRefObject = CreateNewPassage(model);
                var s3Result = UploadPassageJsonFileToS3(qtiRefObject.QTIRefObjectID, model.XmlContentPassage, model.TextTypeId, model.Name);

                var s3ResultUploadXml = UploadPassageXmlFileToS3(qtiRefObject.QTIRefObjectID, model.XmlContentPassage);

                return Json(new { Success = true, qtiRefObjectId = qtiRefObject.QTIRefObjectID });
            }
            catch (Exception e)
            {
                return ShowJsonResultException(model, e.Message);
            }
        }

        private QtiRefObject CreateNewPassage(EditPassageViewModel model)
        {
            var qtiRefObject = new QtiRefObject
            {
                FleschKincaidID = model.FleschKincaidId,
                GradeID = model.GradeId,
                Name = model.Name,
                OldMasterCode = model.OldMasterCode,
                QTIRefObjectFileRef = model.QtiRefObjectFileRef,
                QTIRefObjectID = model.QtiRefObjectId,
                Subject = model.SubjectText ?? string.Empty,
                TextSubTypeID = model.TextSubTypeId,
                TextTypeID = model.TextTypeId,
                TypeID = model.TypeId,
                UserID = model.UserId,
                CreatedDate = DateTime.UtcNow,
            };

            parameters.QtiRefObjectService.Save(qtiRefObject);

            return qtiRefObject;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignPassageNew)]
        public ActionResult Edit(int id, string nameSearch, int? gradeId, string subject, int? textTypeId, int? textSubTypeId, int? fleschKincaidId, string searchBox, bool? alreadyCreated, int? passageNumber, int? districtId)
        {
            if (districtId == null)
                districtId = CurrentUser.DistrictId.GetValueOrDefault();

            if (!CurrentUser.IsPublisher && !parameters.QtiRefObjectService.HasRightToEdit(CurrentUser, id))
            {
                ViewBag.HasPermission = false;
                return RedirectToAction("Index");
            }
            
            nameSearch = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(nameSearch));
            subject = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(subject));
            searchBox = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(searchBox));

            var model = BuildEditPassageViewModel(id, nameSearch, gradeId, subject, textTypeId, textSubTypeId,
                fleschKincaidId, searchBox, alreadyCreated, passageNumber);
            if (model == null)
            {
                return RedirectToAction("Index");
            }

            model.MediaModel = new MediaModel
            {
                ID = id,
                UseS3Content = true // alsway use content from S3
                                    // parameters.DistrictDecodeService.UseS3Content(CurrentUser.DistrictId.GetValueOrDefault())
            };
            model.FromPassageEditor = true;
            var mediaModel = new MediaModel();
            model.XmlContentPassage = PassageUtil.UpdateS3LinkForPassageMedia(model.XmlContentPassage, mediaModel.S3Domain, mediaModel.UpLoadBucketName, mediaModel.AUVirtualTestFolder);

            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult Edit(EditPassageViewModel model)
        {
            if (!parameters.QtiRefObjectService.HasRightToEdit(CurrentUser, model.QtiRefObjectId))
            {
                return Json(new
                {
                    success = "false",
                    message = "Has no right to update this passage."
                });
            }

            // Remove unhandle control characters
            model.XmlContentPassage = model.XmlContentPassage.RemoveTroublesomeCharacters();
            model.XmlContentPassage = model.XmlContentPassage.RemoveZeroWidthSpaceCharacterFromUnicodeString();

            model.SetValidator(parameters.EditPassageViewModelValidator);

            if (!model.IsValid)
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }
            try
            {
                SavePassageHistory(model.QtiRefObjectId, newXmlContentPassage: model.XmlContentPassage ?? string.Empty);
                var qtiRefObject = EditPassage(model);

                //UpdatePassageXmlContent(qtiRefObject.QTIRefObjectID, model.XmlContentPassage); // no store on web server more
                //if (Util.UploadTestItemMediaToS3) //it's now alway upload to S3, no web server more
                {
                    var s3Result = UploadPassageJsonFileToS3(qtiRefObject.QTIRefObjectID, model.XmlContentPassage, model.TextTypeId, model.Name);
                    if (!s3Result.IsSuccess)
                    {
                        return Json(new
                        {
                            success = "false",
                            message = "Update json file to S3 fail: " + s3Result.ErrorMessage
                        });
                    }
                }
            }
            catch (Exception e)
            {
                return ShowJsonResultException(model, e.Message);
            }
            //Upload Passage xml content to file RO_XXX.xml to S3
            try
            {
                var qtiRefObject = EditPassage(model);

                var s3Result = UploadPassageXmlFileToS3(qtiRefObject.QTIRefObjectID, model.XmlContentPassage);
                if (!s3Result.IsSuccess)
                {
                    return Json(new
                    {
                        success = "false",
                        message = "Update passage content file to S3 fail: " + s3Result.ErrorMessage
                    });
                }

                return Json(new { Success = true, qtiRefObjectId = qtiRefObject.QTIRefObjectID });
            }
            catch (Exception e)
            {
                return ShowJsonResultException(model, e.Message);
            }
        }

        [HttpGet]
        public ActionResult GetMostRecentPassageVersions(int qtiRefObjectId, int numberOfVersions = 10)
        {
            var versions = new List<GetMostRecentPassageVersionsDto>();

            var qtiRefObject = parameters.QtiRefObjectService.GetById(qtiRefObjectId);
            if (qtiRefObject != null)
            {
                var xmlContentPassage = GetPassageXmlContent(qtiRefObjectId, out bool fileNotFound);
                if (fileNotFound)
                {
                    xmlContentPassage = string.Empty;
                }

                var authorId = qtiRefObject.UpdatedByUserID ?? qtiRefObject.UserID;
                var user = parameters.UserService.GetUserById(authorId);
                var fullName = user != null ? user.LastName + ", " + user.FirstName : null;

                var qtiRefObjectHistories = parameters.QtiRefObjectHistoryService.GetListByQtiRefObjectId(qtiRefObjectId, numberOfVersions - 1);

                QtiRefObjectHistory revertedQtiRefObjectHistory = null;
                if (qtiRefObject.RevertedFromQTIRefObjectHistoryID.HasValue)
                {
                    revertedQtiRefObjectHistory = qtiRefObjectHistories.FirstOrDefault(x => x.QTIRefObjectHistoryId == qtiRefObject.RevertedFromQTIRefObjectHistoryID.Value);
                    if (revertedQtiRefObjectHistory == null)
                    {
                        revertedQtiRefObjectHistory = parameters.QtiRefObjectHistoryService.GetById(qtiRefObject.RevertedFromQTIRefObjectHistoryID.Value);
                    }
                }

                var timeZoneId = parameters.StateService.GetTimeZoneId(CurrentUser.StateId ?? 0);

                versions.Add(new GetMostRecentPassageVersionsDto
                {
                    QTIRefObjectHistoryId = 0,
                    QTIRefObjectId = qtiRefObjectId,
                    ChangedDate = (qtiRefObject.UpdatedDate ?? qtiRefObject.CreatedDate).ConvertTimeFromUtc(timeZoneId).ToString("s"),
                    XmlContent = xmlContentPassage,
                    AuthorId = authorId,
                    AuthorFullName = fullName,
                    RevertedFromDate = revertedQtiRefObjectHistory?.ChangedDate.ConvertTimeFromUtc(timeZoneId).ToString("s")
                });

                if (qtiRefObjectHistories.Any())
                {
                    var authorIds = qtiRefObjectHistories.Select(x => x.AuthorId).Distinct().ToList();
                    var authors = parameters.UserService.GetUsersByIds(authorIds)
                        .Select(x => new
                        {
                            x.Id,
                            FullName = x.LastName + ", " + x.FirstName
                        }).ToArray();

                    versions.AddRange(
                        from r in qtiRefObjectHistories
                        join u in authors on r.AuthorId equals u.Id into userGroup
                        from u in userGroup.DefaultIfEmpty()
                        select new GetMostRecentPassageVersionsDto
                        {
                            QTIRefObjectHistoryId = r.QTIRefObjectHistoryId,
                            QTIRefObjectId = r.QTIRefObjectId,
                            ChangedDate = r.ChangedDate.ConvertTimeFromUtc(timeZoneId).ToString("s"),
                            XmlContent = r.XmlContent,
                            AuthorId = r.AuthorId,
                            AuthorFullName = u?.FullName,
                        });
                }

                foreach (var version in versions)
                {
                    var mediaModel = new MediaModel();
                    version.XmlContent = PassageUtil.UpdateS3LinkForPassageMedia(version.XmlContent, mediaModel.S3Domain, mediaModel.UpLoadBucketName, mediaModel.AUVirtualTestFolder);
                }
            }
            
            return Json(new { versions }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RevertPassage(RevertPassageRequest model)
        {
            var qtiRefObjectHistory = parameters.QtiRefObjectHistoryService.GetById(model.QtiRefObjectHistoryId);
            if (qtiRefObjectHistory == null || qtiRefObjectHistory.QTIRefObjectId != model.QtiRefObjectId)
            {
                return Json(new
                {
                    success = false,
                    message = $"QtiRefObjectHistoryId: {model.QtiRefObjectHistoryId} doesn't exist"
                });
            }

            var qtiRefObject = parameters.QtiRefObjectService.GetById(model.QtiRefObjectId);
            SavePassageHistory(model.QtiRefObjectId, qtiRefObject);

            qtiRefObject.UpdatedByUserID = CurrentUser.Id;
            qtiRefObject.UpdatedDate = DateTime.UtcNow;
            qtiRefObject.RevertedFromQTIRefObjectHistoryID = model.QtiRefObjectHistoryId;
            parameters.QtiRefObjectService.Save(qtiRefObject);

            var s3Result = UploadPassageJsonFileToS3(qtiRefObject.QTIRefObjectID, qtiRefObjectHistory.XmlContent, qtiRefObject.TextTypeID, qtiRefObject.Name);
            if (!s3Result.IsSuccess)
            {
                return Json(new
                {
                    success = false,
                    message = "Update json file to S3 fail: " + s3Result.ErrorMessage
                });
            }

            s3Result = UploadPassageXmlFileToS3(model.QtiRefObjectId, qtiRefObjectHistory.XmlContent);
            if (!s3Result.IsSuccess)
            {
                return Json(new
                {
                    success = false,
                    message = "Update passage content file to S3 fail: " + s3Result.ErrorMessage
                });
            }

            return Json(new { success = true, revertedXmlContent = qtiRefObjectHistory.XmlContent });
        }

        private S3Result UploadPassageJsonFileToS3(int qTIRefObjectID, string xmlContent, int? textType, string name)
        {
            string textTypeString = string.Empty;
            if (textType.HasValue)
            {
                var textTypeObject = parameters.QTI3pTextTypeService.GetAll().Where(x => x.TextTypeID == textType.Value).FirstOrDefault();
                if (textTypeObject != null)
                {
                    textTypeString = textTypeObject.Name;
                }
            }
            var obj = new PassageJsonModel { textType = textTypeString, titlePassage = name, content = xmlContent };
            var s3json = new JavaScriptSerializer();
            s3json.MaxJsonLength = int.MaxValue;
            var s3jsonObject = s3json.Serialize(obj);
            //var s3json = new JavaScriptSerializer().Serialize(obj);
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(s3jsonObject ?? ""));

            var folder = LinkitConfigurationManager.GetS3Settings().AUVirtualTestROFolder;
            var bucketName = LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName;
            //return s3Service.UploadRubricFile(bucketName, folder + "/RO/RO_" + qTIRefObjectID + ".json", fileStream);
            return _s3Service.UploadRubricFile(bucketName, folder + "RO/RO_" + qTIRefObjectID + ".json", fileStream, false);
        }

        private string GetPassageXmlContent(int qtiRefObjectId, out bool fileNotFound)
        {
            fileNotFound = false;
            try
            {
                //now get the content from S3 always
                var model = new MediaModel();
                var xmlContent =  PassageUtil.GetS3PassageContent(_s3Service, qtiRefObjectId, model.UpLoadBucketName, model.AUVirtualTestROFolder, out fileNotFound);
                xmlContent = XmlUtils.SanitizeAmpersands(xmlContent);
                xmlContent = ItemSetPrinting.AdjustXmlContentFloatImg(xmlContent);
                xmlContent = Util.ReplaceTagListByTagOlForPassage(xmlContent, false);
                return xmlContent;
            }
            catch (Exception)
            {
                return "";
            }

            return "";
        }

        private QtiRefObject EditPassage(EditPassageViewModel model)
        {
            var qtiRefObject = new QtiRefObject
            {
                FleschKincaidID = model.FleschKincaidId,
                GradeID = model.GradeId,
                Name = model.Name,
                OldMasterCode = model.OldMasterCode,
                QTIRefObjectFileRef = model.QtiRefObjectFileRef,
                QTIRefObjectID = model.QtiRefObjectId,
                Subject = model.SubjectText ?? string.Empty,
                TextSubTypeID = model.TextSubTypeId,
                TextTypeID = model.TextTypeId,
                TypeID = model.TypeId,
                UserID = model.UserId,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                UpdatedByUserID = CurrentUser.Id,
            };

            parameters.QtiRefObjectService.Save(qtiRefObject);
            return qtiRefObject;
        }

        private void SavePassageHistory(int qtiRefObjectId, QtiRefObject qtiRefObject = null, string newXmlContentPassage = null)
        {
            if (qtiRefObject == null)
            {
                qtiRefObject = parameters.QtiRefObjectService.GetById(qtiRefObjectId);
            }

            var xmlContentPassage = GetPassageXmlContent(qtiRefObjectId, out bool fileNotFound);
            if (fileNotFound)
            {
                xmlContentPassage = string.Empty;
            }
            
            if (newXmlContentPassage != null && xmlContentPassage == newXmlContentPassage)
            {
                return;
            }

            var qtiRefObjectHistory = new QtiRefObjectHistory
            {
                QTIRefObjectId = qtiRefObject.QTIRefObjectID,
                ChangedDate = qtiRefObject.UpdatedDate ?? qtiRefObject.CreatedDate,
                XmlContent = xmlContentPassage,
                AuthorId = qtiRefObject.UpdatedByUserID ?? qtiRefObject.UserID,
            };

            parameters.QtiRefObjectHistoryService.Save(qtiRefObjectHistory);
        }

        [HttpPost]
        public ActionResult AudioUpload(int id, HttpPostedFileBase file)
        {
            var model = new MediaModel
            {
                ID = id,
                PostedFile = file,
                MediaType = MediaType.Audio
            };

            var result = MediaHelper.UploadPassageMedia(model, _s3Service);
            //build new absolute url for preview on popup
            string absoluteUrl = string.Empty;
            if (string.IsNullOrEmpty(model.AUVirtualTestFolder))
            {
                absoluteUrl = string.Format("{0}/{1}", UrlUtil.GenerateS3Subdomain(model.S3Domain, model.UpLoadBucketName).RemoveEndSlash(), result.MediaPath.RemoveStartSlash());
            }
            else
            {
                absoluteUrl = string.Format("{0}/{1}/{2}", UrlUtil.GenerateS3Subdomain(model.S3Domain, model.UpLoadBucketName).RemoveEndSlash(), model.AUVirtualTestFolder.RemoveEndSlash().RemoveStartSlash(), result.MediaPath.RemoveStartSlash());
            }

            var jsonResult = Json(new { success = result.Success, url = absoluteUrl, absoluteUrl = absoluteUrl, errorMessage = result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            var jsonStringResult = new JavaScriptSerializer().Serialize(jsonResult.Data);

            return Content(jsonStringResult, "text/plain");
        }

        [HttpPost]
        public ActionResult ImageUpload(int id, HttpPostedFileBase file)
        {
            var model = new MediaModel
            {
                ID = id,
                PostedFile = file,
                MediaType = MediaType.Image
            };

            var result = MediaHelper.UploadPassageMedia(model, _s3Service);
            //build new absolute url for preview on popup
            string absoluteUrl = string.Empty;
            if (string.IsNullOrEmpty(model.AUVirtualTestROFolder.RemoveEndSlash()))
            {
                absoluteUrl = string.Format("{0}/{1}", UrlUtil.GenerateS3Subdomain(model.S3Domain, model.UpLoadBucketName).RemoveEndSlash(), result.MediaPath.RemoveStartSlash());
            }
            else
            {
                absoluteUrl = string.Format("{0}/{1}/{2}", UrlUtil.GenerateS3Subdomain(model.S3Domain, model.UpLoadBucketName).RemoveEndSlash(), model.AUVirtualTestROFolder.RemoveEndSlash().RemoveStartSlash(), result.MediaPath.RemoveStartSlash());
            }

            var jsonResult = Json(new { success = result.Success, url = absoluteUrl, absoluteUrl = absoluteUrl, errorMessage = result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            var jsonStringResult = new JavaScriptSerializer().Serialize(jsonResult.Data);

            return Content(jsonStringResult, "text/plain");
        }

        [HttpPost]
        public ActionResult VideoUpload(int id, HttpPostedFileBase file)
        {
            var model = new MediaModel
            {
                ID = id,
                PostedFile = file,
                MediaType = MediaType.Video
            };

            var result = MediaHelper.UploadPassageMedia(model, _s3Service);
            var mediaPath = result.MediaPath ?? string.Empty;
            mediaPath = result.MediaPath.Replace(model.UpLoadBucketName + "/", "");
            if (mediaPath.StartsWith("https://"))
            {
                mediaPath = mediaPath.Replace("https://", "https://" + model.UpLoadBucketName + ".");
            }
            if (mediaPath.StartsWith("http://"))
            {
                mediaPath = mediaPath.Replace("http://", "http://" + model.UpLoadBucketName + ".");
            }
            var jsonResult = Json(new { success = result.Success, ReturnValue = mediaPath, errorMessage = result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            var jsonStringResult = new JavaScriptSerializer().Serialize(jsonResult.Data);

            return Content(jsonStringResult, "text/plain");
        }

        private S3Result UploadFileToS3(int id, HttpPostedFileBase file, string fileName)
        {
            var folder = LinkitConfigurationManager.GetS3Settings().AUVirtualTestROFolder;
            var bucketName = LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName;

            //string itemSetPath = "/RO/RO_" + id.ToString() + "_media";
            string itemSetPath = "RO/RO_" + id.ToString() + "_media";
            //var fileName = Path.GetFileName(file.FileName);
            //fileName = ServiceUtil.AddTimestampToFileName(fileName);
            var fullFileName = itemSetPath + "/" + fileName;

            return _s3Service.UploadRubricFile(bucketName, folder + fullFileName, file.InputStream);
        }

        public ActionResult GetAudio(string id)
        {
            var testItemMediaPath = string.Empty;
            if (string.IsNullOrWhiteSpace(testItemMediaPath))
            {
                return new FileContentResult(new byte[0], "audio/mpeg");
            }

            var roFilePath = Path.Combine(testItemMediaPath, id.Replace("|", "\\"));
            if (!System.IO.File.Exists(roFilePath))
            {
                return new FileContentResult(new byte[0], "audio/mpeg");
            }

            byte[] file = System.IO.File.ReadAllBytes(roFilePath);
            Response.ContentType = "audio/mpeg";
            return File(file, "audio/mpeg");
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult Delete(int qtiRefObjectId)
        {
            if (
               !parameters.QtiRefObjectService.HasRightToEdit(CurrentUser, qtiRefObjectId))
            {
                return
                    Json(
                        new
                        {
                            Success = "false",
                            message = "Has no right to update this passage."
                        });
            }
            try
            {
                parameters.QtiRefObjectService.Delete(qtiRefObjectId);
                return Json(new { Success = true });
            }
            catch (Exception e)
            {
                return Json(new { Success = false });
            }
        }

        [HttpGet]
        public ActionResult GetAvailableGrades()
        {
            var data = parameters.GradeService.GetGrades().Select(x => new ListItem { Id = x.Id, Name = x.Name }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAssignedGradesForPassages(bool isIncludeQti3p = false)
        {
            var data =
                parameters.QtiRefObjectService.GetAssignedGradesForPassages(isIncludeQti3p).Select(x => new ListItem { Id = x.Id, Name = x.Name }).
                    ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadCreatePassageForm(bool? fromItemSetEditor, bool? fromItemEditor, int? qtiItemGroupId)
        {
            PassageCreateViewModel model = new PassageCreateViewModel();
            if (fromItemSetEditor.HasValue && fromItemSetEditor == true)
            {
                model.FromItemSetEditor = true;
            }
            if (fromItemEditor.HasValue && fromItemEditor == true)
            {
                model.FromItemEditor = true;
                model.QtiItemGroupId = qtiItemGroupId ?? 0;
            }
            return PartialView("_Create", model);
        }

        public ActionResult LoadCreatePassageDiv(bool? fromItemSetEditor, bool? fromItemEditor, int? qtiItemGroupId)
        {
            PassageCreateViewModel model = new PassageCreateViewModel();
            if (fromItemSetEditor.HasValue && fromItemSetEditor == true)
            {
                model.FromItemSetEditor = true;
            }
            if (fromItemEditor.HasValue && fromItemEditor == true)
            {
                model.FromItemEditor = true;
                model.QtiItemGroupId = qtiItemGroupId ?? 0;
            }
            return PartialView("_PassageCreateDiv", model);
        }

        private EditPassageViewModel BuildEditPassageViewModel(int id, string nameSearch, int? gradeId, string subject,
            int? textTypeId, int? textSubTypeId, int? fleschKincaidId, string searchBox, bool? alreadyCreated, int? passageNumber)
        {
            var qtiRefObject = parameters.QtiRefObjectService.GetById(id);
            //Check permission
            if (qtiRefObject != null)
            {
                var model = new EditPassageViewModel
                {
                    FleschKincaidId = qtiRefObject.FleschKincaidID,
                    GradeId = qtiRefObject.GradeID,
                    Name = qtiRefObject.Name,
                    OldMasterCode = qtiRefObject.OldMasterCode,
                    QtiRefObjectFileRef = qtiRefObject.QTIRefObjectFileRef,
                    QtiRefObjectId = qtiRefObject.QTIRefObjectID,
                    SubjectText = qtiRefObject.Subject ?? string.Empty,
                    TextSubTypeId = qtiRefObject.TextSubTypeID,
                    TextTypeId = qtiRefObject.TextTypeID,
                    TypeId = qtiRefObject.TypeID,
                    UserId = qtiRefObject.UserID,
                    //Remeber filter
                    NameFilter = string.IsNullOrEmpty(nameSearch) ? string.Empty : HttpUtility.UrlDecode(nameSearch),
                    GradeIdFilter = gradeId ?? 0,
                    SubjectFilter = string.IsNullOrEmpty(subject) ? string.Empty : HttpUtility.UrlDecode(subject),
                    TextTypeIdFilter = textTypeId ?? 0,
                    TextSubTypeIdFilter = textSubTypeId ?? 0,
                    FleschKincaidIdFilter = fleschKincaidId ?? 0,
                    SearchBoxFilter = string.IsNullOrEmpty(searchBox) ? string.Empty : HttpUtility.UrlDecode(searchBox),
                    FileNotFound = false,
                    PassageNumber = passageNumber
                };
                bool fileNotFound = false;
                model.XmlContentPassage = GetPassageXmlContent(model.QtiRefObjectId, out fileNotFound);
                if (fileNotFound)
                {
                    model.FileName = HttpUtility.UrlEncode(string.Format("RO_{0}.xml", id));
                    model.XmlContentPassage = string.Empty;
                }
                if (alreadyCreated == null) //not check in the first time create -> Edit
                {
                    model.FileNotFound = fileNotFound;
                }

                model.XmlContentPassage = Util.ReplaceVideoTag(model.XmlContentPassage);
                return model;
            }
            else
            {
                return null;
            }
        }

        public ActionResult LoadEditPassageForm(int id, bool? firstTime, string from, int? qtiItemGroupId)
        {
            EditPassageViewModel model = new EditPassageViewModel();
            model = BuildEditPassageViewModel(id, null, null, null, null, null, null, null, null, null);
            if (firstTime.HasValue && firstTime == true)
            {
                model.FileNotFound = false;
            }
            if (from == null)
            {
                from = string.Empty;
            }
            if (from.Equals("fromItemSetEditor"))
            {
                model.FromItemSetEditor = true;
            }
            if (from.Equals("fromItemEditor"))
            {
                model.FromItemEditor = true;
                model.QtiItemGroupId = qtiItemGroupId.GetValueOrDefault();
            }

            return PartialView("_Edit", model);
        }

        public ActionResult LoadEditPassageDiv(int id, bool? firstTime, string from, int? qtiItemGroupId, string qtiItemIdsAssignPassage, int? virtualTestId = 0)
        {
            var model = new EditPassageViewModel();

            model = BuildEditPassageViewModel(id, null, null, null, null, null, null, null, null, null);

            model.VirtualTestId = virtualTestId;
            if (firstTime.HasValue && firstTime == true)
            {
                model.FileNotFound = false;
            }
            if (from == null)
            {
                from = string.Empty;
            }
            if (from.Equals("fromItemSetEditor"))
            {
                model.FromItemSetEditor = true;
            }
            if (from.Equals("fromItemEditor"))
            {
                model.FromItemEditor = true;
                model.QtiItemGroupId = qtiItemGroupId.GetValueOrDefault();
            }

            model.MediaModel = new MediaModel
            {
                ID = id,
                UseS3Content = true
            };
            if (from.Equals("fromItemEditorPopup"))
            {
                return PartialView("_PassageEditPopup", model);
            }

            model.QtiItemIdsAssignPassage = qtiItemIdsAssignPassage;

            return PartialView("_PassageEditDiv", model);
        }

        public ActionResult LoadAlertPassageName()
        {
            return PartialView("_PassageAlert");
        }

        public ActionResult LoadConfirmDeletePassage(int id, string passageName)
        {
            var qtiItemIds = parameters.QtiItemRefObjectService.GetAll().Where(x => x.QtiRefObjectId == id).Select(x => x.QtiItemId).ToList();
            var qtiItemIdsNullable = qtiItemIds.Cast<int?>().ToList();
            var virtualTests = parameters.VirtualTestService.GetVirtualTestsByQtiItems(qtiItemIdsNullable);

            passageName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(passageName));
            var obj = new PassageDeleteViewModel
            {
                Id = id,
                Name = passageName,
                VirtualTests = virtualTests
            };
            return PartialView("_ConfirmDeletePassage", obj);
        }

        [HttpGet]
        public ActionResult GetFilterByCurrentUser(int districtId = 0)
        {
            var qtiRefObjects = GetAllPassageOfCurrentUser(districtId);

            var grades = qtiRefObjects
                .Where(x => x.GradeName != null && x.GradeName != "")
                .Select(x => new { x.GradeID, x.GradeName, x.GradeOrder })
                .Distinct()
                .OrderBy(x => x.GradeOrder)
                .Select(x => new ListItem
                {
                    Id = x.GradeID.GetValueOrDefault(),
                    Name = x.GradeName.ToString()
                })
                .ToList();

            var subjects = qtiRefObjects
                .Where(x => x.Subject != null && x.Subject != "")
                .Select(x => x.Subject)
                .Distinct()
                .OrderBy(x => x)
                .Select(x => new ListItemStr
                {
                    Id = x.Replace(" ", "").ToLower(),
                    Name = x
                })
                .ToList();

            var passageNumbers = qtiRefObjects
                .Select(x => new { x.QTIRefObjectID, x.QTIRefObjectFileRef })
                .Distinct()
                .OrderBy(x => x.QTIRefObjectFileRef)
                .Select(x => new ListItem()
                {
                    Id = x.QTIRefObjectID,
                    Name = x.QTIRefObjectFileRef.ToString()
                })
                .ToList();

            var textTypes = qtiRefObjects
                .Where(x => x.TextType != null && x.TextType != "")
                .Select(x => new { x.TextTypeID, x.TextType })
                .Distinct()
                .OrderBy(x => x.TextType)
                .Select(x => new ListItem()
                {
                    Id = x.TextTypeID.GetValueOrDefault(),
                    Name = x.TextType
                })
                .ToList();

            var textSubTypes = qtiRefObjects
                .Where(x => x.TextSubType != null && x.TextSubType != "")
                .Select(x => new { x.TextSubTypeID, x.TextSubType })
                .Distinct()
                .OrderBy(x => x.TextSubType)
                .Select(x => new ListItem()
                {
                    Id = x.TextSubTypeID.GetValueOrDefault(),
                    Name = x.TextSubType
                })
                .ToList();

            var fleschKincaids = qtiRefObjects
                .Where(x => x.FleschKinkaidName != null && x.FleschKinkaidName != "")
                .Select(x => x.FleschKincaidID)
                .Distinct()
                .ToList();

            var listOfFleschKincaids = parameters.QiQti3Service
                .GetFleschKinkaids()
                .Where(x => fleschKincaids.Contains(x.FleschKincaidID))
                .Select(x => new ListItem()
                {
                    Id = x.FleschKincaidID,
                    Name = x.Name
                })
                .ToList();

            var data = new
            {
                Grades = grades,
                Subjects = subjects,
                PassageNumbers = passageNumbers,
                TextTypes = textTypes,
                TextSubTypes = textSubTypes,
                FleschKincaids = listOfFleschKincaids
            };
            var result = new LargeJsonResult
            {
                Data = data,
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return result;
        }

        [HttpGet]
        public ActionResult GetGradesByCurrentUser(int districtId = 0)
        {
            var temp = GetAllPassageOfCurrentUser(districtId)
                .Where(x => x.GradeName != null && x.GradeName != "")
                .Select(x => new { x.GradeID, x.GradeName, x.GradeOrder })
                .Distinct().OrderBy(x => x.GradeOrder).ToList();

            var data = temp.Select(x => new ListItem()
            {
                Id = x.GradeID.GetValueOrDefault(),
                Name = x.GradeName.ToString()
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSubjectsByCurrentUser(int districtId = 0)
        {
            var temp = GetAllPassageOfCurrentUser(districtId)
                .Where(x => x.Subject != null && x.Subject != "")
                .Select(x => new { x.Subject })
                .Distinct()
                .OrderBy(x => x.Subject)
                .ToList();

            var data = new List<ListItemStr>();

            foreach (var subject in temp)
            {
                var item = new ListItemStr
                {
                    Id = subject.Subject.Replace(" ", "").ToLower(),
                    Name = subject.Subject
                };
                data.Add(item);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPassageNumbersByCurrentUser(int districtId = 0)
        {
            var temp = GetAllPassageOfCurrentUser(districtId)
                .Select(x => new { x.QTIRefObjectID, x.QTIRefObjectFileRef })
                .Distinct().OrderBy(x => x.QTIRefObjectFileRef).ToList();
            var data = temp.Select(x => new ListItem()
            {
                Id = x.QTIRefObjectID,
                Name = x.QTIRefObjectFileRef.ToString()
            }).ToList();
            var result = new LargeJsonResult
            {
                Data = data,
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return result;
        }

        [HttpGet]
        public ActionResult GetTextTypesByCurrentUser(int districtId = 0)
        {
            var temp = GetAllPassageOfCurrentUser(districtId)
                .Where(x => x.TextType != null && x.TextType != "")
                .Select(x => new { x.TextTypeID, x.TextType })
                .Distinct().OrderBy(x => x.TextType).ToList();

            var data = temp.Select(x => new ListItem()
            {
                Id = x.TextTypeID.GetValueOrDefault(),
                Name = x.TextType
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTextSubTypesByCurrentUser(int districtId = 0)
        {
            var temp = GetAllPassageOfCurrentUser(districtId)
                .Where(x => x.TextSubType != null && x.TextSubType != "")
                .Select(x => new { x.TextSubTypeID, x.TextSubType })
                .Distinct().OrderBy(x => x.TextSubType).ToList();

            var data = temp.Select(x => new ListItem()
            {
                Id = x.TextSubTypeID.GetValueOrDefault(),
                Name = x.TextSubType
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetFleschKincaidsByCurrentUser(int districtId = 0)
        {
            var fleschKincaidIds = GetAllPassageOfCurrentUser(districtId)
                .Where(x => x.FleschKinkaidName != null && x.FleschKinkaidName != "")
                .Select(x => x.FleschKincaidID).Distinct().ToList();

            var temp =
                parameters.QiQti3Service.GetFleschKinkaids().Where(x => fleschKincaidIds.Contains(x.FleschKincaidID));

            var data = temp.Select(x => new ListItem()
            {
                Id = x.FleschKincaidID,
                Name = x.Name
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private IQueryable<QtiRefObject> GetAllPassageOfCurrentUser(int districtId = 0)
        {
            if (!CurrentUser.IsPublisherOrNetworkAdmin)
            {
                districtId = CurrentUser.DistrictId ?? 0;
            }

            var passageList = parameters.QtiRefObjectService.GetQtiRefObject(new GetQtiRefObjectFilter
            {
                UserId = CurrentUser.Id,
                DistrictId = districtId,
                StartRow = 0,
                PageSize = int.MaxValue
            });
            return passageList;
        }

        private S3Result UploadPassageXmlFileToS3(int qTIRefObjectID, string xmlContent)
        {
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent ?? ""));

            var folder = LinkitConfigurationManager.GetS3Settings().AUVirtualTestROFolder;
            var bucketName = LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName;
            string relativeFilePath = string.Format("{0}/RO/RO_{1}.xml", folder.RemoveStartSlash().RemoveEndSlash(),
                qTIRefObjectID);
            return _s3Service.UploadRubricFile(bucketName, relativeFilePath.RemoveStartSlash(), fileStream, false);
        }

        public ActionResult CheckVirtualTestsHasRefObject(int qtiRefObjectId, int virtualTestId)
        {
            var numOfVirtualTest = parameters.QtiRefObjectService.CountVirtualTestByRefObjectId(qtiRefObjectId, virtualTestId);
            return Json(new { NumOfVirtualTest = numOfVirtualTest }, JsonRequestBehavior.AllowGet);
        }

        private GetQtiRefObjectFilter MappingRequest(GetQtiRefObjectFilterRequest criteria)
        {
            var request = new GetQtiRefObjectFilter
            {
                UserId = CurrentUser.Id,
                GradeId = criteria.GradeId.HasValue && criteria.GradeId.Value == 0 ? null : criteria.GradeId,
                Subject = HttpUtility.UrlDecode(criteria.Subject),
                TextTypeId = criteria.TextTypeId.HasValue && criteria.TextTypeId.Value == 0 ? null : criteria.TextTypeId,
                TextSubTypeId = criteria.TextSubTypeId.HasValue && criteria.TextSubTypeId.Value == 0 ? null : criteria.TextSubTypeId,
                FleschKincaidId = criteria.FleschKincaidId.HasValue && criteria.FleschKincaidId.Value == 0 ? null : criteria.FleschKincaidId,
                Name = HttpUtility.UrlDecode(criteria.NameSearch),
                PassageNumber = criteria.PassageNumber,
                DistrictId = criteria.DistrictId,
                PageSize = criteria.iDisplayLength > 0 ? criteria.iDisplayLength : 20,
                StartRow = criteria.iDisplayStart,
                GeneralSearch = criteria.sSearch,
            };
            if (!string.IsNullOrWhiteSpace(criteria.sColumns) && criteria.iSortCol_0.HasValue)
            {
                var columns = criteria.sColumns.Split(',');
                request.SortColumn = columns[criteria.iSortCol_0.Value];
                request.SortDirection = string.Compare(criteria.sSortDir_0, "desc") == 0 ? "DESC" : "ASC";
            }
            return request;
        }
    }
}
