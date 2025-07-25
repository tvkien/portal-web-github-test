using LinkIt.BubbleSheetPortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class LabelService
    {
        ConfigurationService _configurationService;
        public LabelService(ConfigurationService configurationService)
        {
            this._configurationService = configurationService;
        }

        public string DistrictLabel
        {
            get
            {
                if (HttpContext.Current.Session["DistrictLabel"] == null)
                {
                    HttpContext.Current.Session["DistrictLabel"] = _configurationService.GetConfigurationByKeyWithDefaultValue("DistrictLabel", "District");
                }

                return HttpContext.Current.Session["DistrictLabel"].ToString();
            }
        }
        public string GradeLabel
        {
            get
            {
                if (HttpContext.Current.Session["GradeLabel"] == null)
                {
                    HttpContext.Current.Session["GradeLabel"] = _configurationService.GetConfigurationByKeyWithDefaultValue("GradeLabel", "Grade");
                }

                return HttpContext.Current.Session["GradeLabel"].ToString();
            }
        }
        public string DistrictLabels
        {
            get
            {
                if (HttpContext.Current.Session["DistrictLabels"] == null)
                {
                    HttpContext.Current.Session["DistrictLabels"] = _configurationService.GetConfigurationByKeyWithDefaultValue("DistrictLabels", "Districts");
                }

                return HttpContext.Current.Session["DistrictLabels"].ToString();
            }
        }
        public string GradeLabels
        {
            get
            {
                if (HttpContext.Current.Session["GradeLabels"] == null)
                {
                    HttpContext.Current.Session["GradeLabels"] = _configurationService.GetConfigurationByKeyWithDefaultValue("GradeLabels", "Grades");
                }

                return HttpContext.Current.Session["GradeLabels"].ToString();
            }
        }
        public string GradeLabelShort
        {
            get
            {
                if (HttpContext.Current.Session["GradeLabelShort"] == null)
                {
                    HttpContext.Current.Session["GradeLabelShort"] = _configurationService.GetConfigurationByKeyWithDefaultValue("GradeLabelShort", "Gr.");
                }

                return HttpContext.Current.Session["GradeLabelShort"].ToString();
            }
        }
    }
}
