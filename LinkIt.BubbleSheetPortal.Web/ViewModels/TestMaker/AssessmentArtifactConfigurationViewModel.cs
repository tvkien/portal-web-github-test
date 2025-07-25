using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TestMaker
{
    public class AssessmentArtifactConfigurationViewModel
    {
        public bool ShowSetting =>
            this.AssessmentArtifactFileTypeGroups != null && AssessmentArtifactFileTypeGroups.Any();

        public IEnumerable<AssessmentArtifactRecordingOptionViewModel> RecordingOptions { get; private set; }

        public IEnumerable<AssessmentArtifactFileTypeGroupViewModel> AssessmentArtifactFileTypeGroups
        {
            get;
            private set;
        }

        public AssessmentArtifactConfigurationViewModel(
            IEnumerable<AssessmentArtifactFileTypeGroupViewModel> assessmentArtifactFileTypeGroups, IEnumerable<AssessmentArtifactRecordingOptionViewModel> recordingOptions)
        {
            this.AssessmentArtifactFileTypeGroups = assessmentArtifactFileTypeGroups?.OrderBy(x => x.Order).ToArray() ??
                                                    Enumerable.Empty<AssessmentArtifactFileTypeGroupViewModel>();

            this.RecordingOptions = recordingOptions?.OrderBy(o => o.Order).ToArray() ?? Enumerable.Empty<AssessmentArtifactRecordingOptionViewModel>();
        }
    }
}
