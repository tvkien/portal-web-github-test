using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AnswerKeyData
    {
        public string XmlContent { get; set; }
        public string ResponseProcessing { get; set; }
        public int QTISchemaID { get; set; }
        public bool? IsRubricBasedQuestion { get; set; }
        public string CorrectAnswer { get; set; }
        public int PointsPossible { get; set; }
        public string AlgorithmicExpression { get; set; }
        public int? VirtualQuestionID { get; set; }
        public int? QTIItemID { get; set; }
        public List<ChoiceVariableVirtualQuestionAnswerScore> ChoiceVariableAnswerScores { get; set; }
        public List<ComplexVirtualQuestionAnswerScore> ComplexAnswerScores { get; set; }
        public List<RubricQuestionCategoryDto> RubricAnswerScores { get; set; }
        public IList<QTIItemAnswerScoreInfoDTO> QTIItemAnswerScores { get; set; }
        public IList<QTIItemSubInfoDto> QTIItemSubs { get; set; }
    }
}
