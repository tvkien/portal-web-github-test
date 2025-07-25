using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.StudentPreferenceDTOs
{
    public class Setting
    {
        public int showTest { get; set; }
        public int showNotes { get; set; }
        public int showArtifacts { get; set; }
        public int showStandards { get; set; }
        public int showTopics { get; set; }
        public int showSkills { get; set; }
        public int showItemTags { get; set; }
        public int showOtherTags { get; set; }
        public int showItemData { get; set; }
        public int showQuestions { get; set; }
        public int showCorrectAnswers { get; set; }
        public int showStudentAnswers { get; set; }
        public int showPointsPossible { get; set; }
        public int showTimeSpent { get; set; }
        public int showTimeSpentClass { get; set; }
        public int showTimeSpentSchool { get; set; }
        public int showTimeSpentDistrict { get; set; }
        public int showClassAverages { get; set; }
        public int showSchoolAverage { get; set; }
        public int showDistrictAverage { get; set; }
        public int reviewTest { get; set; }

        public Setting(List<StudentPreferenceDetailDto> items)
        {
            showTest = items.Any(m => m.Name == "showTest" && m.Value == true) ? 1 : 0;
            showNotes = items.Any(m => m.Name == "showNotes" && m.Value == true) ? 1 : 0;
            showArtifacts = items.Any(m => m.Name == "showArtifacts" && m.Value == true) ? 1 : 0;
            showStandards = items.Any(m => m.Name == "showStandards" && m.Value == true) ? 1 : 0;
            showTopics = items.Any(m => m.Name == "showTopics" && m.Value == true) ? 1 : 0;
            showSkills = items.Any(m => m.Name == "showSkills" && m.Value == true) ? 1 : 0;
            showItemTags = items.Any(m => m.Name == "showItemTags" && m.Value == true) ? 1 : 0;
            showOtherTags = items.Any(m => m.Name == "showOtherTags" && m.Value == true) ? 1 : 0;
            showItemData = items.Any(m => m.Name == "showItemData" && m.Value == true) ? 1 : 0;
            showQuestions = items.Any(m => m.Name == "showQuestions" && m.Value == true) ? 1 : 0;
            showCorrectAnswers = items.Any(m => m.Name == "showCorrectAnswers" && m.Value == true) ? 1 : 0;
            showStudentAnswers = items.Any(m => m.Name == "showStudentAnswers" && m.Value == true) ? 1 : 0;
            showPointsPossible = items.Any(m => m.Name == "showPointsPossible" && m.Value == true) ? 1 : 0;
            showTimeSpent = items.Any(m => m.Name == "showTimeSpent" && m.Value == true) ? 1 : 0;
            showTimeSpentClass = items.Any(m => m.Name == "showTimeSpentClass" && m.Value == true) ? 1 : 0;
            showTimeSpentSchool = items.Any(m => m.Name == "showTimeSpentSchool" && m.Value == true) ? 1 : 0;
            showTimeSpentDistrict = items.Any(m => m.Name == "showTimeSpentDistrict" && m.Value == true) ? 1 : 0;
            showClassAverages = items.Any(m => m.Name == "showClassAverages" && m.Value == true) ? 1 : 0;
            showSchoolAverage = items.Any(m => m.Name == "showSchoolAverage" && m.Value == true) ? 1 : 0;
            showDistrictAverage = items.Any(m => m.Name == "showDistrictAverage" && m.Value == true) ? 1 : 0;
            reviewTest = items.Any(m => m.Name == "reviewTest" && m.Value == true) ? 1 : 0;
        }
    }
}
