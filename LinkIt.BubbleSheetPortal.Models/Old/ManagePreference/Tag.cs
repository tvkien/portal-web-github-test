using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Models.ManagePreference
{
    public class Tag
    {
        public int LevelId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Tooltips { get; set; }
        public List<TagAttr> Attributes { get; set; }
        public List<Tag> SectionItems { get; set; }
        public bool Locked
        {
            get
            {
                if (Attributes != null && Attributes.Any())
                {
                    return Attributes.Any(x => x?.Key.ToLower() == "lock" && x.Value.ToLower() == "true");
                }

                return false;
            }
        }

        public Tag()
        {
            Attributes = new List<TagAttr>();
            SectionItems = new List<Tag>();
        }
    }
}
