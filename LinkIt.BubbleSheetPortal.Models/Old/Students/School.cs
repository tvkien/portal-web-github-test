using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class School : ValidatableEntity<School>, IIdentifiable
    {
        private string name = string.Empty;
        private string code = string.Empty;
        private string stateCode = string.Empty;
        private string locationCode = string.Empty;

        public int DistrictId { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? Status { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public string Code
        {
            get { return code; }
            set { code = value.ConvertNullToEmptyString(); }
        }

        public string StateCode
        {
            get { return stateCode; }
            set { stateCode = value.ConvertNullToEmptyString(); }
        }

        public int? StateId { get; set; }

        public string LocationCode
        {
            get { return locationCode; }
            set { locationCode = value.ConvertNullToEmptyString(); }
        }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedUser { get; set; }
        public string ModifiedBy { get; set; }
    }
}