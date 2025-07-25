using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public static class TLDSModelExtension
    {
        public static bool IsEmpty(this TLDSAdditionalInformation obj)
        {
            if (obj == null)
            {
                return true;
            }
            return string.IsNullOrEmpty(obj.AreasOfNote) &&
                   string.IsNullOrEmpty(obj.StrategiesForEnhancedSupport) && obj.DateCreated == null;

        }
        public static bool IsEmpty(this TLDSDevelopmentOutcome obj)
        {
            if (obj == null)
            {
                return true;
            }
            return string.IsNullOrEmpty(obj.Name);

        }
        public static bool IsEmpty(this TLDSDevelopmentOutcomeProfile obj)
        {
            if (obj == null)
            {
                return true;
            }
            return string.IsNullOrWhiteSpace(obj.DevelopmentOutcomeContent)
                   && string.IsNullOrWhiteSpace(obj.StrategyContent);

        }
        public static bool IsEmpty(this TLDSEarlyABLESReport obj)
        {
            if (obj == null)
            {
                return true;
            }
            return string.IsNullOrEmpty(obj.ReportName) && obj.ReportDate == null
                   && obj.LearningReadinessReportCompleted == false
                   && obj.AvailableOnRequest == false;

        }
        public static bool IsEmpty(this TLDSOtherReportPlan obj)
        {
            if (obj == null)
            {
                return true;
            }
            return string.IsNullOrEmpty(obj.ReportName) && obj.ReportDate == null
                   && obj.AvailableOnRequest == false;

        }
        public static bool IsEmpty(this TLDSParentGuardian obj)
        {
            if (obj == null)
            {
                return true;
            }
            return string.IsNullOrWhiteSpace(obj.ParentGuardianName)
                   && !string.IsNullOrWhiteSpace(obj.ParentGuardianRelationship)
                   && !string.IsNullOrWhiteSpace(obj.ParentGuardianPhone)
                   && !string.IsNullOrWhiteSpace(obj.ParentGuardianEmail);

        }
        public static bool IsEmpty(this TLDSProfessionalService obj)
        {
            if (obj == null)
            {
                return true;
            }
            return string.IsNullOrEmpty(obj.Name) && string.IsNullOrEmpty(obj.Address) &&
                   string.IsNullOrEmpty(obj.ContactPerson)
                   && string.IsNullOrEmpty(obj.Position) && string.IsNullOrEmpty(obj.Phone) &&
                   string.IsNullOrEmpty(obj.Email) && obj.ReportForwardedToSchoolDate == null
                   && obj.WrittenReportAvailable.GetValueOrDefault() == false
                   && obj.AvailableUponRequested.GetValueOrDefault() == false;

        }

        public static string ConvertToJsonData(this TLDSUserMetaValueModel metaValueModel)
        {
            if (metaValueModel == null)
            {
                return null;
            }
            var s = new JavaScriptSerializer().Serialize(metaValueModel);
            return s;
        }
    }
}
