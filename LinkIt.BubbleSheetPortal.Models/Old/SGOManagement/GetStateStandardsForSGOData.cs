using System.Collections.Generic;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models.SGOManagement
{
    public class GetStateStandardsForSGOData
    {
        public int MasterStandardId { get; set; }
        public int StateId { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }
        public string SubjectName { get; set; }
        public int GradeId { get; set; }
        public string GradeName { get; set; }
        public int? LowGradeId { get; set; }
        public int? HighGradeId { get; set; }
        public int Level { get; set; }
        public int? Children { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string GUID { get; set; }
        public string ParentGUID { get; set; }
        public int DataSetOriginID { get; set; }
    }
}
