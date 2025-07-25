using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent
{
  public  class AvailableStudentViewModel
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public string Code { get; set; }
        public string Gender { get; set; }
        public string Grade { get; set; }
    }
}
