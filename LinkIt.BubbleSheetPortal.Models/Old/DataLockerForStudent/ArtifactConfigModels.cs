using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent
{
    public class ArtifactConfigModels
    {
        public int VirtualTestID { get; set; }
        public string Level { get; set; }
        public string Label { get; set; }
        public ArtifactConfigDetailModels Value { get; set; }
    }
    public class ArtifactConfigDetailModels
    {
        public int VirtualTestID { get; set; }
        public string Level { get; set; }
        public string Label { get; set; }
    }
}
