using System.Collections.Generic;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AuthorGroup : ValidatableEntity<AuthorGroup>
    {
        public int Id { get; set; }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public int Level { get; set; }
        public int? SchoolId { get; set; }
        public int? DistrictId { get; set; }
        public int? StateId { get; set; }
        public int? UserId { get; set; }
    }
}