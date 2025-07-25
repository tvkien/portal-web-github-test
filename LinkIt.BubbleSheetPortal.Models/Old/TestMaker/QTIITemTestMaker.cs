using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.TestMaker
{
    public class QTIItemTestMaker
    {
        public AssessmentItem AssessmentItem { get; set; }

        public int QTIGroupID { get; set; }
        public int QuestionOrder { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }

        public string AnswerIdentifiers { get; set; }

        public int QtiItemID { get; set; }
        public int QTISchemaID { get; set; }
        public string CorrectAnswer { get; set; }
        public int PointsPossible { get; set; }
        public string ResponseIdentifier { get; set; }
        public string ResponseProcessing { get; set; }
        public int? ResponseProcessingTypeID { get; set; }
        public string XmlContent { get; set; }

        private List<QTIItemAnswerScoreTestMaker> _qtiITemAnswerScoreTestMakers;
        public List<QTIItemAnswerScoreTestMaker> QTIITemAnswerScoreTestMakers
        {
            get
            {
                if (_qtiITemAnswerScoreTestMakers == null) _qtiITemAnswerScoreTestMakers = new List<QTIItemAnswerScoreTestMaker>();
                return _qtiITemAnswerScoreTestMakers;
            }
            set
            {
                _qtiITemAnswerScoreTestMakers = value;
            }
        }

        private List<QTIItemSubTestMaker> _qtiItemSubTestMaker;
        public List<QTIItemSubTestMaker> QTIItemSubTestMakers
        {
            get
            {
                if (_qtiItemSubTestMaker == null) _qtiItemSubTestMaker = new List<QTIItemSubTestMaker>();
                return _qtiItemSubTestMaker;
            }
            set
            {
                _qtiItemSubTestMaker = value;
            }
        }
    }
}
