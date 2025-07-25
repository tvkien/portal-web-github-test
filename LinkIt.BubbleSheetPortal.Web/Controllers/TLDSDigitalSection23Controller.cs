using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.Old.TLDS;
using LinkIt.BubbleSheetPortal.Models.TLDS;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.App_Start;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Web.Models.TLDS;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS;
using LinkIt.BubbleSheetPortal.Web.ViewModels.TLDSDigital;
using S3Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    public class TLDSDigitalSection23Controller : BaseController
    {
        private readonly DistrictDecodeService _districtDecodeService;
        private readonly TLDSDigitalSection23ControllerParameters _parameters;
        private readonly DistrictService _districtService;
        private readonly IS3Service _s3Service;
        private List<TLDSDevelopmentOutcomeTypeViewModel> _tldsDevelopmentOutcomeTypeList = null;

        private Configuration _enrolmentYearConfig;
        private Configuration EnrolmentYearConfig
        {
            get
            {
                if (_enrolmentYearConfig == null)
                {
                    _enrolmentYearConfig = _configurationService.GetConfigurationByKey("TLDSReadOnly");
                }
                return _enrolmentYearConfig;
            }
        }
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
        private HttpCookie httpCookie = new HttpCookie("tldscookie");
        private readonly ConfigurationService _configurationService;

        public TLDSDigitalSection23Controller(TLDSDigitalSection23ControllerParameters parameters, DistrictDecodeService districtDecodeService,
                                              ConfigurationService configurationService, DistrictService districtService, IS3Service s3Service)
        {
            _parameters = parameters;
            _districtDecodeService = districtDecodeService;
            _configurationService = configurationService;
            _districtService = districtService;
            _s3Service = s3Service;
        }

        [TldsDigitalActionFilter(IdParamName = "id")]
        public ActionResult Index(Guid id)
        {
            var profileIsSubmitted = _parameters.TLDSDigitalSection23Service.GetProfile(id).Status;
            var model = new TldsWelcomeViewModel();
            model.FormSection2Status = GetStatusFormSection2(id);
            model.FormSection3Status = GetStatusFormSection3(id);
            ViewBag.TldsProfileLinkID = id;
            ViewBag.ReadOnly = (profileIsSubmitted == 30);

            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult SubmitLogin(TldsLoginViewModel loginInfo)
        {
            DateTime date = DateTime.MinValue;
            TryParseDateWithFormat(loginInfo.DateOfBirthString, out date);
            loginInfo.DateOfBirth = date;

            var isStudent = _parameters.TLDSDigitalSection23Service.LoginTLDSForm(loginInfo.Id, loginInfo.DateOfBirth);

            if (!isStudent)
            {
                var configuration = _configurationService.GetConfigurationByKey(ConfigurationNameConstant.TLDSLoginFailed);
                if (configuration != null)
                {
                    int loginLimit = 0;
                    int.TryParse(configuration.Value, out loginLimit);
                    var tldsProfileLink = _parameters.TLDSDigitalSection23Service.SelectTLDSProfileLink()
                                                                            .FirstOrDefault(x => x.TLDSProfileLinkID == loginInfo.Id);
                    if (tldsProfileLink != null)
                    {
                        var loggedCount = loginLimit - tldsProfileLink.LoginFailed;
                        if (loggedCount <= 0)
                        {
                            return Json(new { status = loggedCount, JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            var countLoginFail = _parameters.TLDSDigitalSection23Service.UpdateLoginFail(loginInfo.Id, loginLimit);
                            var loginFailed = loginLimit - countLoginFail;
                            return Json(new { status = loginFailed, JsonRequestBehavior.AllowGet });
                        }
                    }
                }
            }

            _parameters.TLDSDigitalSection23Service.ResetLoginFailCount(loginInfo.Id);
            httpCookie.Value = loginInfo.Id.ToString();
            httpCookie.Expires = DateTime.Now.AddHours(1);
            System.Web.HttpContext.Current.Response.Cookies.Add(httpCookie);

            return Json(new { status = "success", newUrl = Url.Action("Index", new { id = loginInfo.Id }) });
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult Login(Guid id)
        {
            var deactive = _parameters.TLDSDigitalSection23Service.CheckLinkStatus(id);
            var isExpired = _parameters.TLDSDigitalSection23Service.CheckLinkExpired(id);

            if (deactive)
            {
                return RedirectToAction("Error", new { isExpired = false });
            }
            else if (isExpired)
            {
                return RedirectToAction("Error", new { isExpired = true });
            }
            else
            {
                var tldsProfile = _parameters.TLDSDigitalSection23Service.GetProfile(id);

                if (tldsProfile == null)
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }

                ViewBag.ProfileLinkId = id;

                var subDomain = HelperExtensions.GetSubdomain();
                var districtId = _districtService.GetLiCodeBySubDomain(subDomain);

                var logOnHeaderHtmlContent = string.Empty;
                if (districtId > 0)
                {                    
                    using (var client = new HttpClient())
                    {
                        var url = string.Format("{0}{1}/{2}", LinkitConfigurationManager.GetS3Settings().S3CSSKey, districtId, "LogOn.html");
                        try
                        {
                            HttpResponseMessage response = client.GetAsync(url).Result;
                            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotModified)
                            {
                                logOnHeaderHtmlContent = response.Content.ReadAsStringAsync().Result;
                            }
                        }
                        catch (Exception)
                        {
                            // do nothing
                        }
                    }
                }

                var model = new TldsProfileViewModel
                {
                    ProfileId = tldsProfile.ProfileId,
                    FirstName = tldsProfile.FirstName,
                    LastName = tldsProfile.LastName,
                    DateOfBirth = tldsProfile.DateOfBirth,
                    LogOnHeaderHtmlContent = logOnHeaderHtmlContent
                };

                return View(model);
            }
        }

        [TldsDigitalActionFilter(IdParamName = "id")]
        public ActionResult ChildInformation(Guid id)
        {
            var masterModel = GetMasterModel(id);

            return View(masterModel);
        }

        [TldsDigitalActionFilter(IdParamName = "tldsProfileLinkId")]
        public ActionResult ChildForm(Guid tldsProfileLinkId)
        {
            var model = new TldsFormSection2ViewModel();
            var tldsProfile = _parameters.TLDSDigitalSection23Service.GetProfile(tldsProfileLinkId);
            if (tldsProfile != null)
            {
                ViewBag.ChildName = string.Join(" ", tldsProfile.FirstName, tldsProfile.LastName);
            }

            var childForm = _parameters.TLDSDigitalSection23Service.GetFormSections2(tldsProfileLinkId);
            if (childForm == null)
            {
                return View(model);
            }
            model.TLDSFormSection2ID = childForm.TLDSFormSection2ID;
            model.TLDSProfileLinkID = childForm.TLDSProfileLinkID;
            model.GuardianName = childForm.GuardianName;
            model.Relationship = childForm.Relationship;
            model.Favourite = childForm.Favourite;
            model.Strengths = childForm.Strengths;
            model.Weaknesses = childForm.Weaknesses;
            model.Interested = childForm.Interested;
            model.Expected = childForm.Expected;
            model.Drawing = childForm.Drawing;
            model.IsSubmitted = childForm.IsSubmitted;
            model.DrawingWithAbsolutePath = GetLinkToDownloadChildPhoto(childForm.Drawing);

            return View(model);
        }

        [TldsDigitalActionFilter(IdParamName = "tldsProfileLinkId")]
        public ActionResult FamilyForm(Guid tldsProfileLinkId)
        {
            var model = new TldsFormSection3ViewModel();

            var familyForm = _parameters.TLDSDigitalSection23Service.GetFormSections3(tldsProfileLinkId);
            if (familyForm == null)
            {
                model.IsSubmitted = false;
                return View(model);
            }
            model.TLDSFormSection3ID = familyForm.TLDSFormSection3ID;
            model.TLDSProfileLinkID = familyForm.TLDSProfileLinkID;
            model.GuardianName = familyForm.GuardianName;
            model.Relationship = familyForm.Relationship;
            model.PreferredLanguage = familyForm.PreferredLanguage;
            if (familyForm.IsAborigial.HasValue)
                model.IsAborigial = familyForm.IsAborigial.Value;
            else
                model.IsAborigial = null;
            if (familyForm.HaveSiblingInSchool.HasValue)
                model.HaveSiblingInSchool = familyForm.HaveSiblingInSchool.Value;
            else
                model.HaveSiblingInSchool = null;
            model.NameAndGradeOfSibling = familyForm.NameAndGradeOfSibling;
            model.Wishes = familyForm.Wishes;
            model.InformationSchool = familyForm.InformationSchool;
            model.HelpInformation = familyForm.HelpInformation;
            model.Interested = familyForm.Interested;
            model.ConditionImprovement = familyForm.ConditionImprovement;
            model.OtherInformation = familyForm.OtherInformation;
            model.IsSubmitted = familyForm.IsSubmitted.HasValue ? familyForm.IsSubmitted.Value : false;

            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ChildForm(TldsFormSection2ViewModel model)
        {
            var formSection2 = new TLDSFormSection2
            {
                TLDSFormSection2ID = model.TLDSFormSection2ID.HasValue ? model.TLDSFormSection2ID.Value : 0,
                TLDSProfileLinkID = model.TLDSProfileLinkID,
                GuardianName = model.GuardianName,
                Relationship = model.Relationship,
                Favourite = model.Favourite,
                Strengths = model.Strengths,
                Weaknesses = model.Weaknesses,
                Interested = model.Interested,
                Expected = model.Expected,
                Drawing = model.Drawing
            };

            if (model.RotatePhoto != 0)
            {
                var tldsProfile = _parameters.TLDSDigitalSection23Service.GetProfile(model.TLDSProfileLinkID);
                var profileId = tldsProfile != null ? tldsProfile.ProfileId : 0;
                var nameArr = model.Drawing.Split('?')[0].Split('/');
                var fileName = nameArr[nameArr.Length - 1];
                var tldsS3Setting = TldsDigitalS3Setting.GetTldsDigitalS3Setting(profileId, model.TLDSProfileLinkID);

                var newPhotoName = RotatePhoto(tldsS3Setting, fileName, model.RotatePhoto);
                formSection2.Drawing = tldsS3Setting.GetPhotoPath(newPhotoName);
            }

            _parameters.TLDSDigitalSection23Service.SubmittedFormSection2(formSection2);
            SendMailSubmitedSection(formSection2.TLDSProfileLinkID, 2);
            return Json(new { status = "success", newUrl = Url.Action("Index", new { id = model.TLDSProfileLinkID }) });
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult AddOrUpdateSection2(TldsFormSection2ViewModel model)
        {
            var formSection2 = new TLDSFormSection2
            {
                TLDSFormSection2ID = model.TLDSFormSection2ID.HasValue ? model.TLDSFormSection2ID.Value : 0,
                TLDSProfileLinkID = model.TLDSProfileLinkID,
                GuardianName = model.GuardianName,
                Relationship = model.Relationship,
                Favourite = model.Favourite,
                Strengths = model.Strengths,
                Weaknesses = model.Weaknesses,
                Interested = model.Interested,
                Expected = model.Expected,
                Drawing = model.Drawing
            };

            if(model.RotatePhoto != 0)
            {
                var tldsProfile = _parameters.TLDSDigitalSection23Service.GetProfile(model.TLDSProfileLinkID);
                var profileId = tldsProfile != null ? tldsProfile.ProfileId : 0;
                var nameArr = model.Drawing.Split('?')[0].Split('/');
                var fileName = nameArr[nameArr.Length - 1];
                var tldsS3Setting = TldsDigitalS3Setting.GetTldsDigitalS3Setting(profileId, model.TLDSProfileLinkID);

                var newPhotoName = RotatePhoto(tldsS3Setting, fileName, model.RotatePhoto);
                formSection2.Drawing = tldsS3Setting.GetPhotoPath(newPhotoName);
            }

            var tldsFormSection2 = _parameters.TLDSDigitalSection23Service.GetFormSections2(formSection2.TLDSProfileLinkID);

            if (tldsFormSection2 == null)
            {
                _parameters.TLDSDigitalSection23Service.SaveTldsFormSection2(formSection2);
            }
            else
            {
                _parameters.TLDSDigitalSection23Service.UpdateTldsFormSection2(formSection2);
            }

            return Json(new { status = "success", newUrl = Url.Action("Index", new { id = formSection2.TLDSProfileLinkID }) });
        }

        private string RotatePhoto(TldsDigitalS3Setting s3Setting, string fileName, int rotate)
        {
            var photoChildPath = s3Setting.GetPhotoPath(fileName);
            var result = _s3Service.DownloadFile(s3Setting.TLDSBucket, photoChildPath);
            if (result.IsSuccess)
            {
                var arr = fileName.Split('-');
                var newName = fileName.AddTimestampToFileName();
                if (arr.Length > 1)
                {
                    var exArr = arr[arr.Length - 1].Split('.');
                    if (exArr.Length > 1)
                    {
                        newName = arr[0].AddTimestampToFileName() + "." + exArr[exArr.Length - 1];
                    }
                }

                var newPhotoChildPath = s3Setting.GetPhotoPath(newName);
                var stream = Util.RotateImage(result.ReturnStream, rotate);
                var s3Result = _s3Service.UploadRubricFile(s3Setting.TLDSBucket, newPhotoChildPath, stream, false);
                if (s3Result.IsSuccess)
                {
                    try
                    {
                        //delete unsued file
                        _s3Service.DeleteFile(s3Setting.TLDSBucket, photoChildPath);
                    }
                    catch
                    {
                        //nothing 
                    }

                    return newName;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult FamilyForm(TldsFormSection3ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
           
            var formSection3 = new TLDSFormSection3
            {
                TLDSFormSection3ID = model.TLDSFormSection3ID.HasValue ? model.TLDSFormSection3ID.Value : 0,
                TLDSProfileLinkID = model.TLDSProfileLinkID,
                GuardianName = model.GuardianName,
                Relationship = model.Relationship,
                PreferredLanguage = model.PreferredLanguage,
                IsAborigial = model.IsAborigial,
                HaveSiblingInSchool = model.HaveSiblingInSchool,
                NameAndGradeOfSibling = model.NameAndGradeOfSibling,
                Wishes = model.Wishes,
                InformationSchool = model.InformationSchool,
                Interested = model.Interested,
                HelpInformation = model.HelpInformation,
                ConditionImprovement = model.ConditionImprovement,
                OtherInformation = model.OtherInformation
            };

            _parameters.TLDSDigitalSection23Service.SubmittedFormSection3(formSection3);
            SendMailSubmitedSection(formSection3.TLDSProfileLinkID, 3);
            return Json(new { status = "success", newUrl = Url.Action("Index", new { id = model.TLDSProfileLinkID }) });
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult AddOrUpdateSection3(TldsFormSection3ViewModel model)
        {
            var formSection3 = new TLDSFormSection3
            {
                TLDSFormSection3ID = model.TLDSFormSection3ID.HasValue ? model.TLDSFormSection3ID.Value : 0,
                TLDSProfileLinkID = model.TLDSProfileLinkID,
                GuardianName = model.GuardianName,
                Relationship = model.Relationship,
                PreferredLanguage = model.PreferredLanguage,
                IsAborigial = model.IsAborigial,
                HaveSiblingInSchool = model.HaveSiblingInSchool,
                NameAndGradeOfSibling = model.NameAndGradeOfSibling,
                Wishes = model.Wishes,
                InformationSchool = model.InformationSchool,
                Interested = model.Interested,
                HelpInformation = model.HelpInformation,
                ConditionImprovement = model.ConditionImprovement,
                OtherInformation = model.OtherInformation
            };

            var tldsFormSection3 = _parameters.TLDSDigitalSection23Service.GetFormSections3(formSection3.TLDSProfileLinkID);

            if (tldsFormSection3 == null)
            {
                _parameters.TLDSDigitalSection23Service.SaveTldsFormSection3(formSection3);
            }
            else
            {
                _parameters.TLDSDigitalSection23Service.UpdateTldsFormSection3(formSection3);
            }
            return Json(new { status = "success", newUrl = Url.Action("Index", new { id = formSection3.TLDSProfileLinkID }) });
        }

        public ActionResult CreateNewLink(int profileId)
        {
            var model = new TLDSProfileLink()
            {
                ProfileId = profileId,
                TLDSProfileLinkID = Guid.NewGuid(),
                LinkUrl = Request.Url.Host,
                ExpiredDate = DateTime.UtcNow.AddDays(14).Date,
                IsActive = true
            };

            _parameters.TLDSDigitalSection23Service.SaveTLDSProfileLink(model);
            return LoadTLDSProfileLinkByProfileId(profileId);
        }

        public ActionResult LoadTLDSProfileLinkByProfileId(int profileId)
        {
            DateTimeFormat();
            var profileLinks = _parameters.TLDSDigitalSection23Service.GetTLDSProfileLink(HelperExtensions.GetHTTPProtocal(Request), profileId, CurrentUser.Id, new TLDSProfileLinkFilterParameter());
            var tldsProfile = _parameters.TLDSService.GetTLDSProfile(profileId);
            ViewBag.ProfileStatus = tldsProfile?.Status;
            ViewBag.FormatDate = DateTimeFormat();
            return PartialView("_CreateNewLink", profileLinks);
        }

        public ActionResult CreateNewLinkV2(int profileId)
        {
            var model = new TLDSProfileLink()
            {
                ProfileId = profileId,
                TLDSProfileLinkID = Guid.NewGuid(),
                LinkUrl = Request.Url.Host,
                ExpiredDate = DateTime.UtcNow.AddDays(14).Date,
                IsActive = true
            };

            _parameters.TLDSDigitalSection23Service.SaveTLDSProfileLink(model);
            return LoadTLDSProfileLinkByProfileIdV2(profileId);
        }

        public ActionResult LoadTLDSProfileLinkByProfileIdV2(int profileId)
        {
            DateTimeFormat();
            var profileLinks = _parameters.TLDSDigitalSection23Service.GetTLDSProfileLink(HelperExtensions.GetHTTPProtocal(Request), profileId, CurrentUser.Id, new TLDSProfileLinkFilterParameter());
            var tldsProfile = _parameters.TLDSService.GetTLDSProfile(profileId);
            ViewBag.ProfileStatus = tldsProfile?.Status;
            ViewBag.FormatDate = DateTimeFormat();
            return PartialView("v2/_CreateNewLink", profileLinks);
        }

        [System.Web.Http.HttpPost]
        public ActionResult DeleteTldsProfileLink(Guid tldsProfileLinkId)
        {
            var isDeleted = _parameters.TLDSDigitalSection23Service.DeleteTldsProfileLink(tldsProfileLinkId);
            return Json(new { Result = isDeleted }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadTLDSProfleLink(TLDSProfileLinkFilterParameter parameter)
        {
            var parser = new DataTableParser<TLDSProfileLinkViewModel>();
            var tdlsProfiles = _parameters.TLDSDigitalSection23Service.GetTLDSProfileLink(HelperExtensions.GetHTTPProtocal(Request), 0, CurrentUser.Id, parameter)
                    .Select(x => new TLDSProfileLinkViewModel
                    {
                        ProfileId = x.ProfileId,
                        StudentFirstName = x.StudentFirstName,
                        StudentLastName = x.StudentLastName,
                        Guardian = x.Guardian,
                        SectionCompleted = x.SectionCompleted,
                        LinkUrl = x.LinkUrl,
                        ExpiredDate = x.ExpiredDate,
                        Status = x.Status,
                        TLDSProfileLinkId = x.TLDSProfileLinkID.ToString(),
                        IsAccess = x.Status == Constanst.TLDSProfileLink_Deactivated ? true : false,
                        ProfileStatus = x.ProfileStatus,
                        ProfileIsReadOnly = x.IsReadOnly,
                        EnrolmentYear = x.EnrolmentYear,
                        TLDSGroupID = x.TLDSGroupID
                    });

            return Json(parser.Parse(tdlsProfiles.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RefreshTSLDProfileLink(Guid tldsProfileLinkId)
        {
            var configuration = _configurationService.GetConfigurationByKey(ConfigurationNameConstant.RefreshTLDSProfileLink);
            var value = configuration != null ? configuration.Value : "0";
            var result = _parameters.TLDSDigitalSection23Service.RefreshTLDSProfileLink(tldsProfileLinkId, int.Parse(value));
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateStatusTLDSProfileLink(Guid tldsProfileLinkId, bool value)
        {
            var result = _parameters.TLDSDigitalSection23Service.UpdateTLDSProfileLink(tldsProfileLinkId, value);

            return Json(new { result }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UploadChildDrawing(string imageSrc, Guid profileLinkId, string fileName)
        {
            var tldsProfile = _parameters.TLDSDigitalSection23Service.GetProfile(profileLinkId);
            var profileId = tldsProfile != null ? tldsProfile.ProfileId : 0;

            var tldsS3Setting = TldsDigitalS3Setting.GetTldsDigitalS3Setting(profileId, profileLinkId);

            try
            {
                fileName = fileName.AddTimestampToFileName();
                string photoFileName = tldsS3Setting.GetPhotoPath(fileName);
                imageSrc = imageSrc.Replace("data:image/png;base64,", "");
                byte[] imageData = Convert.FromBase64String(imageSrc); //base64
                using (var stream = new MemoryStream(imageData))
                {
                    var s3Result = _s3Service.UploadRubricFile(tldsS3Setting.TLDSBucket, photoFileName,
                        stream, false);
                    if (s3Result.IsSuccess)
                    {
                        return
                            Json(
                                new
                                {
                                    success = true,
                                    fileNameUrl = GetLinkToDownloadChildPhoto(photoFileName),
                                    drawing = photoFileName
                                },
                                JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { Success = false, ErrorMessage = s3Result.ErrorMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Error(bool isExpired = true)
        {
            ViewBag.IsExpired = isExpired;
            return View();
        }

        public ActionResult GeneratePDFBlankForm(TldsDigitalReportViewModel model)
        {
            try
            {
                string url = string.Empty;
                var dateFormatCookie = System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultDateFormat];
                model.DateFormat = dateFormatCookie == null ? TextConstants.DEFAULT_DATE_FORMAT_AU : dateFormatCookie.Value;

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

        [System.Web.Http.HttpPost]
        [AjaxOnly]
        public ActionResult GetTLDSReportS3File(string fileName, int profileId)
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

        public ActionResult PrintPDFForm(Guid id)
        {
            var masterModel = GetMasterModel(id);
            masterModel.IsPrinting = true;
            masterModel.DateOfBirthFormated = ParseDateWithFormat(masterModel.DateOfBirth);

            return View("ChildInformation", masterModel);
        }

        private byte[] PrintBlankPDFForm(TldsDigitalReportViewModel model, out string url)
        {

            byte[] pdf = null;
            url = string.Empty;

            url = Url.Action("PrintPDFForm", "TLDSDigitalSection23", new { id = model.TldsProfileLinkId }, HelperExtensions.GetHTTPProtocal(Request));
            pdf = ExportToPDF(url, model.TimezoneOffset, model.ProfileId);

            return pdf;
        }

        private byte[] ExportToPDF(string url, int timezoneOffset, int profileId)
        {
            DateTime dt = DateTime.UtcNow.AddMinutes(timezoneOffset * (-1));

            string footerUrl = Url.Action("RenderFooter", "TLDSDigitalSection23", new
            {
                profileId = profileId,
                dateTimeNow = dt
            }, HelperExtensions.GetHTTPProtocal(Request));

            string headerUrl = Url.Action("RenderHeader", "TLDSDigitalSection23", new { profileId = profileId }, HelperExtensions.GetHTTPProtocal(Request));

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

        private void SendMailSubmitedSection(Guid profileLinkId, int sectionNumber)
        {
            var tldsProfileInfo = _parameters.TLDSDigitalSection23Service.GetTLDSInformationForSection23(profileLinkId);
            if (tldsProfileInfo != null && (sectionNumber == 2 || sectionNumber == 3))
            {
                string childName = string.Empty;
                if (!string.IsNullOrWhiteSpace(tldsProfileInfo.FirstName) && !string.IsNullOrWhiteSpace(tldsProfileInfo.LastName))
                {
                    childName = string.Format("{0}, {1}", tldsProfileInfo.LastName, tldsProfileInfo.FirstName);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(tldsProfileInfo.FirstName))
                    {
                        childName = tldsProfileInfo.FirstName;
                    }
                    if (!string.IsNullOrWhiteSpace(tldsProfileInfo.LastName))
                    {
                        childName = tldsProfileInfo.LastName;
                    }
                }
                var objEmailTemplate = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(CurrentUser.DistrictId ?? 0,
                                        Constanst.TLDSSendMailSubmitedSectionTemplate).FirstOrDefault();
                var linkUrl = string.Format("{0}://{1}/TLDSDigitalSection23?id={2}", HelperExtensions.GetHTTPProtocal(Request), tldsProfileInfo.LinkUrl, tldsProfileInfo.TLDSProfileLinkId);
                string strBody = string.Empty;
                if (objEmailTemplate != null)
                {
                    strBody = objEmailTemplate.Value;
                }
                else
                {
                    strBody = "<Section Number> has been submitted for <Child Name> via a shared Online TLDS link.<br><br>";
                    strBody += "Please login to your Online TLDS account to view this document - [PortalLink] <br><br>";
                    strBody += "This is an automatically generated email, <b>please do not reply<b>.";
                }

                strBody = strBody.Replace("<Section Number>", "Section " + sectionNumber.ToString());
                strBody = strBody.Replace("<Child Name>", childName);
                strBody = strBody.Replace("[PortalLink]", linkUrl);
                string strSubject = "A TLDS section has been submitted";

                var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.TLDSUseEmailCredentialKey);
                try
                {
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        Helpers.Util.SendMailTLDSV2(strBody, strSubject, tldsProfileInfo.Email, emailCredentialSetting);
                    },
                    System.Threading.Tasks.TaskCreationOptions.DenyChildAttach);
                }
                catch
                {
                }
            }
        }

        private string DateTimeFormat()
        {
            var dateFormatModel = _districtDecodeService.GetDateFormat(CurrentUser.DistrictId.GetValueOrDefault());
            if (dateFormatModel == null)
            {
                return "";
            }
            else
            {
                return dateFormatModel.DateFormat;
            }
        }

        private TldsFormStatusViewModel GetStatusFormSection2(Guid id)
        {
            var section2Model = new TldsFormStatusViewModel();
            var section2Result = _parameters.TLDSDigitalSection23Service.GetFormSections2(id);
            if (section2Result == null)
            {
                section2Model.ButtonText = Constanst.TLDSForm_OpenButtonText;
            }
            else
            {
                var isSubmitted = section2Result.IsSubmitted;
                switch (isSubmitted)
                {
                    case false:
                        section2Model.ButtonText = Constanst.TLDSForm_SaveButtonText;
                        break;
                    case true:
                        section2Model.ButtonText = Constanst.TLDSForm_SubmitButtonText;
                        section2Model.IsSubmitted = true;
                        break;
                    default:
                        break;
                }
            }

            return section2Model;
        }

        private TldsFormStatusViewModel GetStatusFormSection3(Guid id)
        {
            var section3Model = new TldsFormStatusViewModel();
            var section3Result = _parameters.TLDSDigitalSection23Service.GetFormSections3(id);
            if (section3Result == null)
            {
                section3Model.ButtonText = Constanst.TLDSForm_OpenButtonText;
            }
            else
            {
                var isSubmitted = section3Result.IsSubmitted.HasValue ? section3Result.IsSubmitted.Value : false;
                switch (isSubmitted)
                {
                    case false:
                        section3Model.ButtonText = Constanst.TLDSForm_SaveButtonText;
                        break;
                    case true:
                        section3Model.ButtonText = Constanst.TLDSForm_SubmitButtonText;
                        section3Model.IsSubmitted = true;
                        break;
                    default:
                        break;
                }
            }

            return section3Model;
        }

        private TldsProfileDigitalViewModel GetMasterModel(Guid id)
        {
            var tldsProfile = _parameters.TLDSDigitalSection23Service.GetProfile(id);
            var masterModel = new TldsProfileDigitalViewModel()
            {
                ProfileId = tldsProfile.ProfileId
            };

            BuildGeneralData(masterModel);
            BuildTldsAdditionalInformation(masterModel);
            BuildTldsDevelopmentOutcomeProfile(masterModel);
            BuildTldsSEarlyABLESReport(masterModel);
            BuildTldsOtherReportPlan(masterModel);
            BuildTldsParentGuardian(masterModel);
            BuildTldsProfessionalService(masterModel);
            masterModel.SectionPrint2 = BuildTldsSection2(id);
            masterModel.SectionPrint3 = BuildTldsSection3(id);

            var eyaltProfile = masterModel.DevelopmentOutcomeProfiles.FirstOrDefault(x => x.DevelopmentOutcomeTypeId == 6
                                                                                        && !string.IsNullOrEmpty(x.OriginalFileName)
                                                                                        && !string.IsNullOrEmpty(x.S3FileName));
            if (eyaltProfile != null)
            {
                var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(tldsProfile.ProfileId);
                var eyaltPdf = _s3Service.DownloadFile(LinkitConfigurationManager.GetS3Settings().TLDSBucket, tldsS3Settings.GetEYALTUploadedPath(tldsProfile.ProfileId, eyaltProfile.S3FileName));
                if (eyaltPdf.IsSuccess)
                {
                    var stream = new MemoryStream(eyaltPdf.ReturnStream);
                    masterModel.EYALTFileImages.AddRange(PdfHelper.ConvertImage(stream, 300, false));
                }
            }

            return masterModel;
        }

        private void BuildGeneralData(TldsProfileDigitalViewModel model)
        {
            var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(model.ProfileId);
            //basic information
            var profile = _parameters.TLDSService.GetTLDSProfileIncludeMeta(model.ProfileId);
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
            model.ECSCompledDateFormated = ParseDateWithFormat(profile.ECSCompledDate);
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

        private void BuildTldsAdditionalInformation(TldsProfileDigitalViewModel model)
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

        private void BuildTldsDevelopmentOutcomeProfile(TldsProfileDigitalViewModel model)
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
                        OriginalFileName = x.OriginalFileName
                    }).ToList();
        }

        private void BuildTldsSEarlyABLESReport(TldsProfileDigitalViewModel model)
        {
            model.TLDSEarlyABLESReport = _parameters.TLDSService.GetTLDSEarlyABLESReportOfProfile(model.ProfileId)
                .Select(x => new TLDSEarlyABLESReportViewModel()
                {
                    EarlyABLESReportId = x.EarlyABLESReportId,
                    ReportName = x.ReportName,
                    ReportDate = x.ReportDate,
                    ReportDateFormated = ParseDateWithFormat(x.ReportDate),
                    LearningReadinessReportCompleted = x.LearningReadinessReportCompleted,
                    AvailableOnRequest = x.AvailableOnRequest
                }).ToList();
        }

        private void BuildTldsOtherReportPlan(TldsProfileDigitalViewModel model)
        {
            model.OtherReportPlans =
                _parameters.TLDSService.GetTLDSOtherReportPlanOfProfile(model.ProfileId)
                    .Select(x => new TLDSOtherReportPlanViewModel()
                    {
                        OtherReportPlanId = x.OtherReportPlanID,
                        ReportName = x.ReportName,
                        ReportDate = x.ReportDate,
                        ReportDateFormated = ParseDateWithFormat(x.ReportDate),
                        AvailableOnRequest = x.AvailableOnRequest
                    }).ToList();

        }

        private void BuildTldsParentGuardian(TldsProfileDigitalViewModel model)
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

        private void BuildTldsProfessionalService(TldsProfileDigitalViewModel model)
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
                        ReportForwardedToSchoolDateFormated = ParseDateWithFormat(x.ReportForwardedToSchoolDate),
                        Attached = x.Attached,
                        AvailableUponRequested = x.AvailableUponRequested,
                    }).ToList();
        }

        private TldsFormSection2ViewModel BuildTldsSection2(Guid id)
        {
            var section2 = new TldsFormSection2ViewModel();

            var tldsProfile = _parameters.TLDSDigitalSection23Service.GetProfile(id);
            if (tldsProfile != null)
            {
                ViewBag.ChildName = string.Join(" ", tldsProfile.FirstName, tldsProfile.LastName);
            }

            var childForm = _parameters.TLDSDigitalSection23Service.GetFormSections2(id);
            if (childForm != null)
            {
                section2.TLDSFormSection2ID = childForm.TLDSFormSection2ID;
                section2.TLDSProfileLinkID = childForm.TLDSProfileLinkID;
                section2.GuardianName = childForm.GuardianName;
                section2.Relationship = childForm.Relationship;
                section2.Favourite = childForm.Favourite;
                section2.Strengths = childForm.Strengths;
                section2.Weaknesses = childForm.Weaknesses;
                section2.Interested = childForm.Interested;
                section2.Expected = childForm.Expected;
                section2.Drawing = GetLinkToDownloadChildPhoto(childForm.Drawing);
            }

            return section2;
        }

        private TldsFormSection3ViewModel BuildTldsSection3(Guid id)
        {
            var section3 = new TldsFormSection3ViewModel();

            var familyForm = _parameters.TLDSDigitalSection23Service.GetFormSections3(id);
            if (familyForm == null)
            {
                section3.IsSubmitted = false;
                return section3;
            }
            section3.TLDSFormSection3ID = familyForm.TLDSFormSection3ID;
            section3.TLDSProfileLinkID = familyForm.TLDSProfileLinkID;
            section3.GuardianName = familyForm.GuardianName;
            section3.Relationship = familyForm.Relationship;
            section3.PreferredLanguage = familyForm.PreferredLanguage;
            if (familyForm.IsAborigial.HasValue)
                section3.IsAborigial = familyForm.IsAborigial.Value;
            else
                section3.IsAborigial = null;
            if (familyForm.HaveSiblingInSchool.HasValue)
                section3.HaveSiblingInSchool = familyForm.HaveSiblingInSchool.Value;
            else
                section3.HaveSiblingInSchool = null;
            section3.NameAndGradeOfSibling = familyForm.NameAndGradeOfSibling;
            section3.Wishes = familyForm.Wishes;
            section3.InformationSchool = familyForm.InformationSchool;
            section3.HelpInformation = familyForm.HelpInformation;
            section3.Interested = familyForm.Interested;
            section3.ConditionImprovement = familyForm.ConditionImprovement;
            section3.OtherInformation = familyForm.OtherInformation;

            return section3;
        }

        private string ReplaceLineBreak(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            string replaceStr = str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "<br/>");

            return replaceStr;
        }

        private string GetLinkToDownloadChildPhoto(string fileName)
        {
            var s3Url = _s3Service.GetPublicUrl(LinkitConfigurationManager.GetS3Settings().TLDSBucket, fileName);
            return s3Url;
        }

        private bool TryParseDateWithFormat(string dateInString, out DateTime result)
        {
            //var dateFormatCookie = System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultDateFormat];
            CultureInfo provider = CultureInfo.InvariantCulture;
            if (DateTime.TryParseExact(dateInString, TextConstants.TLDS_DATE_FORMAT_LOGIN, provider, DateTimeStyles.None, out result))
            {
                return true;
            }
            else
            {
                return false;

            }
        }

        private string ParseDateWithFormat(DateTime? dateTime)
        {
            return dateTime.GetValueOrDefault().DisplayDateWithFormat(TextConstants.DEFAULT_DATE_FORMAT_AU, string.Empty, false);
        }
    }
}
