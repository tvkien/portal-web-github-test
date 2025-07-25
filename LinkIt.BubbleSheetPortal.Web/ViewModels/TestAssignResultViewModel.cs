using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    [Serializable]
    public class TestAssignResultViewModel
    {
        public bool IsStudentAssign { get; set; }
        public bool IsClassAssign { get; set; }
        private List<TestStudentAssignResultViewModel>  lstStudentAssign = new List<TestStudentAssignResultViewModel>();
        public List<TestStudentAssignResultViewModel> ListStudentAssign {
            get { return lstStudentAssign; }
            set { lstStudentAssign = value; }
        }

        private List<TestClassAssignResultViewModel> lstClassAssign = new List<TestClassAssignResultViewModel>();
        public List<TestClassAssignResultViewModel> ListClassAssign {
            get { return lstClassAssign; }
            set { lstClassAssign = value; }

        }

        private List<TestGroupStudentAssignResultViewModel> lstGroupStudent = new List<TestGroupStudentAssignResultViewModel>();
        public List<TestGroupStudentAssignResultViewModel> ListGroupStudent
        {
            get { return lstGroupStudent; }
            set { lstGroupStudent = value; }

        }

        public List<ValidationFailure> ErrorList { get; set; }

        public string TestAssingCustomName { get; set; }

        public int PringtType { get; set; }

        public int TotalTabs
        {
            get
            {
                int i = 0;
                if (lstStudentAssign.Count > 0) i++;
                if (lstClassAssign.Count > 0) i++;
                if (lstGroupStudent.Count > 0) i++;
                return i;
            }
        }

        public int CurrentTab { get; set; }
    }
    [Serializable]
    public class TestClassAssignResultViewModel
    {
        public int ID { get; set; }
        public int ClassId { get; set; }
        public string SchoolName { get; set; }
        public string ClassName { get; set; }
        public string DistrictTermName { get; set; }
        public string TeacherLastName { get; set; }
        public string TeacherFirstName { get; set; }
        public string TestCode { get; set; }
        public string Test { get; set; }
        public DateTime Assigned { get; set; }
        public string TeacherName { get; set; }

        public string Tutorial { get; set; }

        public string PortalHyperLinkTestCode { get; set; }
        public string AuthenticationCode { get; set; }
        public string AssignmentDate { get; set; }
    }

    [Serializable]
    public class TestStudentAssignResultViewModel
    {
        public int ID { get; set; }
        public int StudentId { get; set; }
        public string StudentCode { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string TestCode { get; set; }
        public string ShortGUID { get; set; }
        public string Test { get; set; }
        public DateTime Assigned { get; set; }
        public string TeacherName { get; set; }
        public string StudentName { get; set; }
        public string Tutorial { get; set; }

        public string PortalHyperLinkTestCode { get; set; }

        public string ClassName { get; set; }
        public bool IsActive { get; set; } = false;
        public string AuthenticationCode { get; set; }
        public string AssignmentDate { get; set; }
    }
    [Serializable]
    public class TestGroupStudentAssignResultViewModel
    {
        public int ID { get; set; }
        public string SchoolName { get; set; }
        public string ClassName { get; set; }
        public string DistrictTermName { get; set; }
        public string TeacherName { get; set; }
        public string TeacherLastName { get; set; }
        public string TeacherFirstName { get; set; }

        public int StudentId { get; set; }
        public string StudentCode { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string TestCode { get; set; }
        public string ShortGUID { get; set; }
        public string Test { get; set; }
        public DateTime Assigned { get; set; }
        public string StudentName { get; set; }

        public string Tutorial { get; set; }

        public string PortalHyperLinkTestCode { get; set; }
        public string AuthenticationCode { get; set; }
        public string AssignmentDate { get; set; }
    }
}
