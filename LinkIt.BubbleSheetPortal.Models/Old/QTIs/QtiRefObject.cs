using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QtiRefObject
    {
        public int QTIRefObjectID { get; set; }
        public string Name { get; set; }
        public string OldMasterCode { get; set; }
        public int TypeID { get; set; }
        public int UserID { get; set; }
        public int QTIRefObjectFileRef { get; set; }
        public string Subject { get; set; }
        public int? GradeID { get; set; }
        public int? TextTypeID { get; set; }
        public int? TextSubTypeID { get; set; }
        public int? FleschKincaidID { get; set; }
        public int DistrictId { get; set; }
        public int StateId { get; set; }
        public string TextType { get; set; }
        public string TextSubType { get; set; }
        public string GradeName { get; set; }
        public string FleschKinkaidName { get; set; }
        public int? GradeOrder { get; set; }
    }
}
