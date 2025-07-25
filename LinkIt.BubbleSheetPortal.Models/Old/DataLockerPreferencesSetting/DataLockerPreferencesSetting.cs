using LinkIt.BubbleSheetPortal.Common;
using System;

namespace LinkIt.BubbleSheetPortal.Models.DataLockerPreferencesSetting
{
    public class DisplayPerformanceBandsInEnterResults
    {
        public string AllowDisplay { get; set; } = "1";
        public bool Lock { get; set; }
    }
    public class AllowResultDateChange
    {
        public string AllowChange { get; set; } = "1";
        public bool Lock { get; set; }
    }
    public class ExpriedOn
    {
        public string DateExpried { get; set; }
        public string TimeExpried { get; set; }
        public string TypeExpiredOn { get; set; }
        public bool Lock { get; set; }
        public string DateDeadline { get; set; }
    }
    public class PublishingToStudentPortal
    {
        public string AllowPublishing { get; set; }
        public bool Lock { get; set; }
    }
    public class ModificationUploadedArtifacts
    {
        public string AllowModification { get; set; }
        public bool Lock { get; set; }
    }
    public class Upload
    {
        public string AllowUpload { get; set; }
        public bool Lock { get; set; }
    }
    public class AudioRecording
    {
        public string AllowRecording { get; set; }
        public bool Lock { get; set; }
    }
    public class VideoRecording
    {
        public string AllowRecording { get; set; }
        public bool Lock { get; set; }
    }
    public class CameraCapture
    {
        public string AllowCapture { get; set; }
        public bool Lock { get; set; }
    }
    public class DataSettings
    {
        public DisplayPerformanceBandsInEnterResults DisplayPerformanceBandsInEnterResults { get; set; } = new DisplayPerformanceBandsInEnterResults();
        public AllowResultDateChange AllowResultDateChange { get; set; } = new AllowResultDateChange();
        public ExpriedOn ExpriedOn { get; set; } = new ExpriedOn();
        public PublishingToStudentPortal PublishingToStudentPortal { get; set; } = new PublishingToStudentPortal();
        public ModificationUploadedArtifacts ModificationUploadedArtifacts { get; set; } = new ModificationUploadedArtifacts();
        public Upload Upload { get; set; } = new Upload();
        public AudioRecording AudioRecording { get; set; } = new AudioRecording();
        public VideoRecording VideoRecording { get; set; } = new VideoRecording();
        public CameraCapture CameraCapture { get; set; } = new CameraCapture();            
    }
    public class DataLockerPreferenceLocalize
    {
        public string DisplayPerformanceBandsInEnterResults { get; set; }
        public string AllowResultDateChange { get; set; }
        public string ExpriedOn { get; set; }
        public string PublishingToStudentPortal { get; set; }
        public string ModificationUploadedArtifacts { get; set; }
        public string Upload { get; set; }
        public string AudioRecording { get; set; }
        public string VideoRecording { get; set; }
        public string CameraCapture { get; set; }
    }
    public class DataLockerPreferencesSettingModal
    {
        public int LevelId { get; set; }    
        public DateTime? LastUpdatedDate { get; set; }
        public string LastUpdatedDateString { get { return LastUpdatedDate.DisplayDateWithFormat(true); } }
        public int LastUpdatedBy { get; set; }
        public string LastUpdatedByUser { get; set; }
        public DataSettings DataSettings { get; set; }
        public DataSettings ParentDataSettings { get; set; }

        public DataLockerPreferencesSettingModal()
        {
            DataSettings = new DataSettings();
        }
    }
  
}
