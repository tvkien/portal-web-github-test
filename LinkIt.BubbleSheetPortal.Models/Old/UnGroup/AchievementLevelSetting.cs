using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AchievementLevelSetting : ValidatableEntity<AchievementLevelSetting>
    {
        private string valueString = string.Empty;
        private string labelString = string.Empty;
        private string name = string.Empty;
        private string gradeValueString = string.Empty;

        public int AchievementLevelSettingID { get; set; }
        
        public string ValueString
        {
            get { return valueString; }
            set { valueString = value.ConvertNullToEmptyString(); }
        }
        
        public string LabelString
        {
            get { return labelString; }
            set { labelString = value.ConvertNullToEmptyString(); }
        }
        
        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
        
        public string GradeValueString
        {
            get { return gradeValueString; }
            set { gradeValueString = value.ConvertNullToEmptyString(); }
        }
    }
}