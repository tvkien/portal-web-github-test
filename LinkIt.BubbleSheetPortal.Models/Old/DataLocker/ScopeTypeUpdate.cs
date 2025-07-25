using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.Old.DataLocker
{
    public class ScopeTypeUpdate
    {
        public int ScoreId { get; set; }
        public int? SubScoreId { get; set; }
        public string ScoreTypeOld { get; set; }
        public string ScoreTypeNew { get; set; }
    }
}
