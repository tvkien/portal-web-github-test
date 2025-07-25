using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent
{
    public class AssignStudentToParentFilterModelDto
    {
        public List<ListItem> Programs { get; set; }
        public List<ListItem> Grades { get; set; }
        public bool IsShowAddNewStudent { get; set; }
    }
}
