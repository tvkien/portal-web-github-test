using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTI3pPassageProgress
    {
        public int QTI3pPassageProgressID { get; set; }
        public int Qti3pPassageID { get; set; }

        public int? Qti3pProgressPassageTypeID { get; set; }
        public int? Qti3pProgressPassageGenreID { get; set; }
        public string Lexile { get; set; }
        public string Spache { get; set; }
        public string DaleChall { get; set; }
        public string RMM { get; set; }

    }
}
