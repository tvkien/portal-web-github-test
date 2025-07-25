namespace LinkIt.BubbleSheetPortal.Models
{
    public class TTLConfigs
    {
        public int ID { get; set; }        
        public string DynamoTableName { get; set; }
        public int RetentionInDay { get; set; }
    }
}
