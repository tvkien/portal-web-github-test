using LinkIt.BubbleSheetPortal.Common.Enum;
using System.Diagnostics;

namespace LinkIt.BubbleSheetPortal.Models.StudentPreferenceDTOs
{
    [DebuggerDisplay("Name: {Name} ~ Value: {Value} ~ Locked: {Locked}")]
    public class StudentPreferenceDetailDto
    {
        public int StudentPreferenceID { get; set; }
        public string Name { get; set; }
        public bool Value { get; set; }
        public bool Locked { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDisabledByType { get; set; }
        public int Priority { get; set; }
        public bool IsMissing { get; set; }

        public bool IsNotShow { get; set; }
        public bool IsConflict { get; set; } = false;

    }
}
