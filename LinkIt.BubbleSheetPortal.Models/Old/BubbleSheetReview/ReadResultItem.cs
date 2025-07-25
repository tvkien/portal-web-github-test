using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.BubbleSheetReview
{
    public class ReadResultItem
    {
        public int OptionCount { get; set; }
        public List<FilledBubble> Answers = new List<FilledBubble>();
        public int ProblemNumber { get; set; }
        public string Disposition { get { return GetDisposition(); } }
        public String FilledBubbles { get { return GetAnswer(); } }
        public bool Unreadable { get; set; }
        public bool IsACTSheet { get; set; }
        public bool IsSATSheet { get; set; }
        public int SectionIndex { get; set; }
        private const string TextEntryDisposition = "T";

        public Dictionary<int, List<FilledBubble>> TextEntryAnswers =
            new Dictionary<int, List<FilledBubble>>();

        public string TextEntryAnswerInText = string.Empty;

        private string GetDisposition()
        {
            if (!IsSATSheet)
            {
                return DetectDisposition();
            }

            if (TextEntryAnswers.Count == 0
                && string.IsNullOrEmpty(TextEntryAnswerInText.Trim()))
            {
                return DetectDisposition();
            }

            return DetectDispositionForTextEntrySAT();
        }

        private string DetectDispositionForTextEntrySAT()
        {
            if (Unreadable)
            {
                return "M";
            }
            var isUnanswer = TextEntryAnswers.Count(x => x.Value.Count > 0) == 0 && string.IsNullOrEmpty(TextEntryAnswerInText.Trim());
            var isMultiMark = TextEntryAnswers.Count(x => x.Value.Count > 1) > 0;
            if (isMultiMark) return "M";
            if (isUnanswer) return "U";
            return "A";
        }

        private string DetectDisposition()
        {
            if (Unreadable)
            {
                return "M";
            }
            var fillCount = Answers.Count();
            if (fillCount > 1)
            {
                return "M";
            }

            return fillCount == 1 ? "A" : "U";
        }

        public override string ToString()
        {
            if (Unreadable)
            {
                return string.Format("{0}|M||", ProblemNumber);
            }
            var inOrder = Answers.OrderBy(x => x.BubbleIndex).ToList();
            var letters = string.Join(",", inOrder.Select(x => x.BubbleIndex + 1));
            var confidences = string.Join(",", inOrder.Select(x => x.Confidence));
            if (IsACTSheet)
            {
                return string.Format("{0}|{1}|{2}|{3}|{4}", ProblemNumber, GetDisposition(), letters, confidences,
                    SectionIndex);
            }

            if (IsSATSheet && TextEntryAnswers.Count > 0)
            {
                StringBuilder markedBuilder = new StringBuilder();
                var listAnswers = TextEntryAnswers.OrderBy(x => x.Key).ToList();
                for (int index = 0; index < listAnswers.Count; index++)
                {
                    var textEntryAnswer = listAnswers[index];
                    markedBuilder.Append(string.Join(",", textEntryAnswer.Value.Select(x => x.BubbleIndex)));
                    if (index < listAnswers.Count - 1) markedBuilder.Append("-");
                }
                confidences = string.Join(",", TextEntryAnswers.Values.Select(x => string.Join(",", x.Select(y => y.Confidence))).ToList());
                return string.Format("{0}|{1}|{2}|{3}|{4}", ProblemNumber, GetDisposition(), markedBuilder, confidences,
                    SectionIndex);
            }

            if (IsSATSheet && !string.IsNullOrEmpty(TextEntryAnswerInText.Trim()))
            {
                return string.Format("{0}|{1}|{2}|{3}|{4}",
                    ProblemNumber,
                    //GetDisposition(),
                    TextEntryDisposition,
                    TextEntryAnswerInText,
                    1000,
                    SectionIndex);
            }

            if (IsSATSheet)
            {
                return string.Format("{0}|{1}|{2}|{3}|{4}", ProblemNumber, GetDisposition(), letters, confidences,
                    SectionIndex);
            }

            return string.Format("{0}|{1}|{2}|{3}", ProblemNumber, GetDisposition(), letters, confidences);
        }

        private String GetAnswer()
        {
            if (Unreadable) return "M";
            var inOrder = Answers.OrderBy(x => x.BubbleIndex).ToList();
            if (!inOrder.Any()) return "U";
            return string.Join(",", inOrder.Select(x => x.BubbleIndex));
        }

        public void AddAnswer(int bubble, int confidence)
        {
            Answers.Add(new FilledBubble { BubbleIndex = bubble, Confidence = confidence });
        }

        public void AddTextEntryAnswer(List<int> bubbles, List<int> confidence, int bubbleIndex)
        {
            var listFilledBubble = new List<FilledBubble>();
            for (int i = 0; i < bubbles.Count; i++)
            {
                listFilledBubble.Add(new FilledBubble { BubbleIndex = bubbles[i], Confidence = confidence[i] });
            }
            if (TextEntryAnswers.ContainsKey(bubbleIndex))
            {
                TextEntryAnswers[bubbleIndex].AddRange(listFilledBubble);
            }
            else
            {
                TextEntryAnswers.Add(bubbleIndex, listFilledBubble);
            }
        }
    }
}
