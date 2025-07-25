using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TestMaker
{
    public class AssessmentArtifactFileTypeGroupViewModel
    {
          public string DisplayName { get; set; }
          public string Name { get; set; }
          public int Order { get; set; }
          public int MaxFileSizeInBytes { get; set; }
          public List<string> SupportFileType { get; set; }
    }

    public class AssessmentArtifactRecordingOptionViewModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int Order { get; set; }
    }
}
