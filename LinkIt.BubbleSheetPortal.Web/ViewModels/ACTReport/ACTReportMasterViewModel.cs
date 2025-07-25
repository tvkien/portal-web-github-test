using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.ACTReport;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport
{
    public class ACTReportMasterViewModel
    {
        public List<SectionTagViewModel> SectionTagViewModels { get; set; } 
        public DiagnosticHistoryViewModel DiagnosticHistoryViewModel { get; set; }
        public ACTReportStudentInformation StudentInformation { get; set; }
        public ACTSummaryScoreViewModel ACTSummaryScoreViewModel { get; set; }
        public List<BubbleSheetFileSubViewModel> BubbleSheetFileSubViewModels { get; set; }
        public string DistrictLogoUrl { get; set; }
        public string DistrictName { get; set; }
        public bool IsLastReportInList { get; set; }
        public int ReportContentOption { get; set; }
        public string EssayCommentTitle { get; set; }
        public List<string> EssayComments { get; set; }
        public string DistrictReportBannerUrl { get; set; }
        public int VirtualTestSubTypeId { get; set; }

        public List<ACTSectionTagData> DomainTagData { get; set; }
        public bool UseNewACTStudentFormat { get; set; }
        public bool ShowTableBorder { get; set; }
        public int DistrictId { get; set; }

        public bool TagTableUseAlternativeStyle { get; set; }
        public bool BoldZeroPercentScore { get; set; }

        public List<NewACTEssayComment> NewACTEssayComments;

        public List<string> JS { get; set; }
        public List<string> Css { get; set; }

        public ACTReportMasterViewModel()
        {
            JS = new List<string>();
            Css = new List<string>();
        }
    }
}