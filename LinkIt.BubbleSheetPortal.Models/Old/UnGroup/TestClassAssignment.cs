using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestClassAssignment
    {
        private List<string> studentIdList = new List<string>();
        public int TestId { get; set; }
        public int ClassId { get; set; }

        public List<string> StudentIdList
        {
            get { return studentIdList; }
            set { studentIdList = value ?? new List<string>(); }
        }

        public bool IsClassAssignment { get; set; }

    }
}
