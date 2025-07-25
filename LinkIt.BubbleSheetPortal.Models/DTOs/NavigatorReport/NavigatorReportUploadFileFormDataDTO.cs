using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportUploadFileFormDataDTO
    {
        public static NavigatorReportUploadFileFormDataDTO InitFromNameValueCollection(NameValueCollection formData)
        {
            try
            {
                var map = from a in typeof(NavigatorReportUploadFileFormDataDTO).GetProperties()
                          join b in formData.AllKeys.Where(c => c.Length > 6)
                          on a.Name.ToLower() equals b.Substring(6).ToLower()
                          select new
                          {
                              prop = a
                              ,
                              val = formData[b] == "null" || string.IsNullOrEmpty(formData[b]) ? null : formData[b]
                          };
                NavigatorReportUploadFileFormDataDTO res = new NavigatorReportUploadFileFormDataDTO();
                foreach (var item in map)
                {
                    if (!string.IsNullOrEmpty(item.val))
                    {
                        var desVal = Convert.ChangeType(item.val, item.prop.PropertyType);
                        item.prop.SetValue(res, desVal, null);
                    }
                }
                if (res.School < 0)
                {
                    res.School = 0;
                }
                if (res.ReportingPeriod < 0)
                {
                    res.ReportingPeriod = 0;
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string HasCode { get; set; }
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Please input State")]
        public int State { get; set; }

        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Please input District")]
        public int District { get; set; }

        public int School { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SchoolYear { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please input Category")]
        public int NavigatorCategory { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please input Report Type")]
        public int ReportType { get; set; }

        public int ReportingPeriod { get; set; }

        public string KeywordShortNames { get; set; }
        public string KeywordIds { get; set; }
        [StringLength(30, ErrorMessage = "Report Suffix should not be more than {1} characters")]
        public string ReportSuffix { get; set; }
    }
}
