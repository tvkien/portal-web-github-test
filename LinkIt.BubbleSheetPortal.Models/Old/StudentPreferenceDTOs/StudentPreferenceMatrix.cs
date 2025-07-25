using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.Old.StudentPreferenceDTOs
{
    public class StudentPreferenceMatrix
    {
        public string LevelName { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ClassStyle { get; set; }
        public int StudentPreferenceID { get; set; }
        public bool Value { get; set; }
        public bool Locked { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDisabledByType { get; set; }
        public int Priority { get; set; }
        public bool IsMissing { get; set; }
        public bool IsShowItemData { get; set; }
        public bool IsNotShow { get; set; }
        public bool IsConflict { get; set; }
        public int Order { get; set; }
    }
}
