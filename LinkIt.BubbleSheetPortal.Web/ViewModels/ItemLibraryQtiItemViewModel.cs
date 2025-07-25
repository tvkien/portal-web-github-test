using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ItemLibraryQtiItemViewModel
    {
        public int QtiItemId { get; set; }
        public string Name { get; set; }
        public string ToolTip { get; set; }

        public int MaxItemTooltipLength
        {
            get
            {
                if (ToolTip == null)
                {
                    ToolTip = string.Empty;
                }
                string[] lines = ToolTip.Split(new string[] { "<br>" }, StringSplitOptions.None);
                int maxLength = 0;
                foreach (var line in lines)
                {
                    if (line.Length > maxLength)
                    {
                        maxLength = line.Length;
                    }
                }
                return maxLength;
            }
        }

        public int? VirtualQuestionCount { get; set; }

        public int? VirtualQuestionRubricCount { get; set; }
    }

    public class ItemLibraryQtiItemModel
    {
        public int QtiItemId { get; set; }
        public string Content { get; set; }
        public string GroupName { get; set; }
        public string BankName { get; set; }
        public string Standard { get; set; }
        public string DistrictTag { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public int? QTIBankID { get; set; }
        public string ToolTip { get; set; }
        public int? QTIGroupID { get; set; }

        public int MaxItemTooltipLength
        {
            get
            {
                if (ToolTip == null)
                {
                    ToolTip = string.Empty;
                }
                string[] lines = ToolTip.Split(new string[] { "<br>" }, StringSplitOptions.None);
                int maxLength = 0;
                foreach (var line in lines)
                {
                    if (line.Length > maxLength)
                    {
                        maxLength = line.Length;
                    }
                }
                return maxLength;
            }
        }

        public bool HasPermissionEditQTItem { get; set; }
        public string ItemSetUrl { get; set; }
    }
}
