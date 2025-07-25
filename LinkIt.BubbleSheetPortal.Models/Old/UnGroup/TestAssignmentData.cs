using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.ManagePreference;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestAssignmentData : ValidatableEntity<TestClassAssignment>
    {
        private List<string> studentIdList = new List<string>();
        public List<string> StudentIdList
        {
            get { return studentIdList; }
            set { studentIdList = value ?? new List<string>(); }
        }
        public int SchoolId { get; set; }
        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public int BankId { get; set; }
        public int TestId { get; set; }
        public int ClassId { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int DistrictTermId { get; set; }
        public int UserId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSchoolAdmin { get; set; }
        public bool IsPublisher { get; set; }
        public int AssignmentType { get; set; } //1 = assign single class, 2 = assign student, 3 = assign multible class
        public int GroupId { get; set; }

        public string TestName { get; set; }
        public string SubjectName { get; set; }
        public string GradeName { get; set; }
        public bool IsUseRoster { get; set; }
        public bool IsTeacherLed { get; set; }

        public int VirtualTestTimingId { get; set; }
        public int IsTutorialMode { get; set; }
        public TestPreferenceModel ObjTestPreferenceModel { get; set; }

        public bool IsLaunchTeacherLedTest { get; set; }

        public string ListOfDisplayQuestions { get; set; }

        public bool RequireTestTakerAuthentication { get; set; }
        public int StudentID { get; set; }
    }
}
