using Newtonsoft.Json;

namespace GenericLog
{
    public class ActionLogDTO
    {
        private KeyLogs _keylogs;
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Method { get; set; }
        public bool IsLogInput { get; set; }
        public bool IsLogOutput { get; set; }
        public bool IsDisable { get; set; }
        public string KeyLogString { get; set; }
        public KeyLogs KeyLogs
        {
            get
            {
                if (_keylogs == null)
                {
                    try
                    {
                        _keylogs = JsonConvert.DeserializeObject<KeyLogs>(KeyLogString);
                    }
                    catch (System.Exception)
                    {
                        _keylogs = new KeyLogs();
                    }
                }
                   
                return _keylogs;
            }
        }
    }

    public class KeyLogs
    {
        public string KeyName_1 { get; set; }
        public string KeyName_2 { get; set; }
        public string KeyName_3 { get; set; }
        public string KeyName_4 { get; set; }
        public string OutName_1 { get; set; }
        public string OutName_2 { get; set; }
        public string OutName_3 { get; set; }
        public string OutName_4 { get; set; }
    }
}
