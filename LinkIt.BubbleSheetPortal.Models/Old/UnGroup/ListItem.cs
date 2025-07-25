using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool Selected { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as ListItem;
            return t != null && Id.Equals(t.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class ListItemStr
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class ItemValue
    {
        public int Id1 { get; set; }
        public int Id2 { get; set; }
    }
}
