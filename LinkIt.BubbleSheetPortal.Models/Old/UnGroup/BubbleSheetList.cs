using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BubbleSheetList : ValidatableEntity<BubbleSheetList>
    {
        private string ticket = string.Empty;
        private string subjectName = string.Empty;
        private string gradeName = string.Empty;
        private string bankName = string.Empty;
        private string testName = string.Empty;

        public int UserId { get; set; }
        public int GradedCount { get; set; }
        public DateTime DateCreated { get; set; }

        public string Ticket
        {
            get { return ticket; }
            set { ticket = value.ConvertNullToEmptyString(); }
        }

        public string SubjectName
        {
            get { return subjectName; }
            set { subjectName = value.ConvertNullToEmptyString(); }
        }

        public string GradeName
        {
            get { return gradeName; }
            set { gradeName = value.ConvertNullToEmptyString(); }
        }

        public string BankName
        {
            get { return bankName; }
            set { bankName = value.ConvertNullToEmptyString(); }
        }

        public string TestName
        {
            get { return testName; }
            set { testName = value.ConvertNullToEmptyString(); }
        }
    }
}