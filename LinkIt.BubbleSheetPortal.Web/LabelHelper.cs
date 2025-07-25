using LinkIt.BubbleSheetPortal.Web.Helpers;
using System.Web;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Web.Constant;
using System;
namespace LinkIt.BubbleSheetPortal.Web
{
    public class LabelHelper
    {
        public static string DistrictLabel
        {
            get
            {
                if (HttpContext.Current.Session["DistrictLabel"] == null)
                {
                    var configurationService = DependencyResolver.Current.GetService<LinkIt.BubbleSheetPortal.Services.ConfigurationService>();
                    HttpContext.Current.Session["DistrictLabel"] = configurationService.GetConfigurationByKeyWithDefaultValue("DistrictLabel", "District");
                }

                return HttpContext.Current.Session["DistrictLabel"].ToString();
            }
        }

        public static string GradeLabel
        {
            get
            {
                if (HttpContext.Current.Session["GradeLabel"] == null)
                {
                    var configurationService = DependencyResolver.Current.GetService<LinkIt.BubbleSheetPortal.Services.ConfigurationService>();
                    HttpContext.Current.Session["GradeLabel"] = configurationService.GetConfigurationByKeyWithDefaultValue("GradeLabel", "Grade");
                }

                return HttpContext.Current.Session["GradeLabel"].ToString();
            }
        }

        public static string DistrictLabels
        {
            get
            {
                if (HttpContext.Current.Session["DistrictLabels"] == null)
                {
                    var configurationService = DependencyResolver.Current.GetService<LinkIt.BubbleSheetPortal.Services.ConfigurationService>();
                    HttpContext.Current.Session["DistrictLabels"] = configurationService.GetConfigurationByKeyWithDefaultValue("DistrictLabels", "Districts");
                }

                return HttpContext.Current.Session["DistrictLabels"].ToString();
            }
        }

        public static string GradeLabels
        {
            get
            {
                if (HttpContext.Current.Session["GradeLabels"] == null)
                {
                    var configurationService = DependencyResolver.Current.GetService<LinkIt.BubbleSheetPortal.Services.ConfigurationService>();
                    HttpContext.Current.Session["GradeLabels"] = configurationService.GetConfigurationByKeyWithDefaultValue("GradeLabels", "Grades");
                }

                return HttpContext.Current.Session["GradeLabels"].ToString();
            }
        }

        public static string GradeLabelShort
        {
            get
            {
                if (HttpContext.Current.Session["GradeLabelShort"] == null)
                {
                    var configurationService = DependencyResolver.Current.GetService<LinkIt.BubbleSheetPortal.Services.ConfigurationService>();
                    HttpContext.Current.Session["GradeLabelShort"] = configurationService.GetConfigurationByKeyWithDefaultValue("GradeLabelShort", "Gr.");
                }

                return HttpContext.Current.Session["GradeLabelShort"].ToString();
            }
        }


        public static string CodeLabel
        {
            get
            {
                if (HttpContext.Current.Session["CodeLabel"] == null)
                {
                    int districtID = HttpContext.Current.GetCurrentDistrictID();
                    HttpContext.Current.Session["CodeLabel"] = GetLabelInDistrictDecode(districtID, "CodeLabel", "Code");
                }
                return HttpContext.Current.Session["CodeLabel"].ToString();
            }
        }

        public static string CodedLabel
        {
            get
            {
                if (HttpContext.Current.Session["CodedLabel"] == null)
                {
                    int districtID = HttpContext.Current.GetCurrentDistrictID();
                    HttpContext.Current.Session["CodedLabel"] = GetLabelInDistrictDecode(districtID, "CodedLabel", "Coded");
                }
                return HttpContext.Current.Session["CodedLabel"].ToString();
            }
        }

        public static string CodingLabel
        {
            get
            {
                if (HttpContext.Current.Session["CodingLabel"] == null)
                {
                    int districtID = HttpContext.Current.GetCurrentDistrictID();
                    HttpContext.Current.Session["CodingLabel"] = GetLabelInDistrictDecode(districtID, "CodingLabel", "Coding");
                }

                return HttpContext.Current.Session["CodingLabel"].ToString();
            }
        }

        public static string PointsLabel
        {
            get
            {
                if (HttpContext.Current.Session["PointsLabel"] == null)
                {
                    int districtID = HttpContext.Current.GetCurrentDistrictID();
                    HttpContext.Current.Session["PointsLabel"] = GetLabelInDistrictDecode(districtID, "PointsLabel", "Points");
                }

                return HttpContext.Current.Session["PointsLabel"].ToString();
            }
        }

        public static string QuestionLabel
        {
            get
            {
                if (HttpContext.Current.Session["QuestionLabel"] == null)
                {
                    int districtID = HttpContext.Current.GetCurrentDistrictID();
                    HttpContext.Current.Session["QuestionLabel"] = GetLabelInDistrictDecode(districtID, "QuestionLabel", "Question");
                }

                return HttpContext.Current.Session["QuestionLabel"].ToString();
            }
        }

        public static string ViewRubricLabel
        {
            get
            {
                if (HttpContext.Current.Session["ViewRubricLabel"] == null)
                {
                    int districtID = HttpContext.Current.GetCurrentDistrictID();
                    HttpContext.Current.Session["ViewRubricLabel"] = GetLabelInDistrictDecode(districtID, "ViewRubricLabel", "View Rubric");
                }

                return HttpContext.Current.Session["ViewRubricLabel"].ToString();
            }
        }
        
        private static string GetLabelInDistrictDecode(int districtID, string label, string defaultValue)
        {
            var districtDecodeService = DependencyResolver.Current.GetService<Services.DistrictDecodeService>();
            var districtDecode = districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtID, label);

            return districtDecode != null ? districtDecode.Value : defaultValue;
        }

        public static string StudentRace => GetLocalize(LocalizeResource.StudentRace, "Race");
        public static string SchoolCode => GetLocalize(LocalizeResource.SchoolCode, "Code");
        public static string SchoolStateCode => GetLocalize(LocalizeResource.SchoolStateCode, "State Code");
        public static string TestGrade => GetLocalize(LocalizeResource.TestGrade, "Grade");
        public static string TestGrades => GetLocalize(LocalizeResource.TestGrades, "Grades");
        public static string StudentGrade => GetLocalize(LocalizeResource.StudentGrade, "Grade");
        public static string Term => GetLocalize(LocalizeResource.Term, "Term");
        public static string Terms => GetLocalize(LocalizeResource.Terms, "Terms");
        public static string Subject => GetLocalize(LocalizeResource.Subject, "Subject");
        public static string Subjects => GetLocalize(LocalizeResource.Subjects, "Subjects");
        public static string StudentStateID => GetLocalize(LocalizeResource.StudentStateID, "State ID");

        public static string GetLocalize(string key, string defaultValue = null)
        {
            var localizeText = LocalizeHelper.LocalizedToString(key);
            return String.IsNullOrEmpty(localizeText)? defaultValue ?? key:localizeText;
        }
    }
}
