using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOReportDataPoint
    {
        public int SgoDataPointId { get; set; }

		public string Name { get; set; }

        public int Type { get; set; }

		public string TypeName { get; set; }

		public string SubjectName { get; set; }

		public string GradeName { get; set; }

        public string RationaleGuidance { get; set; }

        public int ImprovementBasedDataPoint { get; set; }

        public int? ScoreType { get; set; }

        public string ScoreTypeName { get; set; }

        public int? VirtualTestId { get; set; }
    }
}
