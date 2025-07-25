using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class LockItemViewModel
    {
        #region Options
        [XmlElement(ElementName = "lockVerifyStudent")]
        public string LockVerifyStudent { get; set; }

        [XmlElement(ElementName = "lockRequireTestTakerAuthentication")]
        public string LockRequireTestTakerAuthentication { get; set; }

        [XmlElement(ElementName = "lockShuffleQuestions")]
        public string LockShuffleQuestions { get; set; }
        [XmlElement(ElementName = "lockShuffleAnswers")]
        public string LockShuffleAnswers { get; set; }

        [XmlElement(ElementName = "lockAutoAdvanceTest")]
        public string LockAutoAdvanceTest { get; set; }

        [XmlElement(ElementName = "lockRequireHonorCode")]
        public string LockRequireHonorCode { get; set; }
        [XmlElement(ElementName = "lockTestExtractExportRawScore")]
        public string LockTestExtractExportRawScore { get; set; }
        [XmlElement(ElementName = "LockMustAnswerAllQuestions")]
        public string LockMustAnswerAllQuestions { get; set; }

        [XmlElement(ElementName = "lockCanReviewTest")]
        public string LockCanReviewTest { get; set; }

        [XmlElement(ElementName = "lockcanPauseTest")]
        public string LockCanPauseTest { get; set; }

        [XmlElement(ElementName = "lockDisplayAnswerLabels")]
        public string LockDisplayAnswerLabels { get; set; }

        [XmlElement(ElementName = "lockAnswerLabelFormat")]
        public string LockAnswerLabelFormat { get; set; }

        [XmlElement(ElementName = "lockOverrideAutoGradedTextEntry")]
        public string LockOverrideAutoGradedTextEntry { get; set; }

        [XmlElement(ElementName = "lockPassagePositioninTestTaker")]
        public string LockPassagePositioninTestTaker { get; set; }

        [XmlElement(ElementName = "lockSupportHighlightText")]
        public string LockSupportHighlightText { get; set; }

        [XmlElement(ElementName = "lockEliminateChoiceTool")]
        public string LockEliminateChoiceTool { get; set; }

        [XmlElement(ElementName = "lockFlagItemTool")]
        public string LockFlagItemTool { get; set; }

        [XmlElement(ElementName = "lockTimeLimit")]
        public string LockTimeLimit { get; set; }

        //[XmlElement(ElementName = "lockDuration")]
        //public int LockDuration { get; set; }

        //[XmlElement(ElementName = "lockDeadline")]
        //public string LockDeadline { get; set; }

        [XmlElement(ElementName = "lockShowTimeLimitWarning")]
        public string LockShowTimeLimitWarning { get; set; }

        [XmlElement(ElementName = "lockMultipleChoiceClickMethod")]
        public string LockMultipleChoiceClickMethod { get; set; }

        [XmlElement(ElementName = "lockTestExtract")]
        public string LockTestExtract { get; set; }

        [XmlElement(ElementName = "lockEnableVideoControls")]
        public string LockEnableVideoControls { get; set; }

        [XmlElement(ElementName = "lockSectionBasedTesting")]
        public string LockSectionBasedTesting { get; set; }

        [XmlElement(ElementName = "lockLockedDownTestTaker")]
        public string LockLockedDownTestTaker { get; set; }

        [XmlElement(ElementName = "lockEnableAudio")]
        public string LockEnableAudio { get; set; }

        [XmlElement(ElementName = "lockAdaptiveTest")]
        public string LockAdaptiveTest { get; set; }

        #endregion

        #region Tools
        [XmlElement(ElementName = "lockSimplePalette")]
        public string LockSimplePalette { get; set; }

        [XmlElement(ElementName = "lockMathPalette")]
        public string LockMathPalette { get; set; }

        [XmlElement(ElementName = "lockSpanishPalette")]
        public string LockSpanishPalette { get; set; }

        [XmlElement(ElementName = "lockFrenchPalette")]
        public string LockFrenchPalette { get; set; }

        [XmlElement(ElementName = "loclProtractor")]
        public string LockProtractor { get; set; }

        [XmlElement(ElementName = "lockSupportCalculator")]
        public string LockSupportCalculator { get; set; }

        [XmlElement(ElementName = "lockScientificCalculator")]
        public string LockScientificCalculator { get; set; }
        #endregion
        public LockItemViewModel()
        {
            //Options
            LockVerifyStudent = "0";
            LockRequireTestTakerAuthentication = "0";
            LockShuffleQuestions = "0";
            LockShuffleAnswers = "0";
            LockAutoAdvanceTest = "0";
            LockRequireHonorCode = "0";
            LockTestExtractExportRawScore = "0";
            LockMustAnswerAllQuestions = "0";
            LockCanReviewTest = "0";
            LockCanPauseTest = "0";
            LockDisplayAnswerLabels = "0";
            LockAnswerLabelFormat = "0";
            LockOverrideAutoGradedTextEntry = "0";
            LockPassagePositioninTestTaker = "0";
            LockSupportHighlightText = "0";
            LockEliminateChoiceTool = "0";
            LockFlagItemTool = "0";
            LockTimeLimit = "0";
            //LockDuration = 0;
            //LockDeadline = "0";
            LockShowTimeLimitWarning = "0";
            LockMultipleChoiceClickMethod = "0";
            LockTestExtract = "0";
            LockEnableVideoControls = "0";
            LockLockedDownTestTaker = "0";
            LockEnableAudio = "0";
            LockSectionBasedTesting = "0";
            LockAdaptiveTest = "0";
            //Tools
            LockSimplePalette = "0";
            LockMathPalette = "0";
            LockSpanishPalette = "0";
            LockFrenchPalette = "0";
            LockProtractor = "0";
            LockSupportCalculator = "0";
            LockScientificCalculator = "0";
        }
    }
}
