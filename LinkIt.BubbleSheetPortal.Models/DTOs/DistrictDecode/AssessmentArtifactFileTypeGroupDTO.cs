using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.DistrictDecode
{
    public class AssessmentArtifactFileTypeGroupDTO
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int MaxFileSizeInBytes { get; set; }
        public List<string> SupportFileType { get; set; }
    }
}
