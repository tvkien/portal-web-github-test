using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using  LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    [Serializable]
    public class StudentNavigationViewModel
    {
        public bool IsParentRole { get; set; }
        public int SelectedStudentId { get; set; }
        public List<StudentOfParentViewModel> StudentsOfParent { get; set; }
    }
}