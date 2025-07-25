using System;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport
{
    public class TestAndScoreViewModel
    {
        public int TestResultID { get; set; }
        public DateTime TestDate { get; set; }
        public bool IsSelected { get; set; }
        public string TestName { get; set; }

        public decimal CompositeScore { get; set; }        
        public decimal EnglishScore { get; set; }
        public decimal MathScore { get; set; }
        public decimal ReadingScore { get; set; }
        public decimal ScienceScore { get; set; }
        public decimal WritingScore { get; set; }
        public decimal WritingScoreScaled { get; set; }
        public decimal EnglishWritingScore { get; set; }
        public int VirtualTestSubTypeID { get; set; }

        public string CompositeScoreText
        {
            get { return Math.Round(CompositeScore, MidpointRounding.AwayFromZero).ToString(); }
        }

        public string EnglishScoreText
        {
            get { return Math.Round(EnglishScore, MidpointRounding.AwayFromZero).ToString(); }
        }

        public string MathScoreText
        {
            get { return Math.Round(MathScore, MidpointRounding.AwayFromZero).ToString(); }
        }

        public string ReadingScoreText
        {
            get { return Math.Round(ReadingScore, MidpointRounding.AwayFromZero).ToString(); }
        }

        public string ScienceScoreText
        {
            get { return Math.Round(ScienceScore, MidpointRounding.AwayFromZero).ToString(); }
        }

        public string WritingScoreText
        {
            get { return Math.Round(WritingScore, MidpointRounding.AwayFromZero).ToString(); }
        }

        public string EnglishWritingScoreText
        {
            get { return Math.Round(EnglishWritingScore, MidpointRounding.AwayFromZero).ToString(); }
        }

        public string TestDateText
        {
            get { return TestDate.DisplayDateWithFormat(); }
        }

        public string StemScoreText
        {
            get { return Math.Round((ScienceScore + MathScore)/2, MidpointRounding.AwayFromZero).ToString(); }
        }

        public string ElaScoreText
        {
            get
            {
                return Math.Round((EnglishScore + ReadingScore + WritingScoreScaled) /3, MidpointRounding.AwayFromZero).ToString();
            }
        }
    }
}