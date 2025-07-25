using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.TestMaker
{
    public class QTIItemSubTestMaker
    {
        public int QTIItemSubID { get; set; }
        public int QTIItemID { get; set; }
        public int QTISchemaID { get; set; }
        public string CorrectAnswer { get; set; }
        public string ResponseIdentifier { get; set; }
        public string ResponseProcessing { get; set; }
        public int? ResponseProcessingTypeID { get; set; }

        public int PointsPossible { get; set; } //TODO: Bind data from xmlContent
        public string Depending { get; set; }
        public bool? Major { get; set; }
    }
}
