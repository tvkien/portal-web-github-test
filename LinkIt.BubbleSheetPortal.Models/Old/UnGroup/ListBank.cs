using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ListBank
    {
        private string name = string.Empty;
        private string gradeName = string.Empty;
        private string subjectName = string.Empty;

        public int BankId { get; set; }
        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
        
        public int BankAccessId { get; set; }
        public int CreateByUserId { get; set; }
        public int BankDistrictId { get; set; }
        public int DistrictId { get; set; }
        public int BankDistrictAccessId { get; set; }
        public bool Hide { get; set; }

        public int CreateBankDistrictId { get; set; }

        public int SubjectId { get; set; }
        public string SubjectName
        {
            get { return subjectName; }
            set { subjectName = value.ConvertNullToEmptyString(); }
        }

        public int GradeId { get; set; }

        public string GradeName
        {
            get { return gradeName; }
            set { gradeName = value.ConvertNullToEmptyString(); }
        }
        public bool Archived { get; set; }
    }
}
