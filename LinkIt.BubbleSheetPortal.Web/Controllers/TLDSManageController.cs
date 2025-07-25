using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using System.Web.Script.Serialization;
using System.Web;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using S3Library;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.TLDS;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Models.TLDS;
using LinkIt.BubbleSheetPortal.Models.DTOs.TLDS;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Web.ViewModels.TLDSDigital;
using DevExpress.Utils;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize(Order = 1)]
    [VersionFilter]
    public class TLDSManageController : BaseController
    {
        private readonly TDLSManageControllerParameters _parameters;
        private readonly TLDSDigitalSection23ControllerParameters _tldsDigitalSection23Parameters;
        private readonly ConfigurationService _configurationService;

        private readonly IS3Service _s3Service;

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

        private List<SelectListItem> _qualificationList;

        private List<SelectListItem> _profileTeacherSelectedList;

        private List<SelectListItem> QualificationList
        {
            get
            {
                if (_qualificationList == null)
                {
                    _qualificationList = _parameters.TLDSService.GetTldsLevelQualifications().Select(x => new SelectListItem()
                    {
                        Value = x.TLDSLevelQualificationID.ToString(),
                        Text = x.Name
                    }).ToList();
                    _qualificationList.Insert(0, new SelectListItem() { Value = "0", Text = string.Empty });
                }
                return _qualificationList;
            }
        }

        private List<SelectListItem> ProfileTeacherSelectedList
        {
            get
            {
                if (_profileTeacherSelectedList == null)
                {
                    var userMeta = _parameters.TLDSService.GetTLDSUserMetaByUserId(CurrentUser.Id);
                    if (userMeta != null)
                    {
                        _profileTeacherSelectedList = _parameters.TLDSService.GetAllTldsProfileTeachersByUserMetaID(userMeta.TLDSUserMetaID)
                                                            .Select(x => new SelectListItem
                                                            {
                                                                Value = x.TLDSProfileTeacherID.ToString(),
                                                                Text = x.EducatorName
                                                            }).ToList();
                        _profileTeacherSelectedList.Insert(0, new SelectListItem() { Value = "0", Text = string.Empty });
                    }
                    else
                    {
                        _profileTeacherSelectedList = new List<SelectListItem>();
                    }
                }
                return _profileTeacherSelectedList;
            }
        }

        public TLDSManageController(TDLSManageControllerParameters parameters, TLDSDigitalSection23ControllerParameters tldsDigitalSection23ControllerParameters,
            ConfigurationService configurationService, IS3Service s3Service)
        {
            _parameters = parameters;
            _tldsDigitalSection23Parameters = tldsDigitalSection23ControllerParameters;
            _configurationService = configurationService;
            _s3Service = s3Service;
        }

        #region TLDS Home

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ReportItemTLDSManager)]
        public ActionResult Index()
        {
            var right = AccessRightEnum.View;
            var hasRightToCreate = _parameters.VulnerabilityService.HasRightToCreateTLDSProfile(CurrentUser);
            if (hasRightToCreate)
            {
                right = AccessRightEnum.Create;
            }
            ViewBag.IsSchoolAdmin = CurrentUser.IsSchoolAdmin;
            ViewBag.IsTeacher = CurrentUser.IsTeacher;
            ViewBag.EnrolmentYear = (DateTime.UtcNow.Year + 1);
            return View(right);

        }

        public ActionResult GetTDLSProfile(TLDSFilterParameter p)
        {
            var parser = new DataTableParser<TDLSProfileCustomViewModel>();
            var tdlsProfiles = _parameters.TLDSService.FilterTLDSProfile(CurrentUser.Id, p)
                    .Select(x => new TDLSProfileCustomViewModel
                    {
                        ProfileId = x.ProfileID,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        ECSCompletingFormEducatorName = x.ECSCompletingFormEducatorName,
                        Status = x.Status,
                        LastStatusDate = x.LastStatusDate,
                        School = x.SchoolName,
                        Viewable = x.Viewable,
                        Updateable = x.Updateable,
                        EnrolmentYear = x.EnrolmentYear,
                        OnlyView = CheckTLDSOnlyView(x.EnrolmentYear.GetValueOrDefault())
                    });

            return Json(parser.Parse(tdlsProfiles.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        private bool CheckTLDSOnlyView(int enrolmentYear)
        {
            if (!CurrentUser.IsTeacher) return false;  //check role teacher

            //compare enrolmentYear with current year
            if (enrolmentYear < DateTime.UtcNow.Year)
                return true;
            //compare between enrolmentYear and "TLDSReadOnly" in Configuration table
            if (enrolmentYear == DateTime.UtcNow.Year &&
                EnrolmentYearConfig != null &&
                !string.IsNullOrEmpty(EnrolmentYearConfig.Value))
            {
                DateTime dt = DateTime.ParseExact(EnrolmentYearConfig.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (enrolmentYear <= dt.Year)
                {
                    if (DateTime.UtcNow.Month == dt.Month && DateTime.UtcNow.Day >= dt.Day)
                    {
                        return true;
                    }
                    if (DateTime.UtcNow.Month > dt.Month)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [UploadifyPrincipal(Order = 1)]
        public ActionResult UploadChildPhoto(HttpPostedFileBase postedFile, int profileId)
        {
            if (profileId == 0)
            {
                if (!_parameters.VulnerabilityService.HasRightToCreateTLDSProfile(CurrentUser))
                {
                    return Json(new { success = false, ErrorMessage = "Has no right" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var profile = _parameters.TLDSService.GetTLDSProfile(profileId);
                if (!_parameters.VulnerabilityService.HasRightToUpdateTLDSProfile(CurrentUser, profile.UserID ?? 0,
                    CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { success = false, ErrorMessage = "Has no right" }, JsonRequestBehavior.AllowGet);
                }

            }
            var tldsS3Setting = TLDSS3Settings.GetTLDSS3Settings(profileId);

            try
            {
                var fileName = postedFile.FileName.AddTimestampToFileName();
                string photoFileName = tldsS3Setting.GetPhotoPath(fileName);
                var s3Result = _s3Service.UploadRubricFile(tldsS3Setting.TLDSBucket, photoFileName, postedFile.InputStream, false);
                if (s3Result.IsSuccess)
                {
                    return Json(new { Success = true, FileName = fileName, fileNameUrl = GetLinkToDownloadChildPhoto(photoFileName) }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Success = false, ErrorMessage = s3Result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult UploadChildPhotoEnhancement(string imageSrc, int profileId, string fileName)
        {
            if (profileId == 0)
            {
                if (!_parameters.VulnerabilityService.HasRightToCreateTLDSProfile(CurrentUser))
                {
                    return Json(new { success = false, ErrorMessage = "Has no right" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var profile = _parameters.TLDSService.GetTLDSProfile(profileId);
                if (!_parameters.VulnerabilityService.HasRightToUpdateTLDSProfile(CurrentUser, profile.UserID ?? 0,
                    CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { success = false, ErrorMessage = "Has no right" }, JsonRequestBehavior.AllowGet);
                }

            }
            var tldsS3Setting = TLDSS3Settings.GetTLDSS3Settings(profileId);

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
                                    Success = true,
                                    FileName = fileName,
                                    fileNameUrl = GetLinkToDownloadChildPhoto(photoFileName)
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
        public string GetLinkToDownloadChildPhoto(string fileName)
        {
            var s3Url = _s3Service.GetPublicUrl(LinkitConfigurationManager.GetS3Settings().TLDSBucket, fileName);
            return s3Url;
        }

        public string GetLinkToDownloadUploadFile(string fileName)
        {
            var s3Url = _s3Service.GetPublicUrl(LinkitConfigurationManager.GetS3Settings().TLDSBucket,
               fileName);
            return s3Url;
        }

        #endregion

        #region TLDS Section 0

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ReportItemTLDSManager)]
        public ActionResult Edit(int? profileId)
        {
            var model = new TDLSProfileViewModel();
            model.Genders = _parameters.GenderService.GetAllGenders()
                    .Select(x => new SelectListItem { Text = x.Name, Value = x.GenderID.ToString(CultureInfo.InvariantCulture) }).ToList();
            model.QualificationList = QualificationList;
            model.TLDSProfileTeacherSelectedItem = ProfileTeacherSelectedList;
            model.TLDSGroupSelectedItem = GetGroupSelectedList();

            if (!profileId.HasValue || profileId.Value == 0)
            {
                model.ProfileId = 0;
                model.AccessRight = AccessRightEnum.Create;
                LoadConfigurationToSection0Model(model);

                return View(model);
            }
            else
            {
                var profile = _parameters.TLDSService.GetTLDSProfile(profileId.Value);
                if (profile == null)
                {
                    return RedirectToAction("Index", "TLDSManage");
                }

                model.AccessRight = CheckPermissionToAccessProfile(profile);
                if (model.AccessRight == AccessRightEnum.NoRight)
                {
                    return RedirectToAction("Index", "TLDSManage");
                }

                var tldsS3Settins = TLDSS3Settings.GetTLDSS3Settings(profile.ProfileId);

                model.ProfileId = profile.ProfileId;
                model.TLDSInformationHasBeenSaved = profile.TLDSInformationHasBeenSaved;
                model.Section102HasBeenSaved = profile.Section102HasBeenSaved;
                model.FirstName = profile.FirstName;
                model.LastName = profile.LastName;
                model.DateOfBirth = profile.DateOfBirth;
                model.GenderId = profile.GenderId ?? 0;
                model.PrimarySchool = profile.PrimarySchool;
                model.OutsideSchoolHoursCareService = profile.OutsideSchoolHoursCareService;
                model.PhotoURL = GetLinkToDownloadChildPhoto(tldsS3Settins.GetPhotoPath(profile.PhotoURL));
                model.FileName = profile.PhotoURL;
                model.ECSName = profile.ECSName;
                model.ECSAddress = profile.ECSAddress;
                model.ECSApprovalNumber = profile.ECSApprovalNumber;
                model.ECSCompletingFormEducatorName = profile.ECSCompletingFormEducatorName;
                model.ECSCompletingFormEducatorPosition = profile.ECSCompletingFormEducatorPosition;
                model.ECSCompletingFormEducatorPhone = profile.ECSCompletingFormEducatorPhone;
                model.ECSCompletingFormEducatorEmail = profile.ECSCompletingFormEducatorEmail;
                model.ECSCompledDate = profile.ECSCompledDate;
                model.Status = profile.Status ?? 0;
                model.ECSCompletingFormEducatorQualificationId = profile.ECSCompletingFormEducatorQualificationId ?? 0;
                model.DevelopmentOutcomeHasBeenSaved = profile.DevelopmentOutcomeHasBeenSaved;
                model.Section102HasBeenSaved = profile.Section102HasBeenSaved;
                model.ContextSpecificInforHasBeenSaved = profile.ContextSpecificInforHasBeenSaved;
                model.SectionChildParentCompleted = profile.SectionChildParentCompleted;
                model.TldsGroupId = profile.TldsGroupId;
                //the first time -> load some fields from configurations
                if (!model.TLDSInformationHasBeenSaved)
                {
                    LoadConfigurationToSection0Model(model);
                }

                model.EnrolmentYear = profile.EnrolmentYear;
            }
            return View(model);
        }

        private void LoadConfigurationToSection0Model(TDLSProfileViewModel model)
        {
            var tldsUserMeta = _parameters.TLDSService.GetTLDSUserMetaByUserId(CurrentUser.Id);
            TLDSUserMetaValueModel userMetaModel = null;
            if (tldsUserMeta != null)
            {
                userMetaModel = TLDSUserMetaValueModel.ParseFromJsonData(tldsUserMeta.MetaValue);
            }
            //int some value from configuration
            if (tldsUserMeta != null)
            {
                if (userMetaModel != null && userMetaModel.TLDSUserConfigurations != null &&
                    userMetaModel.TLDSUserConfigurations.EarlyChildHoodServiceConfiguration != null)
                {
                    var earlyChildHoodServiceConfiguration =
                    userMetaModel.TLDSUserConfigurations.EarlyChildHoodServiceConfiguration;
                    model.ECSName = earlyChildHoodServiceConfiguration.NameOfService;
                    model.ECSAddress = earlyChildHoodServiceConfiguration.AddressOfService;
                    model.ECSApprovalNumber = earlyChildHoodServiceConfiguration.ServiceApprovalNumber;
                    model.ECSCompletingFormEducatorPhone = earlyChildHoodServiceConfiguration.Phone;
                    model.ECSCompletingFormEducatorEmail = earlyChildHoodServiceConfiguration.Email;
                }
            }
            if (tldsUserMeta != null && userMetaModel != null && userMetaModel.TLDSUserConfigurations != null)
            {
                model.TLDSUserConfigurations = userMetaModel.TLDSUserConfigurations;
            }
            else
            {
                model.TLDSUserConfigurations = new TLDSUserConfigurations();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TDLSProfileViewModel model)
        {
            if (!_parameters.VulnerabilityService.HasRightToCreateTLDSProfile(CurrentUser))
            {
                return RedirectToAction("Index", "TLDSManage");//has no right
            }

            var tldsProfile = _parameters.TLDSService.GetTLDSProfile(model.ProfileId);
            bool hasCreatedNew = false;

            if (tldsProfile == null)
            {
                tldsProfile = new TLDSProfile();
                tldsProfile.ProfileId = 0;
                tldsProfile.DateCreated = DateTime.UtcNow;
                tldsProfile.Status = (int)TLDSProfileStatusEnum.Draft;
                tldsProfile.LastStatusDate = DateTime.UtcNow;
                tldsProfile.UserID = CurrentUser.Id;
                hasCreatedNew = true;
                tldsProfile.Section102IsNotRequired = true;
            }

            model.AccessRight = AccessRightEnum.NoRight;
            model.Status = tldsProfile.Status.Value;
            //check the security first
            if (tldsProfile.Status == (int)TLDSProfileStatusEnum.SubmittedToSchool || tldsProfile.Status == (int)TLDSProfileStatusEnum.CreatedUnsubmitted
                || tldsProfile.Status == (int)TLDSProfileStatusEnum.AssociatedWithStudent)
            {
                model.AccessRight = AccessRightEnum.View;
            }
            else if (_parameters.VulnerabilityService.HasRightToUpdateTLDSProfile(CurrentUser,
                tldsProfile.UserID ?? 0, CurrentUser.GetMemberListDistrictId()))
            {
                model.AccessRight = AccessRightEnum.Update;
            }
            if (model.AccessRight != AccessRightEnum.Update)
            {
                return RedirectToAction("Index", "TLDSManage");//has no right
            }
            //check required fields
            model.ErrorList = new List<string>();
            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                model.ErrorList.Add("Child's first name is required.");
            }
            if (string.IsNullOrWhiteSpace(model.LastName))
            {
                model.ErrorList.Add("Surname  is required.");
            }
            if (string.IsNullOrWhiteSpace(model.DateOfBirthString))
            {
                model.ErrorList.Add("Child's date of birth is required.");
            }
            if (!model.EnrolmentYear.HasValue)
            {
                model.ErrorList.Add("Year of enrolment is required.");
            }
            if (string.IsNullOrWhiteSpace(model.ECSName))
            {
                model.ErrorList.Add("Name of service is required.");
            }
            if (string.IsNullOrWhiteSpace(model.ECSAddress))
            {
                model.ErrorList.Add("Address of service is required.");
            }
            if (string.IsNullOrWhiteSpace(model.ECSApprovalNumber))
            {
                model.ErrorList.Add("Service approval number is required.");
            }
            if (string.IsNullOrWhiteSpace(model.ECSCompletingFormEducatorPhone))
            {
                model.ErrorList.Add("Phone is required.");
            }
            if (string.IsNullOrWhiteSpace(model.ECSCompletingFormEducatorEmail))
            {
                model.ErrorList.Add("Email is required.");
            }

            model.Genders = _parameters.GenderService.GetAllGenders()
                .Select(x => new SelectListItem { Text = x.Name, Value = x.GenderID.ToString(CultureInfo.InvariantCulture) }).ToList();
            model.QualificationList = QualificationList;
            model.DevelopmentOutcomeHasBeenSaved = tldsProfile.DevelopmentOutcomeHasBeenSaved;
            model.Section102HasBeenSaved = tldsProfile.Section102HasBeenSaved;
            //if there's any error -> no save, return
            if (model.ErrorList.Count > 0)
            {
                return View(model);
            }

            UpdateTLDSProfileSection0FromForm(tldsProfile, model);

            tldsProfile.TLDSInformationHasBeenSaved = true;
            tldsProfile.ECSCompletingFormEducatorName = model.ECSCompletingFormEducatorName;
            //save to database
            tldsProfile.DateUpdated = DateTime.UtcNow;
            tldsProfile.ECSCompledDate = DateTime.UtcNow;

            var tldsS3Setting = TLDSS3Settings.GetTLDSS3Settings(tldsProfile.ProfileId);
            var newPhotoName = string.Empty;

            if ((hasCreatedNew && !string.IsNullOrEmpty(model.FileName)))
            {
                _parameters.TLDSService.SaveTLDSProfile(tldsProfile);
                newPhotoName = CopyPhotoFile(model.FileName, tldsProfile.ProfileId, model.RotatePhoto);
                model.PhotoURL = GetLinkToDownloadChildPhoto(tldsS3Setting.GetPhotoPath(newPhotoName));
            }
            else if (model.RotatePhoto != 0)
            {
                newPhotoName = RotatePhoto(model.ProfileId, model.FileName, model.RotatePhoto);
                model.PhotoURL = GetLinkToDownloadChildPhoto(tldsS3Setting.GetPhotoPath(newPhotoName));
            }

            if (!string.IsNullOrEmpty(newPhotoName))
            {
                tldsProfile.PhotoURL = newPhotoName;
            }

            _parameters.TLDSService.SaveTLDSProfile(tldsProfile);

            model.SaveSuccessful = true;
            model.ProfileId = tldsProfile.ProfileId;
            model.Status = tldsProfile.Status.Value;
            model.DevelopmentOutcomeHasBeenSaved = tldsProfile.DevelopmentOutcomeHasBeenSaved;
            model.Section102HasBeenSaved = tldsProfile.Section102HasBeenSaved;
            var js = new JavaScriptSerializer();
            var guardiantContactData =
                js.Deserialize<List<TLDSGuardiantContactDetailViewModel>>(model.GuardiantContactJSONData);
            //save Guardiant
            if (guardiantContactData != null)
            {
                var guardiantContactList = guardiantContactData.Select(x => new TLDSParentGuardian()
                {
                    TLDSParentGuardianID = x.GuardiantContactDetailId ?? 0,
                    TLDSProfileID = model.ProfileId,
                    ParentGuardianName = x.FullName,
                    ParentGuardianRelationship = x.RelationshipToChild,
                    ParentGuardianPhone = x.Phone,
                    ParentGuardianEmail = x.Email
                }).ToList();
                _parameters.TLDSService.SaveParentGuardian(model.ProfileId, guardiantContactList);
            }
            if (model.IsContinue && model.ErrorList.Count == 0)
            {
                return RedirectToAction("ContextSpecificInfor", "TLDSManage", new { profileId = model.ProfileId });
            }

            if (model.AutoSaving)
            {
                return Json(new { Success = true, tldsProfileId = tldsProfile.ProfileId }, JsonRequestBehavior.AllowGet);
            }

            return RedirectToAction("Edit", "TLDSManage", new { profileId = model.ProfileId });
        }

        private string RotatePhoto(int profileId, string fileName, int rotate)
        {
            var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(profileId);
            var photoChildPath = tldsS3Settings.GetPhotoPath(fileName);
            var result = _s3Service.DownloadFile(tldsS3Settings.TLDSBucket, photoChildPath);
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

                var newPhotoChildPath = tldsS3Settings.GetPhotoPath(newName);
                var stream = Util.RotateImage(result.ReturnStream, rotate);
                var s3Result = _s3Service.UploadRubricFile(tldsS3Settings.TLDSBucket, newPhotoChildPath, stream, false);
                if (s3Result.IsSuccess)
                {
                    try
                    {
                        //delete unsued file
                        _s3Service.DeleteFile(tldsS3Settings.TLDSBucket, photoChildPath);
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

        private string CopyPhotoFile(string photoFileName, int profileId, int rotate = 0)
        {
            var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(0);
            //download photo child from folder 0
            var photoChildPath = tldsS3Settings.GetPhotoPath(photoFileName);
            var result = _s3Service.DownloadFile(tldsS3Settings.TLDSBucket, photoChildPath);
            if (result.IsSuccess)
            {
                //upload
                tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(profileId);
                var newPhotoChildPath = tldsS3Settings.GetPhotoPath(photoFileName);
                MemoryStream stream = new MemoryStream(result.ReturnStream);
                if (rotate != 0)
                {
                    stream = Util.RotateImage(result.ReturnStream, rotate);
                }
                var s3Result = _s3Service.UploadRubricFile(tldsS3Settings.TLDSBucket, newPhotoChildPath, stream,
                    false);
                if (s3Result.IsSuccess)
                {
                    try
                    {
                        //delete unsued file
                        _s3Service.DeleteFile(tldsS3Settings.TLDSBucket, photoChildPath);
                    }
                    catch
                    {
                        //nothing 
                    }
                    return photoFileName;
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
        public ActionResult GetGuardianContactDetail(int profileId)
        {
            var guardiantContactDetails = new List<TLDSGuardiantContactDetailViewModel>();
            var parentGuardianOfProfile = _parameters.TLDSService.GetTLDSParentGuardianOfProfile(profileId);
            if (parentGuardianOfProfile != null && parentGuardianOfProfile.Count > 0)
            {
                //load existing
                guardiantContactDetails.AddRange(parentGuardianOfProfile.Select(x => new TLDSGuardiantContactDetailViewModel()
                {
                    GuardiantContactDetailId = x.TLDSParentGuardianID,
                    ProfileId = x.TLDSProfileID,
                    FullName = x.ParentGuardianName,
                    RelationshipToChild = x.ParentGuardianRelationship,
                    Phone = x.ParentGuardianPhone,
                    Email = x.ParentGuardianEmail
                }).ToList());
            }
            if (guardiantContactDetails.Count < 2)
            {
                for (int i = guardiantContactDetails.Count; i <= 2; i++)
                {
                    guardiantContactDetails.Add(new TLDSGuardiantContactDetailViewModel());//AU needs the list be constructed with at least two rows
                }
            }

            return Json(new { guardiantContactDetails }, JsonRequestBehavior.AllowGet);
        }

        private List<SelectListItem> GetGroupSelectedList()
        {
            List<SelectListItem> listGroups = new List<SelectListItem>();
            var userMeta = _parameters.TLDSService.GetTLDSUserMetaByUserId(CurrentUser.Id);
            if (userMeta != null)
            {
                listGroups = _parameters.TLDSService.GetAllTldsGroupByUserMetaID(userMeta.TLDSUserMetaID)
                                                    .Where(x => x.Status == true)
                                                    .Select(o => new SelectListItem
                                                    {
                                                        Value = o.TLDSGroupID.ToString(),
                                                        Text = o.GroupName
                                                    }).ToList();
                if (listGroups.Count > 0)
                    listGroups.Insert(0, new SelectListItem() { Value = "0", Text = TextConstants.SELECT_GROUP });
            }
            return listGroups;
        }

        public ActionResult GetProfileTeacherById(int tldsProfileTeacherId)
        {
            var profileTeacher = _parameters.TLDSService.GetProfileTeacherById(tldsProfileTeacherId);
            return Json(profileTeacher, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region TLDS Section 1
        [HttpGet]
        public ActionResult ContextSpecificInfor(int? profileId)
        {
            var tldsProfile = _parameters.TLDSService.GetTLDSProfile(profileId ?? 0);
            if (tldsProfile == null)
            {
                return RedirectToAction("Index", "TLDSManage");
            }
            var model = new TDLSProfileViewModel()
            {
                ProfileId = tldsProfile.ProfileId,
                Status = tldsProfile.Status.Value
            };
            model.AccessRight = CheckPermissionToAccessProfile(tldsProfile);
            if (model.AccessRight == AccessRightEnum.NoRight)
            {
                return RedirectToAction("Index", "TLDSManage");
            }

            //if it's the first time ContextOfEarlyYearsSetting is shown
            //init it with configuration value
            if (!tldsProfile.ContextSpecificInforHasBeenSaved
                && string.IsNullOrEmpty(tldsProfile.ContextOfEarlyYearsSetting))
            {
                var tldsUserMeta = _parameters.TLDSService.GetTLDSUserMetaByUserId(CurrentUser.Id);
                if (tldsUserMeta != null)
                {
                    var userMetaModel = TLDSUserMetaValueModel.ParseFromJsonData(tldsUserMeta.MetaValue);
                    if (userMetaModel != null)
                    {
                        var userTLDSUserConfigurations = userMetaModel.TLDSUserConfigurations;
                        if (userTLDSUserConfigurations != null)
                        {
                            tldsProfile.ContextOfEarlyYearsSetting =
                                userTLDSUserConfigurations.ContextSpecificConfiguration.ContextOfEarlyYearsSetting;

                        }
                    }
                }
            }

            //assign some properties used on this form
            model.ContextOfEarlyYearsSetting = tldsProfile.ContextOfEarlyYearsSetting;
            model.SpecificInformation = tldsProfile.SpecificInformation;
            model.ContextSpecificInforHasBeenSaved = tldsProfile.ContextSpecificInforHasBeenSaved;
            model.DevelopmentOutcomeHasBeenSaved = tldsProfile.DevelopmentOutcomeHasBeenSaved;
            model.Section102HasBeenSaved = tldsProfile.Section102HasBeenSaved;
            model.SectionChildParentCompleted = tldsProfile.SectionChildParentCompleted;
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ContextSpecificInfor(TDLSProfileViewModel model)
        {
            //Save new value
            var tldsProfile = _parameters.TLDSService.GetTLDSProfile(model.ProfileId);
            model.AccessRight = AccessRightEnum.NoRight;
            //check the security first
            if (tldsProfile.Status == (int)TLDSProfileStatusEnum.SubmittedToSchool || tldsProfile.Status == (int)TLDSProfileStatusEnum.CreatedUnsubmitted
                || tldsProfile.Status == (int)TLDSProfileStatusEnum.AssociatedWithStudent)
            {
                model.AccessRight = AccessRightEnum.View;
            }
            else if (_parameters.VulnerabilityService.HasRightToUpdateTLDSProfile(CurrentUser,
                tldsProfile.UserID ?? 0, CurrentUser.GetMemberListDistrictId()))
            {
                model.AccessRight = AccessRightEnum.Update;
            }
            if (model.AccessRight != AccessRightEnum.Update)
            {
                return RedirectToAction("Index", "TLDSManage");//has no right
            }

            tldsProfile.ContextOfEarlyYearsSetting = model.ContextOfEarlyYearsSetting;
            tldsProfile.SpecificInformation = model.SpecificInformation;
            tldsProfile.ContextSpecificInforHasBeenSaved = true;
            tldsProfile.DateUpdated = DateTime.UtcNow;
            tldsProfile.ECSCompledDate = DateTime.UtcNow;

            _parameters.TLDSService.SaveTLDSProfile(tldsProfile);
            if (model.IsContinue)
            {
                return RedirectToAction("DevelopmentOutcome", "TLDSManage", new { profileId = model.ProfileId });
            }

            model.SaveSuccessful = true;
            model.DevelopmentOutcomeHasBeenSaved = tldsProfile.DevelopmentOutcomeHasBeenSaved;
            model.Section102HasBeenSaved = tldsProfile.Section102HasBeenSaved;
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion TLDS Section 1
        #region TLDS Section 1.1
        public ActionResult DevelopmentOutcome(int? profileId)
        {
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId ?? 0);
            if (profile == null)
            {
                return RedirectToAction("Index", "TLDSManage");
            }
            var model = new TDLSProfileViewModel()
            {
                ProfileId = profile.ProfileId,
                Status = profile.Status.Value,
                DevelopmentOutcomeHasBeenSaved = profile.DevelopmentOutcomeHasBeenSaved,
                Section102HasBeenSaved = profile.Section102HasBeenSaved,
                HasEYALT = false
            };
            model.AccessRight = CheckPermissionToAccessProfile(profile);
            if (model.AccessRight == AccessRightEnum.NoRight)
            {
                return RedirectToAction("Index", "TLDSManage");
            }
            model.SectionChildParentCompleted = profile.SectionChildParentCompleted;

            var listDevelopmentOutcome = _parameters.TLDSService.GetDevelopmentOutcomeProfileOfProfile(profile.ProfileId);
            if (listDevelopmentOutcome != null && listDevelopmentOutcome.Any())
            {
                model.HasEYALT = listDevelopmentOutcome.FirstOrDefault(x => x.DevelopmentOutcomeTypeID == 6
                                                                        && !string.IsNullOrEmpty(x.OriginalFileName)
                                                                        && !string.IsNullOrEmpty(x.S3FileName)) != null;
            }

            return View(model);
        }
        private static List<TLDSDevelopmentOutcomeTypeViewModel> dummyDevelopmentOutcomeTypes;
        private List<TLDSDevelopmentOutcomeTypeViewModel> DummyDevelopmentOutcomeTypes
        {
            get
            {
                if (dummyDevelopmentOutcomeTypes == null)
                {
                    dummyDevelopmentOutcomeTypes = CreateDummyDevelopmentOutcomeType();
                }
                return dummyDevelopmentOutcomeTypes;
            }
            set { dummyDevelopmentOutcomeTypes = value; }
        }
        private static List<TLDSDevelopmentOutcomeTypeViewModel> CreateDummyDevelopmentOutcomeType()
        {
            return new List<TLDSDevelopmentOutcomeTypeViewModel>
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
        private static List<TLDSDevelopmentOutcomeProfileViewModel> CreateDummyDevelopmentOutcomeProfile()
        {
            return new List<TLDSDevelopmentOutcomeProfileViewModel>
            {
                new TLDSDevelopmentOutcomeProfileViewModel
                {
                    DevelopmentOutcomeProfileId = 0,
                    ProfileId = 0,
                    DevelopmentOutcomeTypeId = 1,
                    DevelopmentOutcomeTypeName = "IDENTITY",
                },
                new TLDSDevelopmentOutcomeProfileViewModel
                {
                     DevelopmentOutcomeProfileId = 0,
                    ProfileId = 0,
                    DevelopmentOutcomeTypeId = 2,
                    DevelopmentOutcomeTypeName = "COMMUNITY",
                },
                new TLDSDevelopmentOutcomeProfileViewModel
                {
                    DevelopmentOutcomeProfileId = 0,
                    ProfileId = 0,
                    DevelopmentOutcomeTypeId = 3,
                    DevelopmentOutcomeTypeName = "WELLBEING",
                },
                new TLDSDevelopmentOutcomeProfileViewModel
                {
                     DevelopmentOutcomeProfileId = 0,
                    ProfileId = 0,
                    DevelopmentOutcomeTypeId = 4,
                    DevelopmentOutcomeTypeName = "LEARNING",
                },
                new TLDSDevelopmentOutcomeProfileViewModel
                {
                     DevelopmentOutcomeProfileId = 0,
                    ProfileId = 0,
                    DevelopmentOutcomeTypeId = 5,
                    DevelopmentOutcomeTypeName = "COMMUNICATION",
                },
            };
        }
        public ActionResult GetDevelopmentOutcome(int profileId)
        {
            var currentDevelopmentOutcomes = new List<TLDSDevelopmentOutcomeProfileViewModel>();
            var developmentOutcomes = new List<TLDSDevelopmentOutcomeProfileViewModel>();
            var developmentOutcomeProfileList = _parameters.TLDSService.GetDevelopmentOutcomeProfileOfProfile(profileId);
            if (developmentOutcomeProfileList == null || developmentOutcomeProfileList.Count == 0)
            {
                //Init
                developmentOutcomes =
                    DummyDevelopmentOutcomeTypes.Select(x => new TLDSDevelopmentOutcomeProfileViewModel
                    {
                        ProfileId = profileId,
                        DevelopmentOutcomeTypeId = x.DevelopmentOutcomeTypeId,
                        DevelopmentOutcomeTypeName =
                            DummyDevelopmentOutcomeTypes.FirstOrDefault(
                                k => k.DevelopmentOutcomeTypeId == x.DevelopmentOutcomeTypeId).Name,
                        DevelopmentOutcomeProfileId =
                            currentDevelopmentOutcomes.Any(
                                k =>
                                    k.ProfileId == profileId && k.DevelopmentOutcomeTypeId == x.DevelopmentOutcomeTypeId)
                                ? currentDevelopmentOutcomes.FirstOrDefault(
                                    k =>
                                        k.ProfileId == profileId &&
                                        k.DevelopmentOutcomeTypeId == x.DevelopmentOutcomeTypeId)
                                    .DevelopmentOutcomeProfileId
                                : -1,
                        DevelopmentOutcomeContent =
                            currentDevelopmentOutcomes.Any(
                                k =>
                                    k.ProfileId == profileId && k.DevelopmentOutcomeTypeId == x.DevelopmentOutcomeTypeId)
                                ? currentDevelopmentOutcomes.FirstOrDefault(
                                    k =>
                                        k.ProfileId == profileId &&
                                        k.DevelopmentOutcomeTypeId == x.DevelopmentOutcomeTypeId)
                                    .DevelopmentOutcomeContent
                                : "",
                        StrategyContent =
                            currentDevelopmentOutcomes.Any(
                                k =>
                                    k.ProfileId == profileId && k.DevelopmentOutcomeTypeId == x.DevelopmentOutcomeTypeId)
                                ? currentDevelopmentOutcomes.FirstOrDefault(
                                    k =>
                                        k.ProfileId == profileId &&
                                        k.DevelopmentOutcomeTypeId == x.DevelopmentOutcomeTypeId).StrategyContent
                                : "",
                    }).ToList();

                developmentOutcomes.Add(new TLDSDevelopmentOutcomeProfileViewModel
                {
                    ProfileId = profileId,
                    DevelopmentOutcomeTypeId = 6,
                    DevelopmentOutcomeProfileId = -1
                });
            }
            else
            {
                developmentOutcomes = CreateDummyDevelopmentOutcomeProfile();
                foreach (var developmentOutcome in developmentOutcomes)
                {
                    var developmentOutcomeProfile =
                        developmentOutcomeProfileList.FirstOrDefault(
                            x => x.DevelopmentOutcomeTypeID == developmentOutcome.DevelopmentOutcomeTypeId);
                    if (developmentOutcomeProfile != null)
                    {
                        developmentOutcome.DevelopmentOutcomeProfileId =
                            developmentOutcomeProfile.DevelopmentOutcomeProfileID;
                        developmentOutcome.ProfileId =
                            developmentOutcomeProfile.ProfileID;
                        developmentOutcome.DevelopmentOutcomeContent =
                            developmentOutcomeProfile.DevelopmentOutcomeContent;
                        developmentOutcome.StrategyContent =
                            developmentOutcomeProfile.StrategyContent;
                    }
                }

                var eyaltProfile = developmentOutcomeProfileList.FirstOrDefault(x => x.DevelopmentOutcomeTypeID == 6);
                var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(profileId);
                if (eyaltProfile != null)
                {
                    var model = new TLDSDevelopmentOutcomeProfileViewModel
                    {
                        DevelopmentOutcomeProfileId = eyaltProfile.DevelopmentOutcomeProfileID,
                        DevelopmentOutcomeTypeId = 6,
                        ProfileId = eyaltProfile.ProfileID,
                        S3FileName = eyaltProfile.S3FileName,
                        OriginalFileName = eyaltProfile.OriginalFileName
                    };
                    if (!string.IsNullOrEmpty(eyaltProfile.OriginalFileName) && !string.IsNullOrEmpty(eyaltProfile.S3FileName))
                        model.S3Url = GetLinkToDownloadUploadFile(tldsS3Settings.GetEYALTUploadedPath(eyaltProfile.ProfileID, eyaltProfile.S3FileName));
                    developmentOutcomes.Add(model);
                }
            }

            return Json(new { developmentOutcomes }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult SaveDevelopmentOutcome(int profileId, string developmentOutcomeData, bool hasEYALT = false)
        {
            var js = new JavaScriptSerializer();
            var developmentOutcomes =
                js.Deserialize<List<TLDSDevelopmentOutcomeProfileViewModel>>(developmentOutcomeData).ToList();
            if (developmentOutcomes == null)
            {
                developmentOutcomes = new List<TLDSDevelopmentOutcomeProfileViewModel>();
            }
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);
            if (profile == null)
            {
                return RedirectToAction("Index", "TLDSManage");
            }
            //check the security first
            if (!_parameters.VulnerabilityService.HasRightToUpdateTLDSProfile(CurrentUser,
                profile.UserID ?? 0, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            //save tlds profile updating status
            profile.DateUpdated = DateTime.UtcNow;
            profile.ECSCompledDate = DateTime.UtcNow;
            profile.DevelopmentOutcomeHasBeenSaved = hasEYALT || developmentOutcomes.Where(x => x.DevelopmentOutcomeTypeId != 6).All(x => !string.IsNullOrEmpty(x.DevelopmentOutcomeContent) && !string.IsNullOrEmpty(x.StrategyContent));

            _parameters.TLDSService.SaveTLDSProfile(profile);
            //save developing outcome
            var developmentOutcomeList = new List<TLDSDevelopmentOutcomeProfile>(developmentOutcomes == null ? developmentOutcomes.Count : 0);
            developmentOutcomeList = developmentOutcomes.Select(x => new TLDSDevelopmentOutcomeProfile()
            {
                DevelopmentOutcomeProfileID = x.DevelopmentOutcomeProfileId,
                ProfileID = profileId,
                DevelopmentOutcomeTypeID = x.DevelopmentOutcomeTypeId,
                DevelopmentOutcomeContent = x.DevelopmentOutcomeContent,
                StrategyContent = x.StrategyContent,
                S3FileName = x.S3FileName,
                OriginalFileName = x.OriginalFileName
            }).ToList();

            if (!hasEYALT)
                developmentOutcomeList.ForEach(x =>
                {
                    if(x.DevelopmentOutcomeTypeID == 6)
                    {
                        x.S3FileName = string.Empty;
                        x.OriginalFileName = string.Empty;
                    }
                });

            _parameters.TLDSService.SaveTLDSDevelopmentOutcomeProfile(developmentOutcomeList);

            return Json(new { Result = true, profile.DevelopmentOutcomeHasBeenSaved }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region TLDS Section 1.2
        public ActionResult EnhancedTransitions(int? profileId)
        {
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId ?? 0);
            if (profile == null)
            {
                return RedirectToAction("Index", "TLDSManage");
            }

            var model = new TDLSProfileViewModel()
            {
                ProfileId = profile.ProfileId,
                Status = profile.Status.Value,
                Section102IsNotRequired = profile.Section102IsNotRequired ?? false,
                NoKWName = profile.Section102Required ? profile.NoKWName : string.Empty,
                NoKWPosition = profile.Section102Required ? profile.NoKWPosition : string.Empty,
                NoKWPhone = profile.Section102Required ? profile.NoKWPhone : string.Empty,
                NoKWEmail = profile.Section102Required ? profile.NoKWEmail : string.Empty,
                WasAnEarlyABLESReportCompleted = profile.WasAnEarlyABLESReportCompleted,
                DevelopmentOutcomeHasBeenSaved = profile.DevelopmentOutcomeHasBeenSaved,
                Section102HasBeenSaved = profile.Section102HasBeenSaved
            };
            model.DateFormatModel = _parameters.DistrictDecodeService.GetDateFormat(CurrentUser.DistrictId ?? 0);

            model.AccessRight = CheckPermissionToAccessProfile(profile);
            if (model.AccessRight == AccessRightEnum.NoRight)
            {
                return RedirectToAction("Index", "TLDSManage");
            }
            model.SectionChildParentCompleted = profile.SectionChildParentCompleted;
            return View(model);
        }

        public ActionResult GetOtherReportPlan(int profileId)
        {
            var otherReportPlans = new List<TLDSOtherReportPlanViewModel>();
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);

            if (profile.Section102Required)
            {
                otherReportPlans =
                _parameters.TLDSService.GetTLDSOtherReportPlanOfProfile(profileId)
                    .Select(x => new TLDSOtherReportPlanViewModel()
                    {
                        OtherReportPlanId = x.OtherReportPlanID,
                        ProfileId = x.ProfileID,
                        ReportName = x.ReportName,
                        ReportDate = x.ReportDate,
                        AvailableOnRequest = x.AvailableOnRequest
                    }).ToList();
            }

            return Json(new { otherReportPlans }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAdditionalInformation(int profileId)
        {
            var addtionalInformation = new List<TLDSAdditionalInformationViewModel>();

            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);

            if (profile.Section102Required)
            {
                addtionalInformation =
                _parameters.TLDSService.GetTLDSAdditionalInformationOfProfile(profileId)
                    .Select(x => new TLDSAdditionalInformationViewModel()
                    {
                        AdditionalInformationId = x.AdditionalInformationID,
                        ProfileId = x.ProfileID,
                        AreasOfNote = x.AreasOfNote,
                        StrategiesForEnhancedSupport = x.StrategiesForEnhancedSupport,
                        DateCreated = x.DateCreated
                    })
                    .ToList();
            }
            return Json(new { addtionalInformation }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetProfessionalService(int profileId)
        {
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);
            var professionalServices = new List<TLDSProfessionalServiceViewModel>();

            if (profile.Section102Required)
            {
                professionalServices = _parameters.TLDSService.GetTLDSProfessionalServiceOfProfile(profileId)
                   .Select(x => new TLDSProfessionalServiceViewModel()
                   {
                       ProfessionalServiceId = x.ProfessionalServiceID,
                       ProfileId = x.ProfileID,
                       Name = x.Name,
                       Address = x.Address,
                       ContactPerson = x.ContactPerson,
                       Position = x.Position,
                       Phone = x.Phone,
                       Email = x.Email,
                       WrittenReportAvailable = x.WrittenReportAvailable,
                       ReportForwardedToSchoolDate = x.ReportForwardedToSchoolDate,
                       AvailableUponRequested = x.AvailableUponRequested,
                   }).ToList();
            }
            int limitItemCount = 8;// Set default = 8
            if (professionalServices.Count < limitItemCount)
            {
                for (int i = professionalServices.Count; i < limitItemCount; i++)
                {
                    professionalServices.Add(new TLDSProfessionalServiceViewModel());
                }
            }

            return Json(new { professionalServices }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveEnhancedTransition(TDLSProfileViewModel model)
        {
            var js = new JavaScriptSerializer();

            var profressionalServices =
                js.Deserialize<List<TLDSProfessionalServiceViewModel>>(model.ProfessionalServiceData).ToList();

            var additionalInformation =
                js.Deserialize<List<TLDSAdditionalInformationViewModel>>(model.AdditionalInformationData).ToList();

            var otherReportPlans =
                js.Deserialize<List<TLDSOtherReportPlanViewModel>>(model.OtherReportPlanData).ToList();

            var earlyABLESReports =
               js.Deserialize<List<TLDSEarlyABLESReportViewModel>>(model.TLDSEarlyABLESReportData).ToList();


            var profile = _parameters.TLDSService.GetTLDSProfile(model.ProfileId);
            if (profile == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            if (!_parameters.VulnerabilityService.HasRightToUpdateTLDSProfile(CurrentUser,
                profile.UserID ?? 0, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var sectionIsNotRequired = model.Section102IsNotRequired.HasValue && model.Section102IsNotRequired.Value;
            if (!sectionIsNotRequired)
            {
                SaveTLDSProfessionalService(model.ProfileId, profressionalServices);
                SaveTLDSAdditionalInformation(model.ProfileId, additionalInformation);
                SaveOtherReportPlans(model.ProfileId, otherReportPlans);
                if (model.WasAnEarlyABLESReportCompleted.HasValue && model.WasAnEarlyABLESReportCompleted.Value)
                {
                    SaveTLDSEarlyABLESReport(model.ProfileId, earlyABLESReports);
                }

                profile.NoKWName = string.IsNullOrEmpty(model.NoKWName) ? profile.NoKWName : model.NoKWName;
                profile.NoKWPhone = string.IsNullOrEmpty(model.NoKWPhone) ? profile.NoKWPhone : model.NoKWPhone;
                profile.NoKWPosition = string.IsNullOrEmpty(model.NoKWPosition) ? profile.NoKWPosition : model.NoKWPosition;
                profile.NoKWEmail = string.IsNullOrEmpty(model.NoKWEmail) ? profile.NoKWEmail : model.NoKWEmail;
                profile.EARReportCompleted = model.EARReportCompleted;
                profile.EARAvailableUponRequest = model.EARAvailableUponRequest;
                profile.EARReportCompleted = model.EARReportCompleted;
                profile.WasAnEarlyABLESReportCompleted = model.WasAnEarlyABLESReportCompleted;
            }
            else
            {
                profile.EARReportCompleted = false;
                profile.EARAvailableUponRequest = false;
                profile.EARReportCompleted = false;
                profile.WasAnEarlyABLESReportCompleted = false;
            }

            profile.Section102IsNotRequired = model.Section102IsNotRequired;
            profile.EARReportDate = model.EARReportDate.GetValueOrDefault();
            profile.Section102HasBeenSaved = true;//mark this section has been saved
            profile.DateUpdated = DateTime.UtcNow;
            profile.ECSCompledDate = DateTime.UtcNow;

            _parameters.TLDSService.SaveTLDSProfile(profile);
            if (model.IsContinue)
            {
                return RedirectToAction("ChildFamily", "TLDSManage", new { profileId = model.ProfileId });
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private void SaveTLDSProfessionalService(int profileId, List<TLDSProfessionalServiceViewModel> professionalServiceViewModel)
        {
            var tldsProfessionalServiceList = professionalServiceViewModel.Select(x => new TLDSProfessionalService()
            {
                ProfessionalServiceID = x.ProfessionalServiceId ?? 0,
                ProfileID = profileId,
                Address = x.Address,
                Attached = x.Attached,
                AvailableUponRequested = x.AvailableUponRequested,
                ContactPerson = x.ContactPerson,
                Email = x.Email,
                Name = x.Name,
                Phone = x.Phone,
                Position = x.Position,
                ReportForwardedToSchoolDate = x.ReportForwardedToSchoolDate,
                WrittenReportAvailable = x.WrittenReportAvailable
            }).ToList();

            if (tldsProfessionalServiceList.Where(x => !x.IsEmpty()).Count() > 0)
            {
                var keepIds = tldsProfessionalServiceList.Where(x => x.ProfessionalServiceID > 0).Select(x => x.ProfessionalServiceID).ToList();
                _parameters.TLDSService.DeleteTldsProfessionalServices(profileId, keepIds);
            }

            _parameters.TLDSService.SaveTLDSProfessionalService(profileId, tldsProfessionalServiceList);
        }

        private void SaveTLDSAdditionalInformation(int profileId, List<TLDSAdditionalInformationViewModel> additionalInformation)
        {
            var additionalInformationList = additionalInformation.Select(x => new TLDSAdditionalInformation()
            {
                AdditionalInformationID = x.AdditionalInformationId ?? 0,
                ProfileID = profileId,
                AreasOfNote = x.AreasOfNote,
                StrategiesForEnhancedSupport = x.StrategiesForEnhancedSupport
            }).ToList();

            if (additionalInformationList.Where(x => !x.IsEmpty()).Count() > 0)
            {
                var keepIds = additionalInformationList.Where(x => x.AdditionalInformationID > 0).Select(x => x.AdditionalInformationID).ToList();
                _parameters.TLDSService.DeleteTldsAdditionalInformations(profileId, keepIds);
            }

            _parameters.TLDSService.SaveTLDSAdditionalInformation(profileId, additionalInformationList);
        }

        private void SaveOtherReportPlans(int profileId,
            List<TLDSOtherReportPlanViewModel> otherReportPlans)
        {
            var otherReportPlanList = otherReportPlans.Select(x => new TLDSOtherReportPlan()
            {
                OtherReportPlanID = x.OtherReportPlanId ?? 0,
                ProfileID = profileId,
                ReportName = x.ReportName,
                ReportDate = x.ReportDate,
                AvailableOnRequest = x.AvailableOnRequest ?? false,
            }).ToList();

            if (otherReportPlanList.Where(x => !x.IsEmpty()).Count() > 0)
            {
                var keepIds = otherReportPlanList.Where(x => x.OtherReportPlanID > 0).Select(x => x.OtherReportPlanID).ToList();
                _parameters.TLDSService.DeleteTldsOtherReportPlans(profileId, keepIds);
            }

            _parameters.TLDSService.SaveTLDSOtherReportPlan(profileId, otherReportPlanList);
        }

        private void SaveTLDSEarlyABLESReport(int profileId,
            List<TLDSEarlyABLESReportViewModel> earlyABLESReports)
        {
            var earlyABLESReportList = earlyABLESReports.Select(x => new TLDSEarlyABLESReport()
            {
                EarlyABLESReportId = x.EarlyABLESReportId ?? 0,
                ProfileId = profileId,
                ReportName = x.ReportName,
                ReportDate = x.ReportDate,
                LearningReadinessReportCompleted = x.LearningReadinessReportCompleted ?? false,
                AvailableOnRequest = x.AvailableOnRequest ?? false
            }).ToList();

            _parameters.TLDSService.SaveTLDSEarlyABLESReport(profileId, earlyABLESReportList);
        }

        private void DeleteTldsEarlyABLESReports(int profileId,
            List<TLDSEarlyABLESReportViewModel> earlyABLESReportsViewModel)
        {
            var earlyABLESReports = earlyABLESReportsViewModel.Select(x => new TLDSEarlyABLESReport()
            {
                EarlyABLESReportId = x.EarlyABLESReportId ?? 0,
                ProfileId = profileId,
                ReportName = x.ReportName,
                ReportDate = x.ReportDate,
                LearningReadinessReportCompleted = x.LearningReadinessReportCompleted ?? false,
                AvailableOnRequest = x.AvailableOnRequest ?? false
            }).ToList();

            _parameters.TLDSService.DeleteTldsEarlyABLESReports(profileId, earlyABLESReports);
        }

        #endregion

        #region TLDS Upcomming School Submit
        public ActionResult UpcomingSchoolSubmit(int? profileId)
        {
            var profile = _parameters.TLDSService.GetTLDSProfileIncludeMeta(profileId ?? 0);
            if (profile == null)
            {
                return RedirectToAction("Index", "TLDSManage");
            }

            var model = new TDLSProfileViewModel()
            {
                ProfileId = profile.ProfileId,
                Status = profile.Status ?? 0,
                UpcommingSchoolID = profile.UpcommingSchoolID,
                WillAttendASchoolInVictoria = profile.WillAttendASchoolInVictoria,
                HasParentSharedStatementWithSchool = profile.HasParentSharedStatementWithSchool,
                SchoolNotListed = profile.SchoolNotListed,
                DevelopmentOutcomeHasBeenSaved = profile.DevelopmentOutcomeHasBeenSaved,
                Section102HasBeenSaved = profile.Section102HasBeenSaved,

                HasProvidedTransitionStatement = profile.HasProvidedTransitionStatement,
                IsAwareTransitionChildSchoolAndOSHC = profile.IsAwareTransitionChildSchoolAndOSHC,
                IsFamilyDidNotCompleteSection2 = profile.IsFamilyDidNotCompleteSection2,
                IsFamilyDidNotCompleteSection3 = profile.IsFamilyDidNotCompleteSection3,
                IsfamilyOptedOutTransitionStatement = profile.IsfamilyOptedOutTransitionStatement,
                SectionChildParentCompleted = profile.SectionChildParentCompleted
            };

            model.AccessRight = CheckPermissionToAccessProfile(profile, true);
            if (model.AccessRight == AccessRightEnum.NoRight)
            {
                return RedirectToAction("Index", "TLDSManage");
            }
            if (model.UpcommingSchoolID.HasValue)
            {
                var school = _parameters.SchoolService.GetSchoolById(model.UpcommingSchoolID.Value);
                model.UpcommingSchoolName = school.Name;
                model.UpcommingDistrictID = school.DistrictId;
            }
            model.IsPublisher = CurrentUser.IsPublisher;
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            model.CurrentDistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            model.ListDistricIds = CurrentUser.IsNetworkAdmin ? CurrentUser.GetMemberListDistrictId() : null;

            return View(model);
        }

        public ActionResult GetSchoolsByDistrict(int? districtId)
        {
            districtId = districtId ?? -1;
            var schools = _parameters.SchoolService.GetTLDSSchoolsByDistrictId(districtId.Value).Select(x => new ListItem
            {
                Id = x.Id,
                Name = x.Name
            }).OrderBy(x => x.Name).ToList();
            return Json(schools, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubmitSchool(int? profileId, int? schoolId, bool parentConsentIsIncluded, bool sectionChildParentCompleted, bool printAllSectionsFamily, char? willAttendASchoolInVictoria, bool? hasParentSharedStatementWithSchool, bool? schoolNotListed, bool? needToSend,
            string hasProvidedTransitionStatement, string isAwareTransitionChildSchoolAndOSHC, string isfamilyOptedOutTransitionStatement)
        {
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId ?? 0);
            if (profile == null)
            {
                return RedirectToAction("Index", "TLDSManage");
            }

            if (profile.Status != (int)TLDSProfileStatusEnum.Draft && profile.Status != (int)TLDSProfileStatusEnum.Recalled
                && profile.Status != (int)TLDSProfileStatusEnum.CreatedUnsubmitted && profile.Status != (int)TLDSProfileStatusEnum.ReturnedBySchool)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            if (!_parameters.VulnerabilityService.HasRightToUpdateTLDSProfile(CurrentUser,
              profile.UserID ?? 0, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            if (profile != null)
            {

                if (needToSend.HasValue && needToSend.Value)
                {
                    if (profile.Status < (int)TLDSProfileStatusEnum.SubmittedToSchool || profile.Status == (int)TLDSProfileStatusEnum.ReturnedBySchool || profile.Status == (int)TLDSProfileStatusEnum.Recalled)
                    {
                        profile.UpcommingSchoolID = schoolId;
                        profile.Status = (int)TLDSProfileStatusEnum.SubmittedToSchool;
                        profile.LastStatusDate = DateTime.UtcNow;
                    }
                    if (!schoolId.HasValue || schoolId.Value == 0)
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    //just Complete but not send
                    if (profile.UpcommingSchoolID != schoolId)
                    {
                        //teacher wants to submit to another school -> change the status
                        profile.Status = (int)TLDSProfileStatusEnum.CreatedUnsubmitted;
                        profile.LastStatusDate = DateTime.UtcNow;
                    }
                    else
                    {
                        //only change the status if the profile is on a lower status
                        if (profile.Status < (int)TLDSProfileStatusEnum.CreatedUnsubmitted || profile.Status == (int)TLDSProfileStatusEnum.ReturnedBySchool || profile.Status == (int)TLDSProfileStatusEnum.Recalled)
                        {
                            profile.Status = (int)TLDSProfileStatusEnum.CreatedUnsubmitted;
                            profile.LastStatusDate = DateTime.UtcNow;
                        }
                    }

                }

                //Security: Check District of the school if the district is a valid receiving TLDS Profile or not
                if (schoolId.HasValue && schoolId.Value > 0)
                {
                    var school = _parameters.SchoolService.GetSchoolById(schoolId.Value);
                    var receivingDistrictIDList = _parameters.DistrictDecodeService.GetReceivingTLDSProfileDistrictID();
                    if (!receivingDistrictIDList.Contains(school.DistrictId))
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }
                profile.ParentConsentIsIncluded = parentConsentIsIncluded;
                profile.PrintAllSectionsFamily = printAllSectionsFamily;
                if (willAttendASchoolInVictoria != null)
                {
                    switch (willAttendASchoolInVictoria.ToString().ToLower())
                    {
                        case "y":
                        case "n":
                        case "u":
                            //valid
                            profile.WillAttendASchoolInVictoria = willAttendASchoolInVictoria;
                            break;
                    }
                }

                profile.HasParentSharedStatementWithSchool = hasParentSharedStatementWithSchool;
                profile.SchoolNotListed = schoolNotListed;
                if (profile.WillAttendASchoolInVictoria.Value.ToString() != "y")
                {
                    profile.SchoolNotListed = false;
                }
                profile.DateUpdated = DateTime.UtcNow;
                _parameters.TLDSService.SaveTLDSProfile(profile);
                if (profile.Status == (int)TLDSProfileStatusEnum.SubmittedToSchool)
                {
                    SendMailSubmitSchool(profile.ProfileId, schoolId.Value);
                }

                profile.HasProvidedTransitionStatement = hasProvidedTransitionStatement;
                profile.IsAwareTransitionChildSchoolAndOSHC = isAwareTransitionChildSchoolAndOSHC;
                profile.IsfamilyOptedOutTransitionStatement = isfamilyOptedOutTransitionStatement;
                SubmitSchoolSaveTLDSProfile(profile);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private void SubmitSchoolSaveTLDSProfile(TLDSProfile profile)
        {
            _parameters.TLDSService.SaveTLDSProfileMeta(profile.ProfileId, nameof(profile.HasProvidedTransitionStatement), profile.HasProvidedTransitionStatement);
            _parameters.TLDSService.SaveTLDSProfileMeta(profile.ProfileId, nameof(profile.IsAwareTransitionChildSchoolAndOSHC), profile.IsAwareTransitionChildSchoolAndOSHC);
            _parameters.TLDSService.SaveTLDSProfileMeta(profile.ProfileId, nameof(profile.IsfamilyOptedOutTransitionStatement), profile.IsfamilyOptedOutTransitionStatement);
        }
        #endregion

        public ActionResult Configuration(int? profileId)
        {
            //Find the configuration of the current user
            TLDSUserConfigurations userTLDSUserConfigurations = null;
            var userMeta = _parameters.TLDSService.GetTLDSUserMetaByUserId(CurrentUser.Id);

            if (userMeta != null)
            {
                //exsing configuration
                var metaModel = TLDSUserMetaValueModel.ParseFromJsonData(userMeta.MetaValue);
                if (metaModel != null && metaModel.TLDSUserConfigurations != null)
                {
                    userTLDSUserConfigurations = metaModel.TLDSUserConfigurations;

                    var tldsGroup = _parameters.TLDSService.GetAllTldsGroupByUserMetaID(userMeta.TLDSUserMetaID);
                    if (tldsGroup.Count > 0)
                        ViewBag.HasTldsGroup = true;
                }
            }
            else
                ViewBag.HasTldsGroup = false;

            if (userTLDSUserConfigurations == null)
            {
                //there's no configuration
                userTLDSUserConfigurations = new TLDSUserConfigurations();
                userTLDSUserConfigurations.EarlyChildHoodServiceConfiguration = new EarlyChildHoodServiceConfiguration();
                userTLDSUserConfigurations.ContextSpecificConfiguration = new ContextSpecificConfiguration();
                //insert a new meta data for current user to make sure the future access would not be null
            }

            return View(userTLDSUserConfigurations);
        }

        public ActionResult HasTldsGroup()
        {
            var userMeta = _parameters.TLDSService.GetTLDSUserMetaByUserId(CurrentUser.Id);

            if (userMeta != null)
            {
                var tldsGroup = _parameters.TLDSService.GetAllTldsGroupByUserMetaID(userMeta.TLDSUserMetaID);
                if (tldsGroup.Count > 0)
                    return Json(new { HasTldsGroup = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { HasTldsGroup = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListGroups()
        {
            var parser = new DataTableParser<TLDSGroupViewModel>();
            var userMeta = _parameters.TLDSService.GetTLDSUserMetaByUserId(CurrentUser.Id);

            if (userMeta == null)
            {
                return Json(parser.Parse2018(new List<TLDSGroupViewModel>().AsQueryable()), JsonRequestBehavior.AllowGet);
            }
            var tldsGroup = _parameters.TLDSService.GetAllTldsGroupByUserMetaID(userMeta.TLDSUserMetaID)
                                                .Select(x => new TLDSGroupViewModel
                                                {
                                                    TLDSGroupID = x.TLDSGroupID,
                                                    GroupName = x.GroupName,
                                                    NumberOfProfile = x.NumberOfProfile,
                                                    Status = x.Status,
                                                    GroupStatus = x.Status ? TextConstants.ACTIVE : TextConstants.INACTIVE
                                                })
                                                .AsQueryable();
            return Json(parser.Parse2018(tldsGroup), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListProfileTeachers()
        {
            var parser = new DataTableParser<TLDSProfileTeacherViewModel>();
            var userMeta = _parameters.TLDSService.GetTLDSUserMetaByUserId(CurrentUser.Id);

            if (userMeta == null)
            {
                return Json(parser.Parse2018(new List<TLDSProfileTeacherViewModel>().AsQueryable()), JsonRequestBehavior.AllowGet);
            }
            var tldsProfileTeacher = _parameters.TLDSService.GetAllTldsProfileTeachersByUserMetaID(userMeta.TLDSUserMetaID)
                                                .Select(x => new TLDSProfileTeacherViewModel
                                                {
                                                    TLDSProfileTeacherID = x.TLDSProfileTeacherID,
                                                    EducatorName = x.EducatorName,
                                                    TLDSLevelQualificationName = x.TLDSLevelQualificationName,
                                                    Position = x.Position
                                                })
                                                .AsQueryable();
            return Json(parser.Parse2018(tldsProfileTeacher), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Configuration(TLDSUserConfigurations model)
        {
            var tldsUserMeta = _parameters.TLDSService.GetTLDSUserMetaByUserId(CurrentUser.Id);
            //save new
            if (tldsUserMeta == null)
            {
                tldsUserMeta = new TLDSUserMeta()
                {
                    UserID = CurrentUser.Id
                };
            }
            var metaModel = TLDSUserMetaValueModel.ParseFromJsonData(tldsUserMeta.MetaValue);
            if (metaModel == null)
            {
                metaModel = new TLDSUserMetaValueModel();
            }
            if (metaModel.TLDSUserConfigurations == null)
            {
                metaModel.TLDSUserConfigurations = new TLDSUserConfigurations();
            }
            var userTLDSUserConfigurations = metaModel.TLDSUserConfigurations;
            userTLDSUserConfigurations.EarlyChildHoodServiceConfiguration.NameOfService =
                model.EarlyChildHoodServiceConfiguration.NameOfService;
            userTLDSUserConfigurations.EarlyChildHoodServiceConfiguration.AddressOfService =
                model.EarlyChildHoodServiceConfiguration.AddressOfService;
            userTLDSUserConfigurations.EarlyChildHoodServiceConfiguration.ServiceApprovalNumber =
                model.EarlyChildHoodServiceConfiguration.ServiceApprovalNumber;
            userTLDSUserConfigurations.EarlyChildHoodServiceConfiguration.Phone =
                model.EarlyChildHoodServiceConfiguration.Phone;
            userTLDSUserConfigurations.EarlyChildHoodServiceConfiguration.Email =
                model.EarlyChildHoodServiceConfiguration.Email;

            userTLDSUserConfigurations.ContextSpecificConfiguration.ContextOfEarlyYearsSetting =
                model.ContextSpecificConfiguration.ContextOfEarlyYearsSetting;

            //save configuration to database
            tldsUserMeta.MetaValue = metaModel.ConvertToJsonData();
            _parameters.TLDSService.SaveTLDSUserMeta(tldsUserMeta);
            ViewBag.HasBeenSavedSuccess = true;
            ViewBag.QualificationList = QualificationList;
            return View(userTLDSUserConfigurations);
        }

        [HttpGet]
        public ActionResult ChildFamily(int profileId)
        {
            var profile = _parameters.TLDSService.GetTLDSProfileIncludeMeta(profileId);
            if (profile == null)
            {
                return RedirectToAction("Index", "TLDSManage");
            }

            var model = new TDLSProfileViewModel()
            {
                ProfileId = profile.ProfileId,
                Status = profile.Status.Value,
                Section2CheckedCompleted = profile.Section2CheckedCompleted ?? false,
                Section3CheckedCompleted = profile.Section3CheckedCompleted ?? false,
                DevelopmentOutcomeHasBeenSaved = profile.DevelopmentOutcomeHasBeenSaved,
                TLDSUploadedDocuments = _parameters.TLDSService.GetTLDSUploadedDocumentByProfileId(profile.ProfileId).OrderBy(x => x.UploadedDate).ToList(),
                AboriginalValue = profile.IsAboriginal,
                HasTheFamilyIndicatedAboriginal = profile.HasTheFamilyIndicatedAboriginal,
                IsFamilyDidNotCompleteSection2 = profile.IsFamilyDidNotCompleteSection2,
                IsFamilyDidNotCompleteSection3 = profile.IsFamilyDidNotCompleteSection3,
                SectionChildParentCompleted = profile.SectionChildParentCompleted,
                IsFormSection2Submitted = _tldsDigitalSection23Parameters.TLDSDigitalSection23Service.CheckTLDSFormSectionSubmitted(profileId, 2),
                IsFormSection3Submitted = _tldsDigitalSection23Parameters.TLDSDigitalSection23Service.CheckTLDSFormSectionSubmitted(profileId, 3),
            };

            var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(profileId);
            foreach (var item in model.TLDSUploadedDocuments)
            {
                item.S3Url = GetLinkToDownloadUploadFile(tldsS3Settings.GetFormPath(item.S3FileName));
            }
            model.AccessRight = CheckPermissionToAccessProfile(profile);
            if (model.AccessRight == AccessRightEnum.NoRight)
            {
                return RedirectToAction("Index", "TLDSManage");
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult ChildFamily(TDLSProfileViewModel model)
        {
            //Save new value
            var tldsProfile = _parameters.TLDSService.GetTLDSProfile(model.ProfileId);
            if (tldsProfile == null)
            {
                return RedirectToAction("Index", "TLDSManage");
            }

            if (!_parameters.VulnerabilityService.HasRightToUpdateTLDSProfile(CurrentUser,
                tldsProfile.UserID ?? 0, CurrentUser.GetMemberListDistrictId()))
            {
                return RedirectToAction("Index", "TLDSManage");
            }

            if (model.TLDSUploadedDocumentData != null)
            {
                var serializer = new JavaScriptSerializer();
                model.TLDSUploadedDocuments = serializer.Deserialize<List<TLDSUploadedDocument>>(model.TLDSUploadedDocumentData);
                SaveUploadedDocument(model.ProfileId, model.TLDSUploadedDocuments);
                var tldsS3Settings = TLDSS3Settings.GetTLDSS3Settings(tldsProfile.ProfileId);
                foreach (var item in model.TLDSUploadedDocuments)
                {
                    item.S3Url = GetLinkToDownloadUploadFile(tldsS3Settings.GetFormPath(item.S3FileName));
                }

            }

            if (!_parameters.VulnerabilityService.HasRightToUpdateTLDSProfile(CurrentUser,
                tldsProfile.UserID ?? 0, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            //save check boxes value
            tldsProfile.Section2CheckedCompleted = model.Section2CheckedCompleted;
            tldsProfile.Section3CheckedCompleted = model.Section3CheckedCompleted;
            tldsProfile.DateUpdated = DateTime.UtcNow;
            tldsProfile.IsAboriginal = model.AboriginalValue;
            tldsProfile.HasTheFamilyIndicatedAboriginal = model.HasTheFamilyIndicatedAboriginal;
            tldsProfile.IsFamilyDidNotCompleteSection2 = model.IsFamilyDidNotCompleteSection2;
            tldsProfile.IsFamilyDidNotCompleteSection3 = model.IsFamilyDidNotCompleteSection3;
            tldsProfile.SectionChildParentCompleted = true;
            _parameters.TLDSService.SaveTLDSProfile(tldsProfile);

            // save tldsprofilemeta
            _parameters.TLDSService.SaveTLDSProfileMeta(tldsProfile.ProfileId, nameof(tldsProfile.HasTheFamilyIndicatedAboriginal), model.HasTheFamilyIndicatedAboriginal);
            _parameters.TLDSService.SaveTLDSProfileMeta(tldsProfile.ProfileId, nameof(tldsProfile.IsFamilyDidNotCompleteSection3), model.IsFamilyDidNotCompleteSection3);
            _parameters.TLDSService.SaveTLDSProfileMeta(tldsProfile.ProfileId, nameof(tldsProfile.IsFamilyDidNotCompleteSection2), model.IsFamilyDidNotCompleteSection2);
            model.SaveSuccessful = true;

            if (model.IsContinue)
            {
                return RedirectToAction("UpcomingSchoolSubmit", "TLDSManage", new { profileId = model.ProfileId });
            }
            return Json(new { result = true, uploadedDocuments = model.TLDSUploadedDocuments }, JsonRequestBehavior.AllowGet);
        }

        private void SaveUploadedDocument(int profileId, List<TLDSUploadedDocument> uploadedDocuments)
        {
            // Remove documents do not still exist in current list
            var dbUploadedDocuemnts = _parameters.TLDSService.GetTLDSUploadedDocumentByProfileId(profileId);
            foreach (var uploadedDocument in dbUploadedDocuemnts)
            {
                if (!uploadedDocuments.Any(x => x.UploadedDocumentId == uploadedDocument.UploadedDocumentId))
                {
                    _parameters.TLDSService.DeleteTLDSUploadedDocument(uploadedDocument.UploadedDocumentId);
                }
            }

            // Insert new documents into database (do not update already existed items)
            foreach (var uploadedDocument in uploadedDocuments)
            {
                if (uploadedDocument.UploadedDocumentId == 0)
                {
                    if (uploadedDocument.ProfileId == 0)
                    {
                        uploadedDocument.ProfileId = profileId;
                        uploadedDocument.UploadedUserId = CurrentUser.Id;
                        uploadedDocument.UploadedDate = DateTime.UtcNow;
                    }

                    _parameters.TLDSService.SaveTLDSUploadedDocument(uploadedDocument);
                }
            }
        }


        public ActionResult DownloadPDFForm(int profileId)
        {
            var tldsS3Setting = TLDSS3Settings.GetTLDSS3Settings(profileId);

            var fileName = System.IO.Path.GetFileName(tldsS3Setting.TLDSBlankTemplateSection234);
            var result = _s3Service.DownloadFile(tldsS3Setting.TLDSBucket, tldsS3Setting.TLDSBlankTemplateSection234);
            if (result.IsSuccess)
            {
                return File(result.ReturnStream, "text/plain", fileName);
            }

            ViewData["message"] = "Invalid download link";

            return View("Error");
        }
        private bool IsValidPostedFile(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(file.FileName))
            {
                return false;
            }
            if (file.InputStream == null)
            {
                return false;
            }
            //check pdf extentsion
            var extension = System.IO.Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension))
            {
                return false;
            }
            extension = extension.Replace(".", "");
            if (extension.ToLower() != "pdf")
            {
                return false;
            }
            return true;
        }
        public ActionResult UploadChildForm(HttpPostedFileBase postedFile, int? profileId)
        {
            if (!IsValidPostedFile(postedFile))
            {
                return Json(new { message = "Invalid file, please try again.", success = false, type = "error" },
                            JsonRequestBehavior.AllowGet);
            }
            if (!profileId.HasValue || profileId.Value == 0)
            {
                if (!_parameters.VulnerabilityService.HasRightToCreateTLDSProfile(CurrentUser))
                {
                    return Json(new { success = false, ErrorMessage = "Has no right" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var profile = _parameters.TLDSService.GetTLDSProfile(profileId.Value);
                if (!_parameters.VulnerabilityService.HasRightToUpdateTLDSProfile(CurrentUser, profile.UserID ?? 0,
                    CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { success = false, ErrorMessage = "Has no right" }, JsonRequestBehavior.AllowGet);
                }
            }
            try
            {
                var tldsS3Setting = TLDSS3Settings.GetTLDSS3Settings(profileId.Value);
                if (PdfHelper.ValidatePDF(postedFile.InputStream))
                {
                    try
                    {
                        var originalFileName = postedFile.FileName;
                        var s3FileName = originalFileName.AddTimestampToFileName();
                        string uploadFileName = tldsS3Setting.GetFormPath(s3FileName);
                        var s3Result = _s3Service.UploadRubricFile(tldsS3Setting.TLDSBucket, uploadFileName, postedFile.InputStream, false);
                        if (s3Result.IsSuccess)
                        {
                            return Json(new { Success = true, OriginalFileName = originalFileName, S3FileName = s3FileName, fileNameUrl = GetLinkToDownloadUploadFile(uploadFileName) }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { Success = false, ErrorMessage = s3Result.ErrorMessage }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return Json(new { success = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { message = "This PDF file contains an error and could not be read correctly. Please contact the person who created the document to correct the problem, or contact the service desk for further advice.", success = false, type = "error" },
                        JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "This PDF file contains an error and could not be read correctly. Please contact the person who created the document to correct the problem, or contact the service desk for further advice.", success = false, type = "error" },
                            JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetGender()
        {
            var data = _parameters.GenderService.GetAllGenders().Select(x => new ListItem() { Id = x.GenderID, Name = x.Name }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        public ActionResult UploadStatementPdf(HttpPostedFileBase postedFile, int? profileId)
        {
            if (!IsValidPostedFile(postedFile))
            {
                return Json(new { message = "Invalid file, please try again.", success = false, type = "error" },
                            JsonRequestBehavior.AllowGet);
            }
            if (profileId.HasValue && profileId.Value > 0)
            {
                var profile = _parameters.TLDSService.GetTLDSProfile(profileId.Value);
                if (!_parameters.VulnerabilityService.HasRightToUpdateTLDSProfile(CurrentUser, profile.UserID ?? 0,
                    CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { success = false, ErrorMessage = "Has no right" }, JsonRequestBehavior.AllowGet);
                }
            }
            try
            {
                var id = profileId.HasValue ? profileId.Value : 0;
                var tldsS3Setting = TLDSS3Settings.GetTLDSS3Settings(id);
                try
                {
                    var originalFileName = postedFile.FileName;
                    var s3FileName = originalFileName.AddTimestampToFileName();
                    string uploadFileName = tldsS3Setting.GetStatementUploadedPath(s3FileName);
                    var s3Result = _s3Service.UploadRubricFile(tldsS3Setting.TLDSBucket, uploadFileName, postedFile.InputStream, false);
                    if (s3Result.IsSuccess)
                    {
                        return Json(new { Success = true, OriginalFileName = originalFileName, S3FileName = s3FileName, fileNameUrl = GetLinkToDownloadUploadFile(uploadFileName) }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { Success = false, ErrorMessage = s3Result.ErrorMessage }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                    return Json(new { success = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { message = "Can not read file, please try again.", success = false, type = "error" },
                            JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveStatementUploadInfo(TDLSProfileUploadViewModel model)
        {
            try
            {
                var tldsProfile = new TLDSProfile();
                tldsProfile.DateCreated = DateTime.UtcNow;
                tldsProfile.Status = (int)TLDSProfileStatusEnum.UploadedBySchool;
                tldsProfile.LastStatusDate = DateTime.UtcNow;
                tldsProfile.UserID = CurrentUser.Id;
                tldsProfile.FirstName = model.FirstName;
                tldsProfile.LastName = model.LastName;
                tldsProfile.GenderId = model.GenderId;
                tldsProfile.DateOfBirth = model.DateOfBirth;
                tldsProfile.ECSName = model.ECSName;
                tldsProfile.Section102IsNotRequired = !(!model.Section102IsCompleted.HasValue ||
                                                       model.Section102IsCompleted.Value);
                var schoolIds = _parameters.UserSchoolService.GetListSchoolIdByUserId(CurrentUser.Id);
                tldsProfile.UpcommingSchoolID = schoolIds.FirstOrDefault();
                tldsProfile.EnrolmentYear = model.EnrollmentYear;
                tldsProfile.ECSCompledDate = DateTime.UtcNow;
                tldsProfile.UserID = CurrentUser.Id;
                tldsProfile.DateUpdated = DateTime.UtcNow;
                tldsProfile.TLDSInformationHasBeenSaved = true;
                tldsProfile.ContextSpecificInforHasBeenSaved = false;
                tldsProfile.DevelopmentOutcomeHasBeenSaved = false;
                tldsProfile.EARReportCompleted = false;
                tldsProfile.EARAvailableUponRequest = false;
                tldsProfile.Section102HasBeenSaved = false;
                tldsProfile.ParentConsentIsIncluded = false;
                tldsProfile.SectionChildParentCompleted = false;
                tldsProfile.PrintAllSectionsFamily = false;
                tldsProfile.WasAnEarlyABLESReportCompleted = false;
                _parameters.TLDSService.SaveTLDSProfile(tldsProfile);

                //save metadata
                _parameters.TLDSService.SaveTLDSProfileMeta(tldsProfile.ProfileId,
                    nameof(tldsProfile.UploadedStatementPdfFileName), model.PdfFileName);
                _parameters.TLDSService.SaveTLDSProfileMeta(tldsProfile.ProfileId,
                    nameof(tldsProfile.IsUploadedStatement), "true");
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetStatesTLDS()
        {
            IQueryable<State> data = _parameters.StateService.GetStates().Where(x => x.Code == "AU");
            data = data.OrderBy(x => x.Name);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDistrictsTLDS()
        {
            var receivingDistrictIDList = _parameters.DistrictDecodeService.GetReceivingTLDSProfileDistrictID();
            IQueryable<District> districts = _parameters.DistrictService.FilterDistricByIds(receivingDistrictIDList);
            IOrderedQueryable<ListItem> data = districts.Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEarlyABLESReportData(int profileId)
        {
            var earlyABLESReports =
                _parameters.TLDSService.GetTLDSEarlyABLESReportOfProfile(profileId)
                    .Select(x => new TLDSEarlyABLESReportViewModel
                    {
                        EarlyABLESReportId = x.EarlyABLESReportId,
                        ProfileId = x.ProfileId,
                        ReportName = x.ReportName,
                        LearningReadinessReportCompleted = x.LearningReadinessReportCompleted,
                        ReportDate = x.ReportDate,
                        AvailableOnRequest = x.AvailableOnRequest,
                    })
                    .ToList();
            // there must be at least two records
            if (earlyABLESReports.Count < 2)
            {
                int count = earlyABLESReports.Count;
                for (int i = count; i < 2; i++)
                {
                    earlyABLESReports.Add(new TLDSEarlyABLESReportViewModel());
                }
            }

            return Json(new { earlyABLESReports }, JsonRequestBehavior.AllowGet);
        }
        private void UpdateTLDSProfileSection0FromForm(TLDSProfile profile, TDLSProfileViewModel model)
        {
            profile.FirstName = model.FirstName;
            profile.LastName = model.LastName;
            profile.DateOfBirth = model.DateOfBirth;
            if (model.GenderId == 0)
            {
                profile.GenderId = (int)GenderEnum.Unknown;
            }
            else
            {
                profile.GenderId = model.GenderId;
            }

            profile.PrimarySchool = model.PrimarySchool;
            profile.OutsideSchoolHoursCareService = model.OutsideSchoolHoursCareService;
            profile.PhotoURL = model.FileName;//now storing file name only
            profile.ECSName = model.ECSName;
            profile.ECSAddress = model.ECSAddress;
            profile.ECSApprovalNumber = model.ECSApprovalNumber;
            profile.ECSCompletingFormEducatorName = model.ECSCompletingFormEducatorName;
            profile.ECSCompletingFormEducatorPosition = model.ECSCompletingFormEducatorPosition;
            profile.ECSCompletingFormEducatorQualification = model.ECSCompletingFormEducatorQualification;
            profile.ECSCompletingFormEducatorQualificationId = model.ECSCompletingFormEducatorQualificationId;
            profile.ECSCompletingFormEducatorPhone = model.ECSCompletingFormEducatorPhone;
            profile.ECSCompletingFormEducatorEmail = model.ECSCompletingFormEducatorEmail;
            profile.ECSCompledDate = DateTime.UtcNow;
            profile.EnrolmentYear = model.EnrolmentYear;
            profile.TldsGroupId = model.TldsGroupId;
        }

        private void SendMailSubmitSchool(int profileId, int schoolId)
        {
            //get receivers
            //receivers is school admin of the school
            var userIdList = _parameters.UserSchoolService.ListUserIdBySchoolId(schoolId);
            if (userIdList == null)
            {
                userIdList = new List<int>();
            }
            var userList = _parameters.UserService.Select().Where(x => userIdList.Contains(x.Id)).ToList();
            //get email address of teacher
            var emailList = new List<string>();
            foreach (var user in userList)
            {
                if (user.RoleId == (int)BubbleSheetPortal.Models.Permissions.SchoolAdmin
                    && !string.IsNullOrWhiteSpace(user.EmailAddress)
                    && user.UserStatusId == (int)UserStatus.Active)
                {
                    emailList.Add(user.EmailAddress);
                }
            }
            var school = _parameters.SchoolService.GetSchoolById(schoolId);
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);

            var objEmailTemplate =
                _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(CurrentUser.DistrictId ?? 0,
                    Constanst.Configuration_TLDSEmailTemplateSubmitToSchool).FirstOrDefault();
            string strBody = objEmailTemplate == null ? string.Empty : objEmailTemplate.Value;
            if (string.IsNullOrWhiteSpace(strBody))
            {
                return;
            }
            string fullName = string.Empty;
            if (!string.IsNullOrWhiteSpace(profile.FirstName) && !string.IsNullOrWhiteSpace(profile.LastName))
            {
                fullName = string.Format("{0}, {1}", profile.LastName, profile.FirstName);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(profile.FirstName))
                {
                    fullName = profile.FirstName;
                }
                if (!string.IsNullOrWhiteSpace(profile.LastName))
                {
                    fullName = profile.LastName;
                }
            }
            strBody = strBody.Replace("<FullName>", fullName);
            strBody = strBody.Replace("<School>", school.Name);
            string strSubject = "TLDS Profile";

            var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.TLDSUseEmailCredentialKey);

            foreach (var email in emailList)
            {
                try
                {
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        LinkIt.BubbleSheetPortal.Web.Helpers.Util.SendMailTLDSV2(strBody, strSubject, email, emailCredentialSetting);
                    }, TaskCreationOptions.DenyChildAttach);
                }
                catch
                {
                    //nothing
                }
            }
        }

        public ActionResult LoadPrintConfirm(int profileId)
        {
            var model = new TDLSProfileViewModel()
            {
                ProfileId = profileId,
            };
            return PartialView("_TLDSPrintConfirm", model);
        }
        public ActionResult GetTeachersSubmitted()
        {
            var teachers = new List<ListItem>();
            if (CurrentUser.IsSchoolAdmin)
            {
                var filterParameter = new TLDSFilterParameter();
                var teacherIdList = _parameters.TLDSService.FilterTLDSProfile(CurrentUser.Id, filterParameter).Select(x => x.UserId).ToList();
                teachers = _parameters.UserService.Select().Where(x => teacherIdList.Contains(x.Id)).OrderBy(x => x.Name).Select(x => new ListItem()
                {
                    Id = x.Id,
                    Name = x.Name
                }).Distinct().ToList();
            }

            return Json(teachers, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadPrintBlankFormConfirm(int profileId)
        {
            var model = new TDLSProfileViewModel()
            {
                ProfileId = profileId,
            };
            return PartialView("_TLDSPrintBlankFormConfirm", model);
        }

        public ActionResult GetTDLSProfileForSchoolAdmin(TLDSFilterParameter p)
        {
            var tldsS3Setting = TLDSS3Settings.GetTLDSS3Settings(0);
            var parser = new DataTableParser<TDLSProfileItemListSchoolAdmin>();
            var tdlsProfiles = _parameters.TLDSService.GetTLDSProfilesForSchoolAdmin(CurrentUser.Id, p)
                    .Select(x => new TDLSProfileItemListSchoolAdmin
                    {
                        ProfileId = x.ProfileID,
                        LastName = x.LastName,
                        FirstName = x.FirstName,
                        DateOfBirth = x.DateOfBirth,
                        GenderID = x.GenderID,
                        ECSName = x.ECSName,
                        Section102IsNotRequired = x.Section102IsNotRequired,
                        EnrolmentYear = x.EnrolmentYear,
                        StudentID = x.StudentID,
                        IsUploadedStatement = x.IsUploadedStatement,
                        PDFUrl = !string.IsNullOrEmpty(x.UploadedStatementPdfFileName) ? GetLinkToDownloadUploadFile(tldsS3Setting.GetStatementUploadedPath(x.UploadedStatementPdfFileName)) : string.Empty,
                        ECSCompledDate = x.ECSCompledDate
                    });


            return Json(parser.Parse(tdlsProfiles.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadStudentFilterForTLDS(int tldsProfileId)
        {
            var model = new TLDSAddNewStudentsCustomViewModel
            {
                ProfileId = tldsProfileId,
                IsPublisherOrNetworkAdmin = false,
                IsPublisher = CurrentUser.IsPublisher,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin
            };

            model.IsPublisherOrNetworkAdmin = CurrentUser.IsPublisher() || CurrentUser.IsNetworkAdmin();

            var profile = _parameters.TLDSService.GetTLDSProfile(tldsProfileId);
            model.SubmittedStudentName = profile.FullName;
            model.DOBString = profile.DateOfBirth.DisplayDateWithFormat();
            if (profile.GenderId.HasValue)
            {
                if (profile.GenderId.Value == (int)GenderEnum.Unknown)
                {
                    model.Gender = "Unknow";
                }

                if (profile.GenderId.Value == (int)GenderEnum.Male)
                {
                    model.Gender = "Male";
                }

                if (profile.GenderId.Value == (int)GenderEnum.Female)
                {
                    model.Gender = "Female";
                }
            }


            return PartialView("_AddNewStudentsFilter", model);
        }
        public ActionResult SearchStudentToAssociate(LookupStudentCustom model)
        {
            var parser = new DataTableParserProc<TLDSStudentFilterViewModel>();
            var data = new List<TLDSStudentFilterViewModel>().AsQueryable();
            int? totalRecords = 0;
            if (!model.ClassId.HasValue)
                model.ClassId = -1;

            if (!model.DistrictId.HasValue && !CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin())
                model.DistrictId = CurrentUser.DistrictId.Value;

            if (model.DistrictId > 0)
            {
                model.UserId = CurrentUser.Id;
                model.RoleId = CurrentUser.RoleId;

                var sortColumns = parser.SortableColumns;

                model.FirstName = model.FirstName == null ? null : model.FirstName.Trim();
                model.LastName = model.LastName == null ? null : model.LastName.Trim();
                model.Code = model.Code == null ? null : model.Code.Trim();
                model.StateCode = model.StateCode == null ? null : model.StateCode.Trim();


                data = _parameters.TLDSService.TLDSStudentLookup(model, parser.StartIndex, parser.PageSize,
                    ref totalRecords, sortColumns)
                    .Select(x => new TLDSStudentFilterViewModel
                    {
                        StudentId = x.StudentId,
                        LastName = x.LastName,
                        FirstName = x.FirstName,
                        //Code = x.Code,
                        //Code = x.StateCode,
                        Code = x.AltCode,
                        GenderCode = x.GenderCode,
                        GradeName = x.GradeName,
                        SchoolName = x.SchoolName,
                        ProfileID = x.TLDSProfileID
                    }).AsQueryable();
            }

            return Json(parser.Parse(data, totalRecords ?? 0, true), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AssociateStudentToProfile(TLDSAssociateStudentToProfileModel model)
        {
            var profile = _parameters.TLDSService.GetTLDSProfile(model.ProfileId);
            if (profile == null)
            {
                return RedirectToAction("Index", "TLDSManage");
            }
            //Security: Check studentId first
            var studentList = new List<int>();
            studentList.Add(model.StudentId);
            var hasRightOnStudent = _parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser,
                studentList);
            if (!hasRightOnStudent)
            {
                return Json(new { Success = false, error = "Has no right on student." }, JsonRequestBehavior.AllowGet);
            }
            if (!_parameters.VulnerabilityService.HasRightToAssociateStudentToTLDSProfile(CurrentUser, profile, null))
            {
                return Json(new { Success = false, error = "Has no right on profile." }, JsonRequestBehavior.AllowGet);
            }
            //Check if this student has been associated with any profile or not
            var associatedProfile = _parameters.TLDSService.GetProfileOfStudent(model.StudentId);
            if (associatedProfile != null)
            {
                return Json(new { Success = false, error = "This student has been associated to a profile." }, JsonRequestBehavior.AllowGet);
            }
            _parameters.TLDSService.AssociateToStudent(model.ProfileId, model.StudentId);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveAssociateStudentFromProfile(int profileId)
        {
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);
            if (profile == null)
            {
                return RedirectToAction("Index", "TLDSManage");
            }
            if (profile.StudentID.HasValue)
            {
                //Security: Check studentId first
                var studentList = new List<int>();
                studentList.Add(profile.StudentID.Value);
                var hasRightOnStudent = _parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser,
                    studentList);
                if (!hasRightOnStudent)
                {
                    return Json(new { Success = true, error = "Has no right on student." }, JsonRequestBehavior.AllowGet);
                }
                if (!_parameters.VulnerabilityService.HasRightToAssociateStudentToTLDSProfile(CurrentUser, profile, null))
                {
                    return Json(new { Success = true, error = "Has no right on profile." }, JsonRequestBehavior.AllowGet);
                }
                _parameters.TLDSService.RemoveAssociatedStudent(profileId);
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteProfile(int profileId)
        {
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);
            if (profile == null)
            {
                return RedirectToAction("Index", "TLDSManage");
            }
            //check security
            if (!_parameters.VulnerabilityService.HasRightToUpdateTLDSProfile(CurrentUser, profile.UserID ?? 0,
                CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = true, error = "Has no right on the profile." }, JsonRequestBehavior.AllowGet);
            }
            if (profile.StudentID.HasValue)
            {
                //Security: Check studentId first
                var studentList = new List<int>();
                studentList.Add(profile.StudentID.Value);
                var hasRightOnStudent = _parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser,
                    studentList);
                if (!hasRightOnStudent)
                {
                    return Json(new { Success = true, error = "Has no right on student." }, JsonRequestBehavior.AllowGet);
                }
            }

            _parameters.TLDSService.DeleteProfile(CurrentUser.Id, CurrentUser.DistrictId ?? 0, profileId);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteProfileSchoolAdmin(int profileId)
        {
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);
            if (profile == null)
            {
                return RedirectToAction("Index", "TLDSManage");
            }
            if (profile.StudentID.HasValue)
            {
                //Security: Check studentId first
                var studentList = new List<int>();
                studentList.Add(profile.StudentID.Value);
                var hasRightOnStudent = _parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser,
                    studentList);
                if (!hasRightOnStudent)
                {
                    return Json(new { Success = true, error = "Has no right on student." }, JsonRequestBehavior.AllowGet);
                }
            }

            _parameters.TLDSService.DeleteProfile(CurrentUser.Id, CurrentUser.DistrictId ?? 0, profileId);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadRejectProfileDialog(int tldsProfileId)
        {
            var profile = _parameters.TLDSService.GetTLDSProfile(tldsProfileId);
            ViewBag.SubmittedStudent = string.Empty;
            if (!string.IsNullOrEmpty(profile.LastName) && !string.IsNullOrEmpty(profile.FirstName))
            {
                ViewBag.SubmittedStudent = string.Format("{0}, {1}", profile.LastName, profile.FirstName);
            }
            else
            {
                ViewBag.SubmittedStudent = string.Format("{0}{1}", profile.LastName, profile.FirstName);
            }
            ViewBag.ProfileId = tldsProfileId;
            return PartialView("_RejectProfile");
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult RejectProfile(int profileId, string reason)
        {
            reason = reason.DecodeParameters();
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);
            if (profile == null)
            {
                return RedirectToAction("Index", "TLDSManage");
            }
            if (!_parameters.VulnerabilityService.HasRightToRejectTLDSProfile(CurrentUser, profile, null))
            {
                return Json(new { Success = true, error = "Has no right on profile." }, JsonRequestBehavior.AllowGet);
            }
            if (profile.StudentID != null)
            {
                return Json(new { Success = true, error = "Has associated student, unable to reject profile." }, JsonRequestBehavior.AllowGet);
            }
            _parameters.TLDSService.RejectProfile(CurrentUser.Id, CurrentUser.DistrictId ?? 0, profileId, reason);

            SendMailRejectProfile(profileId);
             
            //Task.Run(() => SendMailRejectProfile(profileId));//let the send email function run background in another thread
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

        }

        private void SendMailRejectProfile(int profileId)
        {
            //get receivers

            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);
            var owner = _parameters.UserService.GetUserById(profile.UserID ?? 0);
            if (owner != null)
            {
                if (string.IsNullOrEmpty(owner.EmailAddress))
                {
                    return;
                }
                var objEmailTemplate =
               _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(CurrentUser.DistrictId ?? 0,
                   Constanst.Configuration_TLDSEmailTemplateRejectedProfile).FirstOrDefault();
                string strBody = objEmailTemplate == null ? string.Empty : objEmailTemplate.Value;
                if (string.IsNullOrWhiteSpace(strBody))
                {
                    return;

                }
                string fullName = string.Empty;
                if (!string.IsNullOrWhiteSpace(profile.FirstName) && !string.IsNullOrWhiteSpace(profile.LastName))
                {
                    fullName = string.Format("{0}, {1}", profile.LastName, profile.FirstName);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(profile.FirstName))
                    {
                        fullName = profile.FirstName;
                    }
                    if (!string.IsNullOrWhiteSpace(profile.LastName))
                    {
                        fullName = profile.LastName;
                    }
                }
                strBody = strBody.Replace("<FullName>", fullName);
                strBody = strBody.Replace("<Reason>", profile.RejectedReason);
                string strSubject = "TLDS Profile Returned";

                var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.TLDSUseEmailCredentialKey);
                try
                {
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        LinkIt.BubbleSheetPortal.Web.Helpers.Util.SendMailTLDSV2(strBody, strSubject, owner.EmailAddress, emailCredentialSetting);
                    }, TaskCreationOptions.DenyChildAttach);
                }
                catch
                {
                    //Not allow email to affect another process
                }

            }

        }

        public ActionResult LoadBatchPrintConfirm(string profileIdList)
        {
            ViewBag.ProfileIdList = profileIdList;
            return PartialView("_TLDSBatchPrintConfirm");
        }
        public ActionResult GetEnrollmentYearFilter(bool? showArchived = false)
        {
            var data = new List<ListItem>();
            if (CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher)
            {
                var filterParameter = new TLDSFilterParameter() { ShowArchived = showArchived };
                var enrolmentYears = _parameters.TLDSService.GetTLDSProfilesForSchoolAdmin(CurrentUser.Id, filterParameter).Where(x => x.EnrolmentYear.HasValue).Select(x => x.EnrolmentYear).Distinct().ToList();
                data = enrolmentYears.Select(x => new ListItem()
                {
                    Id = x ?? 0,
                    Name = x.ToString()
                }).OrderByDescending(x => x.Id).ToList();

                if (CurrentUser.IsSchoolAdmin)
                {
                    foreach(var item in data)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }

            var nextYear = DateTime.UtcNow.Year + 1;
            if (!data.Select(c => c.Id).Contains(nextYear))
            {
                data.Add(new ListItem()
                {
                    Id = nextYear,
                    Selected = CurrentUser.IsTeacher,
                    Name = nextYear.ToString()
                });
            }
            else
            {
                if (CurrentUser.IsTeacher)
                {
                    foreach (var item in data)
                    {
                        if (item.Id == nextYear)
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                }
            }

            return Json(data.OrderByDescending(x => x.Id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadPrintSummaryReportConfirm()
        {
            return PartialView("_TLDSPrintSummaryReportConfirm");
        }

        [HttpGet]
        public ActionResult GetGradesToFilter()
        {

            if (CurrentUser.IsSchoolAdmin)
            {
                var data = _parameters.TLDSService.GetGradesForFilter(CurrentUser.Id, CurrentUser.DistrictId,
                    CurrentUser.RoleId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult LoadRecallProfileDialog(int profileId)
        {
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);
            if (profile != null)
            {
                ViewBag.SubmittedStudent = string.Format("{0}, {1}", profile.LastName, profile.FirstName);
            }
            ViewBag.ProfileId = profileId;
            return PartialView("_RecallProfileDialog");
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RecallTLDS(int profileId, string reason)
        {
            reason = reason.DecodeParameters();
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);
            if (profile == null)
            {
                return RedirectToAction("Index", "TLDSManage");
            }
            if (profile.UserID.HasValue && CurrentUser.Id != profile.UserID)
            {
                return Json(new { Success = false, Error = "Has no right on profile." }, JsonRequestBehavior.AllowGet);
            }
            if (profile.StudentID != null || (profile.Status.HasValue && profile.Status.Value == (int)TLDSProfileStatusEnum.AssociatedWithStudent))
            {
                return Json(new { Success = false, Error = "Has associated student, unable to recall profile." }, JsonRequestBehavior.AllowGet);
            }

            if (profile.Status.HasValue && profile.Status.Value == (int)TLDSProfileStatusEnum.SubmittedToSchool)
            {
                profile.Status = (int)TLDSProfileStatusEnum.Recalled;
                SendMailRecallProfileToSubmittedSchool(profile, reason);
                profile.LastStatusDate = DateTime.UtcNow;
                profile.DateUpdated = DateTime.UtcNow;
                profile.ECSCompledDate = DateTime.UtcNow;
                profile.UpcommingSchoolID = null;
                _parameters.TLDSService.SaveTLDSProfile(profile);
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        private string FormatFullName(string firstName, string lastName)
        {
            string fullName = string.Empty;
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                fullName = string.Format("{0}, {1}", firstName, lastName);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(firstName))
                {
                    fullName = firstName;
                }
                if (!string.IsNullOrWhiteSpace(lastName))
                {
                    fullName = lastName;
                }
            }
            return fullName;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ReopenProfile(int profileId)
        {
            var profile = _parameters.TLDSService.GetTLDSProfile(profileId);
            if (profile == null)
            {
                return RedirectToAction("Index", "TLDSManage");
            }
            if (profile.UserID.HasValue && CurrentUser.Id != profile.UserID)
            {
                return Json(new { Success = false, Error = "Has no right on profile." }, JsonRequestBehavior.AllowGet);
            }
            if (profile.StudentID != null || (profile.Status.HasValue && profile.Status.Value == (int)TLDSProfileStatusEnum.AssociatedWithStudent))
            {
                return Json(new { Success = false, Error = "Has associated student, unable to recall profile." }, JsonRequestBehavior.AllowGet);
            }

            if (profile.Status.HasValue && profile.Status.Value == (int)TLDSProfileStatusEnum.CreatedUnsubmitted)
            {
                profile.Status = (int)TLDSProfileStatusEnum.Draft;
                profile.UpcommingSchoolID = null;
                profile.LastStatusDate = DateTime.UtcNow;
                profile.DateUpdated = DateTime.UtcNow;
                profile.ECSCompledDate = DateTime.UtcNow;
                _parameters.TLDSService.SaveTLDSProfile(profile);
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        private void SendMailRecallProfileToSubmittedSchool(TLDSProfile profile, string reason)
        {
            try
            {
                var userIdList = new List<int>();
                userIdList = _parameters.UserSchoolService.ListUserIdBySchoolId(profile.UpcommingSchoolID.Value).ToList();
                var userList = _parameters.UserService.Select().Where(x => userIdList.Contains(x.Id)).ToList();
                var emailList = new List<string>();
                foreach (var user in userList)
                {
                    if (user.RoleId == (int)BubbleSheetPortal.Models.Permissions.SchoolAdmin
                        && !string.IsNullOrWhiteSpace(user.EmailAddress)
                        && user.UserStatusId == (int)UserStatus.Active)
                    {
                        emailList.Add(user.EmailAddress);
                    }
                }

                var currentUser = _parameters.UserService.GetUserById(CurrentUser.Id);
                var school = _parameters.SchoolService.GetSchoolById(currentUser.SchoolId ?? 0);

                string sender = school != null ? school.Name : "Unknown School";
                string emailTemplate = "A user at {0} has recalled the online TLDS for <b> {1} </b> due to the following reason: {2}";
                string fullName = FormatFullName(profile.LastName, profile.FirstName);
                string strBody = string.Format(emailTemplate, sender, fullName, reason);
                string strSubject = "TLDS Profile Recall";

                var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.TLDSUseEmailCredentialKey);

                foreach (var email in emailList)
                {
                    Task.Factory.StartNew(() =>
                    {
                        Util.SendMailTLDSV2(strBody, strSubject, email, emailCredentialSetting);
                    }, TaskCreationOptions.DenyChildAttach);
                }
            }
            catch
            {
                //log
            }
        }

        [HttpGet]
        public ActionResult GetSubmitTLDSStatus(int submitDistrictId)
        {
            var submitTLDS = true;

            var isDistTrainingTeacher = _parameters.DistrictDecodeService.GetDistrictDecodeByLabel(CurrentUser.DistrictId.Value, ContaintUtil.TLDSTrainingDistrict);
            if (isDistTrainingTeacher)
            {
                var isTrainingDist = _parameters.DistrictDecodeService.GetDistrictDecodeByLabel(submitDistrictId, ContaintUtil.TLDSTrainingDistrict);
                if (!isTrainingDist)
                    submitTLDS = false;
            }

            return Json(new { SubmitTLDS = submitTLDS }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReviewLinks()
        {
            return View();
        }

        public ActionResult CreateGroup()
        {
            var model = new TLDSGroupDTO();
            return PartialView("_CreateGroup", model);
        }

        [HttpPost]
        public ActionResult DeactiveGroup(int tldsGroupId)
        {
            var isSuccess = _parameters.TLDSService.DeactiveTldsGroup(tldsGroupId);

            return Json(new { Success = isSuccess }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateGroup(TLDSGroupDTO tldsGroup)
        {
            var userMeta = _parameters.TLDSService.GetTLDSUserMetaByUserId(CurrentUser.Id);
            if (userMeta != null)
            {
                if (SaveTldsGroup(userMeta.TLDSUserMetaID, tldsGroup))
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Success = false, ErrorMessage = TextConstants.DUPLICATED_GROUP_NAME }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //save configuration to database
                var tldsUserMeta = new TLDSUserMeta()
                {
                    UserID = CurrentUser.Id
                };
                var metaModel = new TLDSUserMetaValueModel();
                metaModel.TLDSUserConfigurations = new TLDSUserConfigurations();
                tldsUserMeta.MetaValue = metaModel.ConvertToJsonData();
                _parameters.TLDSService.SaveTLDSUserMeta(tldsUserMeta);
                // save group to database
                if (SaveTldsGroup(tldsUserMeta.TLDSUserMetaID, tldsGroup))
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Success = false, ErrorMessage = TextConstants.DUPLICATED_GROUP_NAME }, JsonRequestBehavior.AllowGet);
            }
        }

        private bool SaveTldsGroup(int tldsGroupUserMetaId, TLDSGroupDTO tldsGroup)
        {
            if (_parameters.TLDSService.CheckUniqueGroupName(tldsGroupUserMetaId, tldsGroup.GroupName.Trim()))
            {
                return false;
            }
            tldsGroup.TLDSUserMetaID = tldsGroupUserMetaId;
            tldsGroup.Status = true;
            _parameters.TLDSService.SaveTldsGroup(tldsGroup);
            return true;
        }

        public ActionResult CreateProfileTeacher()
        {
            ViewBag.QualificationList = QualificationList;
            return PartialView("_CreateProfileTeacher");
        }

        [HttpPost]
        public ActionResult CreateProfileTeacher(TLDSProfileTeacherDTO tldsProfileTeacherDto)
        {
            var userMeta = _parameters.TLDSService.GetTLDSUserMetaByUserId(CurrentUser.Id);
            if (userMeta != null)
            {
                SaveTeacherProfile(userMeta.TLDSUserMetaID, tldsProfileTeacherDto);
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //save configuration to database
                var tldsUserMeta = new TLDSUserMeta()
                {
                    UserID = CurrentUser.Id
                };
                var metaModel = new TLDSUserMetaValueModel();
                metaModel.TLDSUserConfigurations = new TLDSUserConfigurations();
                tldsUserMeta.MetaValue = metaModel.ConvertToJsonData();
                _parameters.TLDSService.SaveTLDSUserMeta(tldsUserMeta);
                // save teacher profile to database
                SaveTeacherProfile(tldsUserMeta.TLDSUserMetaID, tldsProfileTeacherDto);
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
        }

        private void SaveTeacherProfile(int tldsUserMetaId, TLDSProfileTeacherDTO tldsProfileTeacherDto)
        {
            tldsProfileTeacherDto.TLDSUserMetaID = tldsUserMetaId;
            _parameters.TLDSService.SaveTldsProfileTeacher(tldsProfileTeacherDto);
        }

        public ActionResult AssociateToProfile(int tldsGroupId)
        {
            if (tldsGroupId == 0)
                ViewBag.SelectGroups = GetGroupSelectedList();
            else
                ViewBag.SelectGroups = null;
            return View();
        }

        [HttpPost]
        public ActionResult AssociateToProfile(AssociateToProfileDTO associateToProfileDto)
        {
            var groupId = associateToProfileDto.TLDSGroupID;
            if (associateToProfileDto.TLDSProfileIDs != null || associateToProfileDto.TLDSProfileIDs.Count > 0)
            {
                foreach (var profileId in associateToProfileDto.TLDSProfileIDs)
                {
                    _parameters.TLDSService.AssociateToProfile(profileId, groupId);
                }
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTDLSProfileForAssociateToGroup(TLDSFilterParameter p)
        {
            var parser = new DataTableParser<TDLSProfileCustomViewModel>();
            var tdlsProfiles = _parameters.TLDSService.GetTLDSProfileForAssociateToGroup(CurrentUser.Id, p)
                    .Select(x => new TDLSProfileCustomViewModel
                    {
                        ProfileId = x.ProfileID,
                        LastName = x.LastName,
                        FirstName = x.FirstName,
                        ECSCompletingFormEducatorName = x.ECSCompletingFormEducatorName,
                        Status = x.Status,
                        LastStatusDate = x.LastStatusDate,
                        School = x.SchoolName,
                        OnlyView = CheckTLDSOnlyView(x.EnrolmentYear.GetValueOrDefault())
                    }).Where(x => x.OnlyView == p.ShowArchived.GetValueOrDefault());

            return Json(parser.Parse(tdlsProfiles.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGroupSelectList(bool? showArchived = false)
        {
            var data = new List<ListItem>();
            if (CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher)
            {
                var filterParameter = new TLDSFilterParameter() { ShowArchived = showArchived };
                var groups = _parameters.TLDSService.GetTLDSProfilesForSchoolAdmin(CurrentUser.Id, filterParameter)
                                                .Where(x => x.TldsGroupID.HasValue && x.StatusGroup)
                                                .Select(x => new { x.TldsGroupID, x.GroupName })
                                                .Distinct()
                                                .ToList();

                data = groups.Select(x => new ListItem()
                {
                    Id = x.TldsGroupID ?? 0,
                    Name = x.GroupName
                }).ToList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditGroup(int tldsGroupId)
        {
            var tldsGroup = new TLDSGroupDTO();
            var userMeta = _parameters.TLDSService.GetTLDSUserMetaByUserId(CurrentUser.Id);
            if (userMeta != null)
            {
                tldsGroup = _parameters.TLDSService.GetAllTldsGroupByUserMetaID(userMeta.TLDSUserMetaID)
                                                        .FirstOrDefault(x => x.TLDSGroupID == tldsGroupId);
                if (tldsGroup == null)
                    return View(new TLDSGroupDTO());
            }
            return View(tldsGroup);
        }

        [HttpPost]
        public ActionResult RemoveToGroup(AssociateToProfileDTO removeToGroupDto)
        {
            if (removeToGroupDto.TLDSProfileIDs != null || removeToGroupDto.TLDSProfileIDs.Count > 0)
            {
                foreach (var profileId in removeToGroupDto.TLDSProfileIDs)
                {
                    _parameters.TLDSService.RemoveToGroup(profileId);
                }
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTldsProfileByGroupId(int tldsGroupId)
        {
            var parser = new DataTableParser<TDLSProfileCustomViewModel>();
            var filterValues = new TLDSFilterParameter();
            filterValues.TldsGroupID = tldsGroupId;

            var tdlsProfiles = _parameters.TLDSService.FilterTLDSProfile(CurrentUser.Id, filterValues)
                    .Select(x => new TDLSProfileCustomViewModel
                    {
                        ProfileId = x.ProfileID,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        ECSCompletingFormEducatorName = x.ECSCompletingFormEducatorName,
                        Status = x.Status,
                        LastStatusDate = x.LastStatusDate,
                        School = x.SchoolName,
                        Viewable = x.Viewable,
                        Updateable = x.Updateable
                    });

            return Json(parser.Parse(tdlsProfiles.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ActiveGroup(int tldsGroupId)
        {
            var isSuccess = _parameters.TLDSService.ActiveTldsGroup(tldsGroupId);

            return Json(new { Success = isSuccess }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveTeacherProfile(int teacherProfileId)
        {
            var isSuccess = false;
            var message = string.Empty;

            // valid permision
            var userMeta = _parameters.TLDSService.GetTLDSUserMetaByUserId(CurrentUser.Id);

            if (userMeta == null)
            {
                message = "You don't have nay user metadata";
            }
            else
            {
                var teacherProfiles = _parameters.TLDSService.GetAllTldsProfileTeachersByUserMetaID(userMeta.TLDSUserMetaID);
                var isOwner = teacherProfiles.Any(m => m.TLDSProfileTeacherID == teacherProfileId);

                if (isOwner)
                {
                    isSuccess = _parameters.TLDSService.RemoveTeacherProfile(teacherProfileId);
                }
                else
                {
                    message = "You don't have permision to remove this.";
                }
            }


            return Json(new { success = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewTheParentResponse(Guid tldsProfileLinkId)
        {
            var model = new TldsViewTheParentResponseViewModel();
            model.TldsFormSection2 = BuildTldsSection2(tldsProfileLinkId);
            model.TldsFormSection3 = BuildTldsSection3(tldsProfileLinkId);
            return View(model);
        }

        private TldsFormSection2ViewModel BuildTldsSection2(Guid id)
        {
            var section2 = new TldsFormSection2ViewModel();

            var tldsProfile = _tldsDigitalSection23Parameters.TLDSDigitalSection23Service.GetProfile(id);
            if (tldsProfile != null)
            {
                ViewBag.ChildName = string.Join(" ", tldsProfile.FirstName, tldsProfile.LastName);
            }

            var childForm = _tldsDigitalSection23Parameters.TLDSDigitalSection23Service.GetFormSections2(id);
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
                section2.Drawing = _s3Service.GetPublicUrl(LinkitConfigurationManager.GetS3Settings().TLDSBucket, childForm.Drawing);
            }

            return section2;
        }

        private TldsFormSection3ViewModel BuildTldsSection3(Guid id)
        {
            var section3 = new TldsFormSection3ViewModel();

            var familyForm = _tldsDigitalSection23Parameters.TLDSDigitalSection23Service.GetFormSections3(id);
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

        private AccessRightEnum CheckPermissionToAccessProfile(TLDSProfile profile, bool upcomingSchoolSubmit = false)
        {
            var accessRight = AccessRightEnum.NoRight;
            if (_parameters.VulnerabilityService.HasRightToAccessTLDSProfile(CurrentUser, profile.ProfileId))
            {
                accessRight = AccessRightEnum.View;

                if (profile.Status == (int)TLDSProfileStatusEnum.SubmittedToSchool
                    || profile.Status == (int)TLDSProfileStatusEnum.AssociatedWithStudent
                    || CheckTLDSOnlyView(profile.EnrolmentYear.GetValueOrDefault())
                    || (!upcomingSchoolSubmit && profile.Status == (int)TLDSProfileStatusEnum.CreatedUnsubmitted))
                    accessRight = AccessRightEnum.View;

                else if (_parameters.VulnerabilityService.HasRightToUpdateTLDSProfile(CurrentUser,
                    profile.UserID ?? 0, CurrentUser.GetMemberListDistrictId()))
                    accessRight = AccessRightEnum.Update;
            }
            return accessRight;
        }

        public ActionResult UploadEYALTReport(HttpPostedFileBase postedFile, int? profileId)
        {
            if (!IsValidPostedFile(postedFile))
            {
                return Json(new { message = "Invalid file, please try again.", success = false, type = "error" },
                            JsonRequestBehavior.AllowGet);
            }
            if (!profileId.HasValue || profileId.Value == 0)
            {
                return Json(new { success = false, ErrorMessage = "Invalid Profile Id" }, JsonRequestBehavior.AllowGet);
            }

            if (!_parameters.VulnerabilityService.HasRightToCreateTLDSProfile(CurrentUser))
            {
                return Json(new { success = false, ErrorMessage = "Has no right" }, JsonRequestBehavior.AllowGet);
            }

            var profile = _parameters.TLDSService.GetTLDSProfile(profileId.Value);
            if (profile == null)
            {
                return Json(new { success = false, ErrorMessage = "Invalid Profile Id" }, JsonRequestBehavior.AllowGet);
            }

            if (!_parameters.VulnerabilityService.HasRightToUpdateTLDSProfile(CurrentUser, profile.UserID ?? 0,
                CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { success = false, ErrorMessage = "Has no right" }, JsonRequestBehavior.AllowGet);
            }
            
            try
            {
                var tldsS3Setting = TLDSS3Settings.GetTLDSS3Settings(profileId.Value);
                if (PdfHelper.ValidatePDF(postedFile.InputStream))
                {
                    try
                    {
                        var originalFileName = postedFile.FileName;
                        var s3FileName = originalFileName.AddTimestampToFileName();
                        string uploadFileName = tldsS3Setting.GetEYALTUploadedPath(profile.ProfileId, s3FileName);
                        var s3Result = _s3Service.UploadRubricFile(tldsS3Setting.TLDSBucket, uploadFileName, postedFile.InputStream, false);
                        if (s3Result.IsSuccess)
                        {
                            return Json(new { Success = true, OriginalFileName = originalFileName, S3FileName = s3FileName, fileNameUrl = GetLinkToDownloadUploadFile(uploadFileName) }, JsonRequestBehavior.AllowGet);
                        }

                        return Json(new { Success = false, ErrorMessage = s3Result.ErrorMessage }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return Json(new { success = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { message = "This PDF file contains an error and could not be read correctly. Please contact the person who created the document to correct the problem, or contact the service desk for further advice.", success = false, type = "error" },
                        JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { message = "This PDF file contains an error and could not be read correctly. Please contact the person who created the document to correct the problem, or contact the service desk for further advice.", success = false, type = "error" },
                            JsonRequestBehavior.AllowGet);
            }
        }
    }
}
