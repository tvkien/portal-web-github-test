using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportDownloadFilterDto
    {
        public NavigatorReportDownloadFilterDto()
        {
            DistrictTerms = new List<DistrictTermDto>();
            Classes = new List<ClassDto>();
        }
        public List<DistrictTermDto> DistrictTerms { get; set; }
        public List<ClassDto> Classes { get; set; }
        public List<SelectListItemDTO> Students { get; set; }
    }

    public class DistrictTermDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ClassDto
    {
        public int Id { get; set; }
        public int DistrictTermId { get; set; }
        public string Name { get; set; }
    }
}
