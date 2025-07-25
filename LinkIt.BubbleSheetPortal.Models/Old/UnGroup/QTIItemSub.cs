using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTIItemSub
    {
        public int QTIItemSubId { get; set; }
        public int QTIItemId { get; set; }
        public int QTISchemaId { get; set; }
        public string CorrectAnswer { get; set; }
        public string ResponseIdentifier { get; set; }
        public int PointsPossible { get; set; }
        public string ResponseProcessing { get; set; }
        public DateTime Updated { get; set; }
        public int? ResponseProcessingTypeId { get; set; }
        public int? SourceId { get; set; }
        public string Depending { get; set; }
        public bool? Major { get; set; }
    }
}
