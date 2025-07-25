using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSProfile
    {
        public int ProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? GenderId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? EnrolmentYear { get; set; }

        public string PrimarySchool { get; set; }
        public string OutsideSchoolHoursCareService { get; set; }
        public string PhotoURL { get; set; }

        public string ECSName { get; set; }
        public string ECSAddress { get; set; }
        public string ECSApprovalNumber { get; set; }
        public string ECSCompletingFormEducatorName { get; set; }
        public string ECSCompletingFormEducatorPosition { get; set; }
        public string ECSCompletingFormEducatorQualification { get; set; }
        public int? ECSCompletingFormEducatorQualificationId { get; set; }
        public string ECSCompletingFormEducatorPhone { get; set; }
        public string ECSCompletingFormEducatorEmail { get; set; }
        public DateTime? ECSCompledDate { get; set; }
        public bool TLDSInformationHasBeenSaved { get; set; }
        public string ContextOfEarlyYearsSetting { get; set; }
        public string SpecificInformation { get; set; }
        public bool ContextSpecificInforHasBeenSaved { get; set; }
        public bool DevelopmentOutcomeHasBeenSaved { get; set; }
        public string SectionChildInputFileUrl { get; set; }
        public string SectionFamilyFileUrl { get; set; }
        public string SectionParentConsentFileUrl { get; set; }
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
        public bool Section102HasBeenSaved { get; set; }
        public string UploadedChildFormFileName { get; set; }
        public string UploadedFamilyFormFileName { get; set; }
        public int? UpcommingSchoolID { get; set; }
        public bool ParentConsentIsIncluded { get; set; }
        public bool SectionChildParentCompleted { get; set; }
        public bool PrintAllSectionsFamily { get; set; }
        public bool? WasAnEarlyABLESReportCompleted { get; set; }
        public char? WillAttendASchoolInVictoria { get; set; }// Unknown(U), Yes(Y), No(N)
        public bool? HasParentSharedStatementWithSchool { get; set; }
        public bool? SchoolNotListed { get; set; }
        public bool? Section2CheckedCompleted { get; set; }
        public bool? Section3CheckedCompleted { get; set; }
        public bool? Section4CheckedCompleted { get; set; }

        public int? Status { get; set; }
        public DateTime? LastStatusDate { get; set; }
        public int? UserID { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? StudentID { get; set; }

        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string RejectedReason { get; set; }
        public string UploadedStatementPdfFileName { get; set; }
        public bool? IsUploadedStatement { get; set; }
        
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(FirstName))
                {
                    return string.Format("{0}{1}", LastName, FirstName);
                }
                else
                {
                    return string.Format("{0}, {1}", LastName, FirstName);
                }
            } 
        }

        public bool? IsAboriginal { get; set; }

        #region TLDSProfileMetaData
        public string HasTheFamilyIndicatedAboriginal { get; set; } // Yes, No, Unknown
        public string HasProvidedTransitionStatement { get; set; } // Yes, No
        public string IsAwareTransitionChildSchoolAndOSHC { get; set; } // Yes, No
        public string IsFamilyDidNotCompleteSection3 { get; set; } // Yes, No
        public string IsfamilyOptedOutTransitionStatement { get; set; } // Yes, No

        public List<TLDSProfileMeta> TLDSProfileMetaes { get; set; }
        public string IsFamilyDidNotCompleteSection2 { get; set; } // Yes, No
        #endregion
    }
}
