using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Test : ValidatableEntity<Test>, IIdentifiable
    {
        private string name = string.Empty;
        private string status = string.Empty;

        public int Id { get; set; }
        public int? Type { get; set; }
        public int StateId { get; set; }
        public int BankId { get; set; }
        public int QuestionCount { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public string Status
        {
            get { return status; }
            set { status = value.ConvertNullToEmptyString(); }
        }

        public int? VirtualTestSubTypeId { get; set; }
        public int VirtualTestSourceId { get; set; }
        public int? NavigationMethodId { get; set; }

        public bool? IsTeacherLed { get; set; }
        //TOOD: Task: [LNKT-30796] Phase 2
        public int? QuestionGroupCount { get; set; }
        public int? DataSetOriginID { get; set; }
        public int? DataSetCategoryID { get; set; }
        public int? ParentTestID { get; set; }
        public int? OriginalTestID { get; set; }
        public int? AchievementLevelSettingID { get; set; }
        public int? AuthorUserID { get; set; }
    }
}
