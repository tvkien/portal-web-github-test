using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BubbleSheet : ValidatableEntity<BubbleSheet>, IIdentifiable
    {
        private string bubbleSheetCode = string.Empty;
        private string bubbleSize = string.Empty;
        private string ticket = string.Empty;
        private string studendIds = string.Empty;

        public int Id { get; set; }
        public int? ClassId { get; set; }
        public int? SchoolId { get; set; }
        public int? TeacherId { get; set; }
        public int? StudentId { get; set; }
        public int? TestId { get; set; }
        public int? DistrictTermId { get; set; }
        public int? UserId { get; set; }
        public int? CreatedByUserId { get; set; }
        public bool IsArchived { get; set; }
        public bool IsManualEntry { get; set; }
        public bool IsGenericSheet { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public int? PrintingGroupJobID { get; set; }

        public string BubbleSheetCode
        {
            get { return bubbleSheetCode; }
            set { bubbleSheetCode = value.ConvertNullToEmptyString(); }
        }

        public string BubbleSize
        {
            get { return bubbleSize; }
            set { bubbleSize = value.ConvertNullToEmptyString(); }
        }

        public string Ticket
        {
            get { return ticket; }
            set { ticket = value.ConvertNullToEmptyString(); }
        }

        public string StudentIds
        {
            get { return studendIds; }
            set { studendIds = value.ConvertNullToEmptyString(); }
        }

        public int? TestExtract { get; set; }

        public string ClassIds { get; set; }
    }
}