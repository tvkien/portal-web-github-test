using System.Collections.Generic;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QuestionOptions : ValidatableEntity<QuestionOptions>
    {
        private string answerIdentifiers = string.Empty;

        public int TestId { get; set; }
        public int VirtualQuestionId { get; set; }
        public int ProblemNumber { get; set; }
        public bool IsOpenEndedQuestion { get; set; }
        public bool IsGhostQuestion { get; set; }
        public int? PointsPossible { get; set; }

        public string AnswerIdentifiers
        {
            get { return answerIdentifiers; }
            set { answerIdentifiers = value.ConvertNullToEmptyString(); }
        }

        public IEnumerable<string> Options
        {
            get
            {
                if (string.IsNullOrEmpty(answerIdentifiers))
                {
                    return new List<string> {"A", "B", "C", "D"};
                }
                return answerIdentifiers.Split(';');
            }
        }

        public int QtiSchemaId { get; set; }

        public int? QuestionGroupID { get; set; }

        public int? VirtualSectionID { get; set; }

    }
}
