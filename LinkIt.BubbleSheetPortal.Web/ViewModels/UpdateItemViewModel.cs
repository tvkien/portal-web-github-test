using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestMaker;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class UpdateItemViewModel
    {
        public int QtiItemId { get; set; }
        public string XMLContent { get; set; }
        public bool? NoDuplicateAnswers { get; set; }
        public int? VirtualquestionId { get; set; }
        public List<AlgorithmicExpression> ListExpression { get; set; }
        public List<MultiPartExpressionDto> ListMultiPartExpression { get; set; }

        public List<RubricQuestionCategoryDto> RubricQuestionCategories { get; set; }

        public int? PointsPossible { get; set; }

        public bool IsChangeAnswerChoice { get; set; } = false;
        public int? VirtualSectionId { get; set; }
        public int VirtualTestId { get; set; }
        public string Title {  get; set; }
        public string Description { get; set; }
    }
}
