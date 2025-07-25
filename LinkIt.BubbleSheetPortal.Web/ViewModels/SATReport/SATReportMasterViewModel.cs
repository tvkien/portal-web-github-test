using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.ACTReport;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SATReport
{
    public class SATReportMasterViewModel
    {
        public List<SectionTagViewModel> SectionTagViewModels { get; set; }
        public SATDiagnosticHistoryViewModel DiagnosticHistoryViewModel { get; set; }
        public ACTReportStudentInformation StudentInformation { get; set; }
        public SATSummaryScoreViewModel SATSummaryScoreViewModel { get; set; }
        public List<BubbleSheetFileSubViewModel> BubbleSheetFileSubViewModels { get; set; }
        public string DistrictLogoUrl { get; set; }
        public string DistrictName { get; set; }
        public bool IsLastReportInList { get; set; }
        public int ReportContentOption { get; set; }

        public int EssaySectionId { get; set; }

        public string EssayCommentTitle { get; set; }
        public List<string> EssayComments { get; set; }
        public string DistrictReportBannerUrl { get; set; }

        public List<KeyValuePair<string, string>> TestScoreRanges { get; set; }

        public bool KNOWSYS_SATReport_SectionPageBreak { get; set; }
        public bool KNOWSYS_SATReport_ShowScoreRange { get; set; }
        public bool KNOWSYS_SATReport_ShowSectionScoreScaled { get; set; }
        public bool KNOWSYS_SATReport_ShowAssociatedTagName { get; set; }
        public bool KNOWSYS_SATReport_ShowEssay { get; set; }
        public bool KNOWSYS_SATReport_ShowComment { get; set; }

        public string StateInformationImageUrl { get; set; }
        public int VirtualTestSubTypeId { get; set; }

        public List<ACTSectionTagData> DomainTagData { get; set; }

        public List<string> ListVirtualTestConversionName { get; set; }
        public bool ShowTableBorder { get; set; }
        public int DistrictId { get; set; }

        public bool TagTableUseAlternativeStyle { get; set; }
        public bool BoldZeroPercentScore { get; set; }

        public List<NewSATEssayComment> NewSATEssayComments;

        public List<string> JS { get; set; }
        public List<string> Css { get; set; }

        public SATReportMasterViewModel()
        {
            JS = new List<string>();
            Css = new List<string>();
        }
    }
}