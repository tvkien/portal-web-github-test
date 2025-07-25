using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.TestResultRemover
{
    public class ClassAdminSchool
    {
        private string className = string.Empty;

        public int UserId { get; set; }
        public int ClassId { get; set; }
        public int SchoolId { get; set; }

        public string ClassName
        {
            get { return className; }
            set { className = value; }
        } 
    }
}
