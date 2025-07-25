using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LinkIt.BubbleSheetPortal.Models
{
    [Serializable]
    public class AttachmentForStudentModel
    {
        public DateTime ResultDate { get; set; }
        public string VirtualTestName { get; set; }
        public string TeacherName { get; set; }
        public string ClassName { get; set; }
        public string AttachmentName { get; set; }
        public string Attachments { get; set; }
        public int? VirtualTestId { get; set; }
        public int? TeacherID { get; set; }
        public int? ClassId { get; set; }
        public int FileNumber => Artifacts.Count;
        public string FilterJson { get; set; }
        public int? TestResultScoreID { get; set; }
        public int? TestResultSubScoreID { get; set; }
        public List<TestResultScoreArtifact> Artifacts { get; set; } = new List<TestResultScoreArtifact>();
        public int Status { get; set; }
        public string MetaData { get; set; }
        public PreferencesValueJson Preferences { get; set; }
        public DateTime? PublishDate { get; set; }
        public int? Order { get; set; }
    }
}
