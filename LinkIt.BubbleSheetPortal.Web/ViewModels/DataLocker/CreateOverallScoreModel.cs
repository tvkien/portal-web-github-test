using LinkIt.BubbleSheetPortal.Web.ViewModels.TestMaker;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker
{
    public class CreateOverallScoreModel
    {
        public CreateOverallScoreModel()
        {
            UploadFileTypeList = new List<string>();
            AssessmentArtifactFileTypeGroupViewModel = new List<AssessmentArtifactFileTypeGroupViewModel>();
        }

        public int TemplateId { get; set; }
        public int SubscoreId { get; set; }
        public List<string> UploadFileTypeList { get; set; }
        public int MaxFileSize { get; set; }
        public List<AssessmentArtifactFileTypeGroupViewModel> AssessmentArtifactFileTypeGroupViewModel { get; set; }
    }
}
