using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestMaker;
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class QtiItemEditViewModel
    {
        public QtiItemEditViewModel()
        {
            QtiItemId = 0;
            XmlContent = string.Empty;
            ResponseIdentifier = string.Empty;
            QtiGroupName = string.Empty;
            NoDuplicateAnswer = false;
            FromVirtualQuestionProperty = false;
            VirtualTestId = 0;
            VirtualQuestionId = 0;
            ResponseProcessingTypeID = 0;
            ListMultiPartExpression = new List<MultiPartExpressionDto>();
            VirtualQuestionRubricCount = 0;
        }

        public int QtiItemId { get; set; }
        public string XmlContent { get; set; }
        public int QTISchemaId { get; set; }
        public int PointsPossible { get; set; }
        public string ResponseIdentifier { get; set; }
        public int QtiItemGroupId { get; set; }
        public string QtiGroupName { get; set; }
        public bool HasTest { get; set; }
        public int PreviousQtiItemId { get; set; }
        public int NextQtiItemId { get; set; }
        public bool NoDuplicateAnswer { get; set; }
        public bool FromVirtualQuestionProperty { get; set; }
        public int VirtualTestId { get; set; }
        public int VirtualQuestionId { get; set; }
        public int CountQtiItems { get; set; }
        public int QuestionOrder { get; set; }
        public int CountVirtualQuestions { get; set; }
        public int VirtualQuestionOrder { get; set; }
        public MediaModel MediaModel { get; set; }
        public int PreviousVirtualQuesionId { get; set; }
        public int NextVirtualQuesionId { get; set; }

        public int WarningTimeoutMinues { get; set; }
        public int DefaultCookieTimeOutMinutes { get; set; }
        public int KeepAliveDistanceSecond { get; set; }
        public string BasicSciencePaletteSymbol { get; set; }

        public int ResponseProcessingTypeID { get; set; }
        public List<AlgorithmicExpression> ListExpression { get; set; }
        public List<MultiPartExpressionDto> ListMultiPartExpression { get; set; }

        public List<RubricQuestionCategoryDto> RubricQuestionCategories { get; set; }

        public int IsAllowRubricGradingMode { get; set; } = 0;

        public int VirtualQuestionRubricCount { get; set; } = 0;
        public int VirtualSectionId { get; set; }

        public List<PassageViewModel> PassageList { get; set; } = new List<PassageViewModel>();

        public int IsSurvey { get; set; } = 0;
        public bool IsVirtualTestHasRetakeRequest { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool FromItemLibrary { get; set; }
    }
}
