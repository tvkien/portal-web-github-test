using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ConfirmDeassignParentViewModel
    {
        public int ParentID { get; set; }
        public List<StudentOfParentViewModel> ListStudent { get; set; }
    }

    [Serializable]
    public class StudentOfParentViewModel
    {
        public int StudentID { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
    }
}