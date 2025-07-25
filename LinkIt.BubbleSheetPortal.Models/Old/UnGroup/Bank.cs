using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Bank : ValidatableEntity<Bank>, IIdentifiable
    {
        private string name = string.Empty;

        public int Id { get; set; }
        public int SubjectID { get; set; }
        public int CreatedByUserId { get; set; }

        public int? BankAccessID { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Archived { get; set; }
    }
}