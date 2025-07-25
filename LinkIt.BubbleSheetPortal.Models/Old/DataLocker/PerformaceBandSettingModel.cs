using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    public class PerformanceBandSettingScoreModel
    {
        public int VirtualTestID { get; set; }
        public string SubScoreName { get; set; }
        public string Bands { get; set; }
        public string Label { get; set; }
        public string Cutoffs { get; set; }
        public string ScoreType { get; set; }
        public string Color { get; set; }
        public int? Level { get; set; }
        public int LOCKED { get; set; }
        public List<string> BandsColor { get; set; }
        public List<string> Colors { get; set; }
        public List<string> CutOffsColor { get; set; }
    }
}
