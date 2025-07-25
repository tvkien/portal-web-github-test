using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.Old.ManagePreference
{
    public class TagHasDependent
    {
        public string Key { get; set; }
        public int CurrentLevelId { get; set; }
        public string ChildTag { get; set; }
    }
}
