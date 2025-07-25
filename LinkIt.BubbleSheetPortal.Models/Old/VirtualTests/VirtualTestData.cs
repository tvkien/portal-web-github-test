using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    [Serializable()]
    public class VirtualTestData
    {
        public int VirtualTestID { get; set; }
        public string Name { get; set; }
        public int StateID { get; set; }
        public int BankID { get; set; }
        public int VirtualTestSourceID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? AuthorUserID { get; set; }
        public bool? Archived { get; set; }
        public int? EditedByUserID { get; set; }
        public int? AchievementLevelSettingID { get; set; }
        public int? VirtualTestType { get; set; }
        public string Instruction { get; set; }
        public int? PreQTIVirtualTestID { get; set; }
        public int? PreProdVTID { get; set; }
        public int? TestScoreMethodID { get; set; }
        public int? VirtualTestSubTypeID { get; set; }

        public bool? IsTeacherLed { get; set; }

        public bool? HasUseRationale { get; set; }
    }
}
