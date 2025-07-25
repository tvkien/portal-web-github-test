using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;

namespace LinkIt.BubbleSheetPortal.Web.Print
{
    public class VirtualQuestionModel
    {
        public string TestName { get; set; }

        public string TestInstruction { get; set; }

        public int VirtualSectionID { get; set; }

        public string SectionTitle { get; set; }

        public int SectionOrder { get; set; }

        public string SectionInstruction { get; set; }

        public int? SectionReferenceID { get; set; }

        public int VirtualQuestionID { get; set; }

        public int QuestionOrder { get; set; }

        public int PointsPossible { get; set; }

        public int QTIItemID { get; set; }
        public int QTISchemaID { get; set; }

        public string Title { get; set; }

        public int QTIGroupID { get; set; }

        public string XmlContent { get; set; }

        public string UrlPath { get; set; }
        public string CorrectAnswer { get; set; }
        public List<string> PassageTexts { get; set; }

        public AnswerKeyXml AnswerKeyXml { get; set; }

        public string Skills { get; set; }
        public string Topics { get; set; }
        public string Other { get; set; }
        public string Standards { get; set; }
        public string DistrictTag { get; set; }

        public int? QuestionGroupId { get; set; }

        public string GroupQuestionCommon { get; set; }
        public int ResponseProcessingTypeID { get; set; }
        public List<AlgorithmicCorrectAnswer> AlgorithmicCorrectAnswers { get; set; }

        public string GroupQuestionTitle { get; set; }
        public string QuestionLabel { get; set; }
    }
}
