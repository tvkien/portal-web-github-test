using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    public class EntryResultStudentModel
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
                {
                    return string.Format("{0}{1}", FirstName, LastName);
                }
                else
                {
                    return string.Format("{0}, {1}", FirstName, LastName);
                }
            } 
        }

        public DateTime ResultDate { get; set; }
        public string ResultDateString
        {
            get { return ResultDate.DisplayDateWithFormat(); }
        }
        public decimal BySightDecimal { get; set; }
       public string ScoreText { get; set; }
        public decimal ScoreDecimal { get; set; }
        public decimal ScoreRange { get; set; }
        public decimal ScorePercent { get; set; }
        public string ScoreDropdownList { get; set; }
        public string ScoreAlphanumeric { get; set; }

    }
}
