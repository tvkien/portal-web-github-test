using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AlreadyAnsweredQuestion
    {
        private string answerLetter = string.Empty;

        public int QuestionId { get; set; }
        public int QuestionOrder { get; set; }
        public string AnswerIdentifiers { get; set; }
        public int QTISchemaId { get; set; }
        public int PointsPossible { get; set; }
        public int? StudentId { get; set; }
        public int? BubbleSheetId { get; set; }
        public string Ticket { get; set; }

        public string AnswerLetter
        {
            get { return answerLetter; }
            set { answerLetter = value.ConvertNullToEmptyString(); }
        }

        public bool IsOpenEndedQuestion
        {
            get { return QTISchemaId.Equals(10); }
        }

        public IEnumerable<string> AnswerChoices
        {
            get
            {
                if (IsOpenEndedQuestion)
                {
                    for (int i = 0; i < PointsPossible + 1; i++)
                    {
                        yield return i.ToString(CultureInfo.InvariantCulture);
                    }
                    yield break;
                }
                if (string.IsNullOrEmpty(AnswerIdentifiers))
                {
                    AnswerIdentifiers = "A;B;C;D";
                }
                var answerChoices = AnswerIdentifiers.Split(';');
                for (int i = 0; i < answerChoices.Length; i++)
                {
                    var index = i >= 20 ? i + 1 : i;
                    int letterIndex = index % 26;
                    int prefixIndex = index / 26;
                    char? prefixChar = null;

                    if (prefixIndex > 0)
                        prefixChar = (char)('A' + prefixIndex - 1);

                    yield return prefixChar + ((char)('A' + letterIndex)).ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        public string XmlContent { get; set; }
        public int MaxChoice { get; set; }
        public string Cardinality { get; set; }

        public List<string> ListAnswerLetter
        {
            get
            {
                return answerLetter.Split(',').ToList();
            }
        }
    }
}
