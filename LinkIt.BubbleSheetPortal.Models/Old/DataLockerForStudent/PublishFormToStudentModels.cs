using LinkIt.BubbleSheetPortal.Models.DataLockerPreferencesSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent
{
    public class PublishFormToStudentModels
    {
        public int VirtualTestID { get; set; }
        public int ClassID { get; set; }
        public PreferencesValueJson ValueOject { get; set; }
    }

    public class PreferencesValueJson
    {
        public ExpriedOn ExpriedOn { get; set; }
        public PublishingToStudentPortal PublishingToStudentPortal { get; set; }
        public ModificationUploadedArtifacts ModificationUploadedArtifacts { get; set; }
        public Upload Upload { get; set; }
        public AudioRecording AudioRecording { get; set; }
        public VideoRecording VideoRecording { get; set; }
        public CameraCapture CameraCapture { get; set; }
    }
}
