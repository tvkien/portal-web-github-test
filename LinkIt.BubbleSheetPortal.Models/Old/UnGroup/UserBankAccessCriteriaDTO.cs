using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class UserBankAccessCriteriaDTO
    {
        private bool _filterByDistrict = true;
        public int UserId { get; set; }
        public int SubjectId { get; set; }
        public List<int> SubjectIds { get; set; }
        public List<string> SubjectNames { get; set; }
        public int BankAccessId { get; set; }
        public List<int> GradeIds { get; set; }

        public int SchoolId { get; set; }
        public int DistrictId { get; set; }

        public bool ShowArchived { get; set; }
        public bool HideCreatedByTeacher { get; set; }
        public bool HideCreatedByOthers { get; set; }

        public bool FilterByDistrict {
            get { return _filterByDistrict; }
            set { _filterByDistrict = value; }
        }

    }
}
