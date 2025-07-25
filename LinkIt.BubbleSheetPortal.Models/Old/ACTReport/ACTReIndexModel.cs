using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.ACTReport
{
    public class ACTReIndexModel
    {
        public int AnswerID { get; set; }
        public int SectionID { get; set; }
        public int Order { get; set; }
        public int SubjectID { get; set; }
        public int VirtualQuestionID { get; set; }
    }

    public class ACTReIndexModelComparer : IEqualityComparer<ACTReIndexModel>
    {
        public bool Equals(ACTReIndexModel x, ACTReIndexModel y)
        {
            return x.AnswerID == y.AnswerID && x.Order == y.Order && x.SectionID == y.SectionID &&
                   x.VirtualQuestionID == y.VirtualQuestionID;
        }

        public int GetHashCode(ACTReIndexModel obj)
        {
            return obj.VirtualQuestionID ^ obj.AnswerID ^ obj.SectionID ^ obj.Order;
        }
    }
}