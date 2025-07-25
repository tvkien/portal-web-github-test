using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent
{
    public class TestResultScoreAllArtifact
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public string Url { get; set; }

        public bool IsLink { get; set; }

        public DateTime UploadDate { get; set; }

        public string TagValue { get; set; }

        public int StudentId { get; set; }
        public int? TestResultScoreID { get; set; }
        public int? TestResultSubScoreID { get; set; }
    }
}
