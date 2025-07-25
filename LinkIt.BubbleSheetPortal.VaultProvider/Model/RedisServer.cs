namespace LinkIt.BubbleSheetPortal.VaultProvider.Model
{
    public class RedisServer
    {
        public string RedisCacheName { get; set; }
        public string RedisCacheUserName { get; set; }
        public string RedisCachePassword { get; set; }
        public int RedisCachePort { get; set; }
        public bool RedisCacheSSL { get; set; }
        public int? ConnectTimeout { get; set; }
        public int? SyncTimeout { get; set; }
    }
}
