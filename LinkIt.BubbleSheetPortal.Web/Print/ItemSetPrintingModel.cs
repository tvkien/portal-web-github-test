using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.Print
{
    public class ItemSetPrintingModel
    {
        public List<ItemModel> Items { get; set; }

        public string TestTitle { get; set; }
        public string TeacherName { get; set; }
        public string ClassName { get; set; }
        public string Css { get; set; }
        public string JS { get; set; }

        private List<KeyValuePair<string, string>> preferenceObjects;

        public List<KeyValuePair<string, string>> PreferenceObjects
        {
            get
            {
                if (preferenceObjects == null) preferenceObjects = new List<KeyValuePair<string, string>>();
                return preferenceObjects;
            }
            set
            {
                preferenceObjects = value;
            }
        }
    }
}
