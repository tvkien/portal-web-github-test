using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.Web.Models.TestAssignmentRegrader
{
    public class QuestionAssignment
    {
        public int QTIItemID { get; set; }
        public int VirtualQuestionID { get; set; }
        public int QTIItemSchemaID { get; set; }
        public int QuestionOrder { get; set; }
        public string XmlContent { get; set; }
        public bool Answered { get; set; }
        public int PointsPossible { get; set; }
        public string SectionInstruction { get; set; }

        public bool Unanswered
        {
            get { return !Answered; }
        }

        public bool Reviewable { get { return QTIItemSchemaID == 10 || QTIItemSchemaID == 9; } }

        public bool? IsRubricBasedQuestion { get; set; }

        public bool IsBaseVirtualQuestion { get; set; }
        public bool IsGhostVirtualQuestion { get; set; }
        public int? BaseVirtualQuestionID { get; set; }
        public string CorrectAnswer { get; set; }
        public string ResponseProcessing { get; set; }
        public int? ResponseProcessingTypeID { get; set; }
        public int? QuestionGroupID { get; set; }
        public bool IsRestrictedManualGrade { get; set; }

        public bool IsApplyAlgorithmicScoring
        {
            get
            {
                if (ResponseProcessingTypeID.HasValue)
                {
                    return ResponseProcessingTypeID.Value == (int)ResponseProcessingTypeEnum.AlgorithmicScoring;
                }
                else
                {
                    return false;
                }
            }
        }

        public List<AlgorithmicQuestion> AlgorithmicQuestions { get; set; }

        public List<AlgorithmicQuestionExpression> AlgorithmicQuestionExpressions { get; set; }

        public List<AlgorithmicCorrectAnswer> AlgorithmicCorrectAnswers { get; set; }

        public List<RubricQuestionCategoryDto> RubricQuestionCategories { get; set; }
    }
}
