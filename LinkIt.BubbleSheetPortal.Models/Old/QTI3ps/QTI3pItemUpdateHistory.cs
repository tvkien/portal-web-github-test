using System;
using System.Collections.Generic;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTI3pItemUpdateHistory
    {
        public int QTI3pItemUpdateHistoryID { get; set; }
        public int QTI3pItemID { get; set; }
        public string ColumnName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}