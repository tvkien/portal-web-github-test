using LinkIt.BubbleSheetPortal.Models.DTOs.Commons;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent
{
    public class AddOrEditParentViewModelDto
    {
        public int ParentId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Code { get; set; } = "";
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public int StateId { get; set; }        
        public string UserName { get; set; } = "";
        public List<MetaDataKeyValueDto> ParentMetaDatas { get; set; }
    }
}
