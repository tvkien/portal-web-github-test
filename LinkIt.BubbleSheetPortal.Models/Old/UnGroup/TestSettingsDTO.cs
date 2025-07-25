using System;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestSettingsDTO
    {
        [XmlIgnore]
        public string SettingLevel { get; set; }
        [XmlIgnore]
        public bool IsAssignGroup { get; set; }
        [XmlIgnore]
        public bool IslockedBank { get; set; }
        [XmlIgnore]
        public int DistrictId { get; set; }
        [XmlIgnore]
        public int SettingType { get; set; }

        [XmlElement(ElementName = "verifyStudent")]
        public string VerifyStudent { get; set; }

        [XmlElement(ElementName = "requireTestTakerAuthentication")]
        public string RequireTestTakerAuthentication { get; set; }

        [XmlElement(ElementName = "shuffleQuestions")]
        public string ShuffleQuestions { get; set; }

        [XmlElement(ElementName = "autoAdvanceTest")]
        public string AutoAdvanceTest { get; set; }

        [XmlElement(ElementName = "mustAnswerAllQuestions")]
        public string MustAnswerAllQuestions { get; set; }

        [XmlElement(ElementName = "canReviewTest")]
        public string CanReviewTest { get; set; }

        [XmlElement(ElementName = "canPauseTest")]
        public string CanPauseTest { get; set; }

        [XmlElement(ElementName = "displayAnswerLabels")]
        public string DisplayAnswerLabels { get; set; }

        [XmlElement(ElementName = "answerLabelFormat")]
        public string AnswerLabelFormat { get; set; }

        [XmlElement(ElementName = "overrideAutoGradedTextEntry")]
        public string OverrideAutoGradedTextEntry { get; set; }

        [XmlElement(ElementName = "passagePositioninTestTaker")]
        public string PassagePositioninTestTaker { get; set; }

        [XmlElement(ElementName = "supportHighlightText")]
        public string SupportHighlightText { get; set; }

        [XmlElement(ElementName = "eliminateChoiceTool")]
        public string EliminateChoiceTool { get; set; }

        [XmlElement(ElementName = "flagItemTool")]
        public string FlagItemTool { get; set; }

        [XmlElement(ElementName = "timeLimit")]
        public string TimeLimit { get; set; }

        [XmlElement(ElementName = "duration")]
        public int Duration { get; set; }

        [XmlElement(ElementName = "deadline")]
        public string Deadline { get; set; }

        [XmlElement(ElementName = "showTimeLimitWarning")]
        public string ShowTimeLimitWarning { get; set; }

        [XmlElement(ElementName = "multipleChoiceClickMethod")]
        public string MultipleChoiceClickMethod { get; set; }

        [XmlElement(ElementName = "testExtract")]
        public string TestExtract { get; set; }

        [XmlElement(ElementName = "enableVideoControls")]
        public string EnableVideoControls { get; set; }

        [XmlElement(ElementName = "sectionBasedTesting")]
        public string SectionBasedTesting { get; set; }

        [XmlElement(ElementName = "lockedDownTestTaker")]
        public string LockedDownTestTaker { get; set; }

        [XmlElement(ElementName = "adaptiveTest")]
        public string AdaptiveTest { get; set; }
        [XmlElement(ElementName = "enableAudio")]
        public string EnableAudio { get; set; }


        [XmlIgnore]
        public bool IsUseTestExtract { get; set; }

        [XmlIgnore]
        public DateTime DeadlineDisplay { get; set; }


        public TestSettingsDTO()
        {
            ShowTimeLimitWarning = string.Empty;
            DeadlineDisplay = DateTime.Now;
            Deadline = string.Empty;
            Duration = 0;
            TimeLimit = string.Empty;
            SettingLevel = string.Empty;
            IsAssignGroup = false;
            IslockedBank = false;
            DistrictId = 0;
            SettingType = 0;
            VerifyStudent = string.Empty;
            RequireTestTakerAuthentication = string.Empty;
            ShuffleQuestions = string.Empty;
            AutoAdvanceTest = string.Empty;
            //AutoEndTest = string.Empty;
            MustAnswerAllQuestions = string.Empty;
            CanReviewTest = string.Empty;
            CanPauseTest = string.Empty;
            DisplayAnswerLabels = string.Empty;
            AnswerLabelFormat = string.Empty;
            OverrideAutoGradedTextEntry = string.Empty;
            PassagePositioninTestTaker = string.Empty;
            SupportHighlightText = string.Empty;
            EliminateChoiceTool = string.Empty;
            FlagItemTool = string.Empty;
            TestExtract = string.Empty;
            EnableVideoControls = string.Empty;
            SectionBasedTesting = string.Empty;
            LockedDownTestTaker = string.Empty;
            AdaptiveTest = string.Empty;
            EnableAudio = string.Empty;
        }
    }
}
