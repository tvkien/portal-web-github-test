using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ACTUnansweredQuestion
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

        public IEnumerable<string> DisplayAnswerChoices
        {
            get
            {
                if (IsOpenEndedQuestion)
                {
                    for (int i = 0; i < PointsPossible + 1; i++)
                    {
                        yield return i.ToString(CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(AnswerIdentifiers))
                    {
                        AnswerIdentifiers = "A;B;C;D";
                    }
                    var answerChoices = AnswerIdentifiers.Split(';');

                    char character = (OrderSectionQuestionIndex - 1) % 2 == 0 ? 'A' : 'F';
                    for (int i = 0; i < answerChoices.Count(); i++)
                    {
                        int characterOffset = i;
                        if (character == 'F')
                        {
                            if (i >= 3)
                            {
                                characterOffset += 1;
                            }
                        }
                        yield return ((char)(character + characterOffset)).ToString(CultureInfo.InvariantCulture);
                    }
                }
            }
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
                    yield return ((char)('A' + i)).ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        public string XmlContent { get; set; }
        public int MaxChoice { get; set; }

        public List<string> ListAnswerLetter
        {
            get
            {
                return answerLetter.Split(',').ToList();
            }
        }

        //\
        private string _sectionTitle;
        public string SectionTitle
        {
            get { return _sectionTitle; }
            set { _sectionTitle = value.ConvertNullToEmptyString(); }
        }


        public int OrderSectionQuestion { get; set; }
        public int OrderSection { get; set; }

        public int OrderSectionQuestionIndex { get; set; }
        public int OrderSectionIndex { get; set; }

        public int VirtualSectionId { get; set; }

        public bool IsMultiMarkQuestion { get; set; }

        public string DomainTag { get; set; }
    }
}
