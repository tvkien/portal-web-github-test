using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Models
{
    public class TestAssignSameTestParam
    {
        private List<string> studentIdList = new List<string>();
        public int AssignmentType { get; set; }

        public List<string> StudentIdList
        {
            get { return studentIdList; }
            set { studentIdList = value ?? new List<string>(); }
        }

        public int ClassId { get; set; }
        public int TestId { get; set; }
        public int GroupId { get; set; }
        public bool IsUseRoster { get; set; }
        public bool IsGroupPrinting { get; set; }
        public string StudentIds { get; set; }
        public int? DistrictID { get; set; }
        public bool IsGenericBubbleSheet { get; set; }
        public string SSearch { get; set; }
    }
}
