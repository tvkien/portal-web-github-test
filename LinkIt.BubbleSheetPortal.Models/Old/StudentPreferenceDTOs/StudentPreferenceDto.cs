using System;
using System.Collections.Generic;
using System.Drawing;

namespace LinkIt.BubbleSheetPortal.Models.StudentPreferenceDTOs
{
    public class StudentPreferenceDto
    {
        public StudentPreferenceDto Clone()
        {
            return (StudentPreferenceDto)this.MemberwiseClone();
        }

        public int StudentPreferenceID { get; set; }
        public string Level { get; set; }
        public int LevelID { get; set; }
        public int? DataSetCategoryID { get; set; }
        public int? VirtualTestID { get; set; }
        public string VirtualTestIDs { get; set; }
        public int[] ClassIds { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedBy { get; set; }
        public virtual List<StudentPreferenceDetailDto> Details { get; set; }
        public virtual List<StudentPreferenceGroupDetailDto> GroupDetails { get; set; }
        public bool CanEditStudentPreference { get; set; }

        public static readonly string[] GroupNames = new string[]
            {
                "General Options",
                "Supplementary",
                "Test Review",
                "Percentile",
                "Tags",
                "Averages",
                "Item Data",
                "Item Analysis",
                "Time Spent",
                "Item Detail Chart",
            };

        public static readonly int[] TotalItemInGroups = new int[] { 2, 2, 1, 2, 6, 3, 1, 2, 4, 4 };

        // General Options [Top]
        public const string ShowTest = "showTest";
        public const string VisibilityInTestSpecific = "visibilityInTestSpecific";

        //// [Supplementary]
        public const string ShowNotes = "showNotes";
        public const string ShowAttachments = "showArtifacts";

        // [Test Review]
        public const string ReviewTest = "reviewTest";
        //public const string ShowTeacherComment = "showTeacherComment";

        // [Item Data]
        public const string ShowItemData = "showItemData";

        // [Tags]
        public const string ShowAssociatedQuestions = "showAssociatedQuestions";
        public const string ShowStandards = "showStandards";
        public const string ShowTopics = "showTopics";
        public const string ShowSkills = "showSkills";
        public const string ShowOtherTags = "showOtherTags";
        public const string ShowCustomTags = "showCustomTags";

        // [Averages]
        public const string ShowClassAverages = "showClassAverages";
        public const string ShowSchoolAverage = "showSchoolAverage";
        public const string ShowDistrictAverage = "showDistrictAverage";

        // [Show Summary Detail]
        public const string ShowSummaryDetail = "showSummaryDetail";

        // [Launch Item Analysis]
        public const string LaunchItemAnalysis = "launchItemAnalysis";

        // [Show Question Content]
        public const string ShowQuestions = "showQuestions";

        // [Time Spent]
        public const string ShowStudentTimeSpent = "showTimeSpent";
        public const string ShowClassTimeSpent = "showTimeSpentClass";
        public const string ShowSchoolTimeSpent = "showTimeSpentSchool";
        public const string ShowDistrictTimeSpent = "showTimeSpentDistrict";

        // [Item Detail Chart]
        public const string ShowItemDetailChart = "showItemDetailChart";
        public const string ShowCorrectAnswers = "showCorrectAnswers";
        public const string ShowStudentAnswers = "showStudentAnswers";
        public const string ShowPointPossible = "showPointsPossible";

        // [Percentile]
        public const string ShowSchoolPercentile = "showSchoolPercentile";
        public const string ShowDistrictPercentile = "showDistrictPercentile";

        public static List<string> ListName
        {
            get
            {
                return new List<string>
                {
                    ShowTest,
                    VisibilityInTestSpecific,
                    ShowNotes,
                    ShowAttachments,
                    ReviewTest,
                    ShowSchoolPercentile,
                    ShowDistrictPercentile,
                    //ShowTeacherComment,
                    //ShowItemData,
                    ShowAssociatedQuestions,
                    ShowStandards,
                    ShowTopics,
                    ShowSkills,
                    ShowOtherTags,
                    ShowCustomTags,
                    ShowClassAverages,
                    ShowSchoolAverage,
                    ShowDistrictAverage,
                    ShowSummaryDetail,
                    LaunchItemAnalysis,
                    ShowQuestions,
                    ShowStudentTimeSpent,
                    ShowClassTimeSpent,
                    ShowSchoolTimeSpent,
                    ShowDistrictTimeSpent,
                    ShowItemDetailChart,
                    ShowCorrectAnswers,
                    ShowStudentAnswers,
                    ShowPointPossible
                };
            }
        }

        public static List<StudentOption> StudentOptions
        {
            get
            {
                return new List<StudentOption>()
                {
                    new StudentOption{ Name = ShowTest, DisplayName = "Show Test", ClassStyle = "general-options", Order = 0 },
                    new StudentOption{ Name = VisibilityInTestSpecific, DisplayName = "Visibility In Test-Specific Options", ClassStyle = "general-options", Order = 1 },
                    new StudentOption{ Name = ShowNotes, DisplayName = "Show Notes", ClassStyle = "supplementary", Order = 2 },
                    new StudentOption{ Name = ShowAttachments, DisplayName = "Show Artifacts", ClassStyle = "supplementary", Order = 3 },
                    new StudentOption{ Name = ReviewTest, DisplayName = "Can Review Test", ClassStyle = "test-review", Order = 4 },
                    //new StudentOption{ Name = ShowTeacherComment, DisplayName = "Show Teacher Comment", ClassStyle = "test-review", Order = 5 },
                    new StudentOption{ Name = ShowSchoolPercentile, DisplayName = "Show School Percentile", ClassStyle = "percentile", Order = 5 },
                    new StudentOption{ Name = ShowDistrictPercentile, DisplayName = "Show District Percentile", ClassStyle = "percentile", Order = 6 },
                    new StudentOption{ Name = ShowItemData, DisplayName = "Item Data", ClassStyle = "test-review", Order = 7 },
                    new StudentOption{ Name = ShowAssociatedQuestions, DisplayName = "Show Associated Questions", ClassStyle = "tags", Order = 8 },
                    new StudentOption{ Name = ShowStandards, DisplayName = "Show Standards", ClassStyle = "tags", Order = 9 },
                    new StudentOption{ Name = ShowTopics, DisplayName = "Show Topics", ClassStyle = "tags", Order = 10 },
                    new StudentOption{ Name = ShowSkills, DisplayName = "Show Skills", ClassStyle = "tags", Order = 11 },
                    new StudentOption{ Name = ShowOtherTags, DisplayName = "Show Other Tags", ClassStyle = "tags", Order = 12 },
                    new StudentOption{ Name = ShowCustomTags, DisplayName = "Show Custom Tags", ClassStyle = "tags", Order = 13 },
                    new StudentOption{ Name = ShowClassAverages, DisplayName = "Show Class Averages", ClassStyle = "averages", Order = 14 },
                    new StudentOption{ Name = ShowSchoolAverage, DisplayName = "Show School Averages", ClassStyle = "averages", Order = 15 },
                    new StudentOption{ Name = ShowDistrictAverage, DisplayName = "Show District Averages", ClassStyle = "averages", Order = 16 },
                    new StudentOption{ Name = ShowSummaryDetail, DisplayName = "Show Summary Detail", ClassStyle = "launch-item-analysis", Order = 17 },
                    new StudentOption{ Name = LaunchItemAnalysis, DisplayName = "Launch Item Analysis", ClassStyle = "launch-item-analysis", Order = 18 },
                    new StudentOption{ Name = ShowQuestions, DisplayName = "Show Question Content", ClassStyle = "question-content", Order = 19 },
                    new StudentOption{ Name = ShowStudentTimeSpent, DisplayName = "Show Student Time Spent", ClassStyle = "time-spent", Order = 20 },
                    new StudentOption{ Name = ShowClassTimeSpent, DisplayName = "Show Class Time Spent (Avg.)", ClassStyle = "time-spent", Order = 21 },
                    new StudentOption{ Name = ShowSchoolTimeSpent, DisplayName = "Show School Time Spent (Avg.)", ClassStyle = "time-spent", Order = 22 },
                    new StudentOption{ Name = ShowDistrictTimeSpent, DisplayName = "Show District Time Spent (Avg.)", ClassStyle = "time-spent", Order = 23 },
                    new StudentOption{ Name = ShowItemDetailChart, DisplayName = "Show Item Detail Chart", ClassStyle = "answer-option", Order = 24 },
                    new StudentOption{ Name = ShowCorrectAnswers, DisplayName = "Show Correct Answers", ClassStyle = "answer-option", Order = 25 },
                    new StudentOption{ Name = ShowStudentAnswers, DisplayName = "Show Student Answers", ClassStyle = "answer-option", Order = 26 },
                    new StudentOption{ Name = ShowPointPossible, DisplayName = "Show Points Possible", ClassStyle = "answer-option", Order = 27 }
                };
            }
        }
    }

    public class StudentOption
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ClassStyle { get; set; }
        public int Order { get; set; }
    }
}
