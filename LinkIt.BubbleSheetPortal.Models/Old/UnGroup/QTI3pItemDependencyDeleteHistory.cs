using System;
using System.Collections.Generic;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTI3pItemDependencyDeleteHistory
    {
        public int QTI3pItemDependencyDeleteHistoryID { get; set; }
        public string TableName { get; set; }
        public int QTI3pItemID { get; set; }
        public string DenpendencyEntityValue { get; set; }
    }
}