namespace LinkIt.BubbleSheetPortal.VaultProvider.Model
{
    public class ASPStateDto
    {
        public string Mode { get; set; }
        public DatabaseServer SQLConnection { get; set; }
        public RedisServer RedisConnection { get; set; }
    }
}
