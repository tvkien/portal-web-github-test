using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.DTOs.Commons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent
{
    public class UpdateParentRequestModel : ValidatableEntity<UpdateParentRequestModel>
    {
        public int UserId { get; set; }
        public int CurrentUserRoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public string UserName { get; set; }
        public int ParentId { get; set; }

        public List<MetaDataKeyValueDto> ParentMetaDatas { get; set; }
        public List<StudentParentCustom> StudentParents { get; set; }
        public string Phone
        {
            get
            {
                return ParentMetaDatas?.FirstOrDefault(o => o.Name.Equals(ContaintUtil.PARENT_PHONE_COLUMN, StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty;
            }
        }
    }
}
