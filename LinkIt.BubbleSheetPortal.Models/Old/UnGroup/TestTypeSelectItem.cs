
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.Old.UnGroup
{
    public class TestTypeSelectItem
    {
        public bool Selected { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public bool IsShow { get; set; }
        public string Tooltip { get; set; }
    }

    public class TestTypeFilter
    {
        public int? Id { get; set; }
        public string Text { get; set; }
        public List<TestTypeSelectItem> TestTypeSelectItems { get; set; }
    }
}
