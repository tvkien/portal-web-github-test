using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.TLDS;
using LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TLDSDigital
{
    public class TldsProfileDigitalViewModel
    {
        public TldsProfileDigitalViewModel()
        {
            EYALTFileImages = new List<string>();
        }

        public int ProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GenderId { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PrimarySchool { get; set; }
        public string OutsideSchoolHoursCareService { get; set; }
        public string PhotoURL { get; set; }
        public string FileName { get; set; }

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
        
        public string EARAttachmentURL { get; set; }

        public bool EARAvailableUponRequest { get; set; }

        public int? UpcommingSchoolID { get; set; }

        public int Status { get; set; }

        public int UserID { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public List<TLDSDevelopmentOutcomeProfileViewModel> DevelopmentOutcomeProfiles { get; set; }

        public List<TLDSOtherReportPlanViewModel> OtherReportPlans { get; set; }

        public List<TLDSProfessionalServiceViewModel> ProfessionalServices { get; set; }

        public List<TLDSAdditionalInformationViewModel> AdditionalInformation { get; set; }

        public bool TLDSInformationHasBeenSaved { get; set; }

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


        public bool? Section2CheckedCompleted { get; set; }

        public bool? Section3CheckedCompleted { get; set; }

        public string UploadedChildFormFileName { get; set; }

        public string UploadedFamilyFormFileName { get; set; }

        public DateTime? LastStatusDate { get; set; }

        public List<TLDSParentGuardian> TLDSParentGuardians { get; set; }

        public string HasProvidedTransitionStatement { get; set; } // True, False

        public string IsAwareTransitionChildSchoolAndOSHC { get; set; } // True, False

        public string IsFamilyDidNotCompleteSection3 { get; set; } // True, False

        public string IsfamilyOptedOutTransitionStatement { get; set; } // True, False

        public string IsFamilyDidNotCompleteSection2 { get; set; } // True, False

        public TldsFormSection2ViewModel SectionPrint2 { get; set; }

        public TldsFormSection3ViewModel SectionPrint3 { get; set; }

        public bool IsPrinting { get; set; }

        public string DateOfBirthFormated { get; set; }

        public string ECSCompledDateFormated { get; set; }

        public List<string> EYALTFileImages { get; set; }
    }
}
