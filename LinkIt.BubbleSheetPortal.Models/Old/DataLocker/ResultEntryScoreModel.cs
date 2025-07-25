using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    public class ResultEntryScoreModel
    {
        public string ScoreName { get; set; }
        public string ScoreLable { get; set; }
        public int Order { get; set; }

        public VirtualTestCustomMetaModel MetaData { get; set; }
    }
}
