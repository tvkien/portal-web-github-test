using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.Algorithmic
{
    public class AlgorithmicQuestionExpression
    {
        public int VirtualQuestionID { get; set; }

        public int VirtualTestID { get; set; }

        public int QTIItemID { get; set; }

        public string AlgorithmicExpression { get; set; }

        public int PointsEarned { get; set; }

        public int? AlgorithmicOrder { get; set; }
    }

    public class AlgorithmicQuestion
    {
        public int VirtualQuestionID { get; set; }
        public int VirtualTestID { get; set; }
        public int QTIItemID { get; set; }
        public int? AlgorithmQuestionID { get; set; }
        public string AlgorithmQuestionExpression { get; set; }
        public int? AlgorithmQTIItemID { get; set; }
        public string AlgorithmQTIItemExpression { get; set; }
    }

    public class BaseAlgorithmicQuestion
    {
        public int VirtualQuestionID { get; set; }
        public int QTIItemID { get; set; }
        public string Expression { get; set; }
    }
    public class AlgorithmicCorrectAnswer
    {
        public AlgorithmicCorrectAnswer()
        {
            ConditionValue = new List<string>();
        }

        public string ResponseIdentifier { get; set; }

        public int ConditionType { get; set; }

        public List<string> ConditionValue { get; set; }

        public string OriginalExpression { get; set; }

        public int PointsEarned { get; set; }
        public int? Amount { get; set; }
    }
}
