using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.TLDS;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Web.ViewModels.TLDSDigital;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TDLSProfileViewModel
    {
        public TDLSProfileViewModel()
        {
            TLDSInformationHasBeenSaved = false;
            SaveSuccessful = false;
            AccessRight = AccessRightEnum.NoRight;
            TLDSProfileTeacherSelectedItem = new List<SelectListItem>();
            TLDSGroupSelectedItem = new List<SelectListItem>();
            EYALTFileImages = new List<string>();
        }

        public int ProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GenderId { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DateOfBirthString
        {
            get { return DateOfBirth.HasValue ? DateOfBirth.Value.DisplayDateWithFormat() : ""; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    DateTime date = DateTime.MinValue;
                    value.TryParseDateWithFormat(out date);
                    DateOfBirth = date;
                }
            }
        }
        public int? EnrolmentYear { get; set; }
        public string PrimarySchool { get; set; }
        public string OutsideSchoolHoursCareService { get; set; }
        public string PhotoURL { get; set; }
        public string FileName { get; set; }

        public string GuardianName { get; set; }
        public string GuardianRelationship { get; set; }
        public string GuardianPhone { get; set; }
        public string GuardianEmail { get; set; }
        public List<TLDSGuardiantContactDetailViewModel> GuardiantContactDetails { get; set; }

        public string ECSName { get; set; }
        public string ECSAddress { get; set; }
        public string ECSApprovalNumber { get; set; }
        public string ECSCompletingFormEducatorName { get; set; }
        public string ECSCompletingFormEducatorPosition { get; set; }
        public string ECSCompletingFormEducatorQualification { get; set; }
        public int ECSCompletingFormEducatorQualificationId { get; set; }
        public string ECSCompletingFormEducatorPhone { get; set; }
        public string ECSCompletingFormEducatorEmail { get; set; }
        public DateTime? ECSCompledDate { get; set; }
        public string ECSCompledDateString
        {
            get { return ECSCompledDate.HasValue ? ECSCompledDate.Value.DisplayDateWithFormat() : ""; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    DateTime date = DateTime.MinValue;
                    value.TryParseDateWithFormat(out date);
                    ECSCompledDate = date;
                }
            }
        }
        public string SectionChildInputFileUrl { get; set; }
        public string SectionFamilyFileUrl { get; set; }
        public bool? Section102IsNotRequired { get; set; }
        public string NoKWName { get; set; }
        public string NoKWPosition { get; set; }
        public string NoKWPhone { get; set; }
        public string NoKWEmail { get; set; }
        public bool ABLESReportCompleted { get; set; }
        public bool EARReportCompleted { get; set; }
        public DateTime? EARReportDate { get; set; }
        public string EARReportDateString
        {
            get { return EARReportDate.HasValue ? EARReportDate.Value.DisplayDateWithFormat() : ""; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    DateTime date = DateTime.MinValue;
                    value.TryParseDateWithFormat(out date);
                    EARReportDate = date;
                }
            }
        }
        public string EARAttachmentURL { get; set; }
        public bool EARAvailableUponRequest { get; set; }
        public int? UpcommingSchoolID { get; set; }
        public string UpcommingSchoolName { get; set; }
        public int? UpcommingDistrictID { get; set; }
        public int Status { get; set; }
        public int UserID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public List<SelectListItem> Genders { get; set; }
        public List<TLDSDevelopmentOutcomeProfileViewModel> DevelopmentOutcomeProfiles { get; set; }
        public List<TLDSOtherReportPlanViewModel> OtherReportPlans { get; set; }
        public List<TLDSProfessionalServiceViewModel> ProfessionalServices { get; set; }
        public List<TLDSAdditionalInformationViewModel> AdditionalInformation { get; set; }

        public string ProfessionalServiceData { get; set; }
        public string OtherReportPlanData { get; set; }
        public string AdditionalInformationData { get; set; }
        public bool IsContinue { get; set; }
        public bool IsPublisher { get; set; }

        public bool IsNetworkAdmin { get; set; }

        public bool IsDistrictAdmin { get; set; }

        public bool IsSchoolAdmin { get; set; }

        public int CurrentDistrictId { get; set; }
        public List<int> ListDistricIds { get; set; }

        public List<string> ErrorList { get; set; }
        public List<SelectListItem> QualificationList { get; set; }
        public bool TLDSInformationHasBeenSaved { get; set; }
        private TLDSUserConfigurations _tldsUserConfigurations;

        public TLDSUserConfigurations TLDSUserConfigurations
        {
            get
            {
                if (_tldsUserConfigurations == null)
                {
                    _tldsUserConfigurations = new TLDSUserConfigurations();
                }
                return _tldsUserConfigurations;
            }
            set { _tldsUserConfigurations = value; }
        }
        public string ContextOfEarlyYearsSetting { get; set; }
        public string SpecificInformation { get; set; }

        public bool ContextSpecificInforHasBeenSaved { get; set; }//Section 1
        public bool DevelopmentOutcomeHasBeenSaved { get; set; }
        public bool? WasAnEarlyABLESReportCompleted { get; set; }
        public bool Section102HasBeenSaved { get; set; }
        public bool ParentConsentIsIncluded { get; set; }
        public bool SectionChildParentCompleted { get; set; }
        public bool PrintAllSectionsFamily { get; set; }
        private List<TLDSEarlyABLESReportViewModel> _TLDSEarlyABLESReport;

        public List<TLDSEarlyABLESReportViewModel> TLDSEarlyABLESReport
        {
            get
            {
                if (_TLDSEarlyABLESReport == null)
                {
                    _TLDSEarlyABLESReport = new List<TLDSEarlyABLESReportViewModel>();
                }
                return _TLDSEarlyABLESReport;
            }
            set { _TLDSEarlyABLESReport = value; }
        }
        public char? WillAttendASchoolInVictoria { get; set; }// Unknown(U), Yes(Y), No(N)
        public bool? HasParentSharedStatementWithSchool { get; set; }
        public bool? SchoolNotListed { get; set; }
        public string TLDSEarlyABLESReportData { get; set; }
        public bool? Section2CheckedCompleted { get; set; }
        public bool? Section3CheckedCompleted { get; set; }
        public bool SaveSuccessful { get; set; }
        public string GuardiantContactJSONData { get; set; }
        public DateFormatModel DateFormatModel { get; set; }
        public AccessRightEnum AccessRight { get; set; }

        public bool Step4IsEnable
        {
            get { return Status >= (int)TLDSProfileStatusEnum.Draft; }
        }
        public bool Step5IsEnable
        {
            get { return DevelopmentOutcomeHasBeenSaved; }
        }
        public bool Step6IsEnable
        {
            get { return DevelopmentOutcomeHasBeenSaved; }
        }
        public bool Step7IsEnable
        {
            get { return DevelopmentOutcomeHasBeenSaved && SectionChildParentCompleted; }
        }
        public string UploadedChildFormFileName { get; set; }
        public string UploadedFamilyFormFileName { get; set; }
        public DateTime? LastStatusDate { get; set; }
        public List<TLDSParentGuardian> TLDSParentGuardians { get; set; }
        public List<TLDSUploadedDocument> TLDSUploadedDocuments { get; set; }
        public string TLDSUploadedDocumentData { get; set; }
        public bool? AboriginalValue { get; set; }

        public string HasTheFamilyIndicatedAboriginal { get; set; }
        public string HasProvidedTransitionStatement { get; set; } // True, False
        public string IsAwareTransitionChildSchoolAndOSHC { get; set; } // True, False
        public string IsFamilyDidNotCompleteSection3 { get; set; } // True, False
        public string IsfamilyOptedOutTransitionStatement { get; set; } // True, False

        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
                {
                    return string.Format("{0} {1}", FirstName, LastName);
                }
                else
                {
                    return string.Format("{0}{1}", FirstName, LastName);
                }
            }
        }
        public List<string> FileImages { get; set; }
        public string IsFamilyDidNotCompleteSection2 { get; set; } // True, False

        public bool IsFormSection2Submitted { get; set; }
        public bool IsFormSection3Submitted { get; set; }
        public List<TldsFormSection2ViewModel> TLDSFormSection2 { get; set; }
        public List<TldsFormSection3ViewModel> TLDSFormSection3 { get; set; }
        public List<TLDSDigitalExportPDFViewModel> TLDSFormSectionExport { get; set; }
        public bool AutoSaving { get; set; }

        public List<SelectListItem> TLDSProfileTeacherSelectedItem { get; set; }

        public List<SelectListItem> TLDSGroupSelectedItem { get; set; }

        public int? TldsGroupId { get; set; }
        public int RotatePhoto { get; set; }
        public List<string> EYALTFileImages { get; set; }
        public bool HasEYALT { get; set; }
    }
}
