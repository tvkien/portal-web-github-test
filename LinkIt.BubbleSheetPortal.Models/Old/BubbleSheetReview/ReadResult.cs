using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.BubbleSheetReview
{
    public class ReadResult
    {
        private string outputFile = string.Empty;
        private IList<ReadResultItem> questions = new List<ReadResultItem>();

        public ReadResult()
        {
            RosterPosition = "0";
        }

        public int UserId { get; set; }
        public int DistrictId { get; set; }
        public string Barcode1 { get; set; }
        public string Barcode2 { get; set; }
        public string InputPath { get; set; }
        public string InputFileName { get; set; }
        public double Dpi { get; set; }
        public int PageNumber { get; set; }
        public int? PageType { get; set; }
        public string FileDisposition { get { return "Readable"; } }
        public string OutputFile
        {
            get { return outputFile; }
            set { outputFile = value ?? string.Empty; }
        }
        public string UrlSafeOutputFile { get { return OutputFile.Replace('/', '-'); } }
        public TimeSpan ProcessingTime { get; set; }
        public DateTime CompletedTime { get; set; }
        public string RosterPosition { get; set; }
        public int QuestionCount { get { return Questions == null ? 0 : Questions.Count(); } }
        public IEnumerable<double[]> RawResult { get; set; }

        public int TestType { get; set; }
        public int ACTPageIndex { get; set; }

        public IEnumerable<string> Errors { get; set; }
        public bool IsRoster { get; set; }
        public string ResultDescription { get { return GetResultDescription(); } }

        public ICollection<ReadResultItem> Questions
        {
            get { return questions; }
            set { questions = new List<ReadResultItem>(value ?? new ReadResultItem[0]); }
        }

        private string GetResultDescription()
        {
            if (!questions.Any()) return "Unreadable-Total";
            var multiMarks = questions.Count(x => x.Answers.Count() > 1);
            var noMarks = questions.Count(x => !x.Answers.Any());

            string problems = string.Empty;
            if (multiMarks > 0) problems += string.Format("{0} {1} multiple answers. ", multiMarks, multiMarks > 1 ? "problems have" : "problem has");
            if (noMarks > 0) problems += string.Format("{0} {1} no answers. ", noMarks, noMarks > 1 ? "problems have" : "problem has");

            return problems;
        }

        public void SaveRawOutput(string file)
        {
            if (RawResult == null) return;
            var outputFile = File.CreateText(file);
            int bubbleNumber = 0;
            int questionNumber = 0;
            foreach (var rawOutputLine in RawResult)
            {
                if (bubbleNumber % 4 == 0)
                {
                    bubbleNumber = 0;
                    questionNumber++;
                    outputFile.WriteLine(@"\\Question {0}", questionNumber);
                }
                outputFile.WriteLine(@"\\Bubble {0}", (bubbleNumber++) + 1);
                outputFile.WriteLine(string.Join(",", rawOutputLine));
            }
            outputFile.Flush();
            outputFile.Close();
            outputFile.Dispose();
        }
    }
}
