using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Formatting;
using System.Collections.Generic;
using System.IO;

namespace LinkIt.BubbleSheetPortal.Web.Models.Logging
{
    public class CustomSerilogJsonFormatter : ITextFormatter
    {
        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (IsJson<PortalLogEntry>(logEvent.MessageTemplate?.ToString()))
            {
                var portalLogEntry = JsonConvert.DeserializeObject<PortalLogEntry>(logEvent.MessageTemplate.ToString());
                var json = JsonConvert.SerializeObject(portalLogEntry);
                output.WriteLine(json);
            }
            else
            {
                var portalLogEntry = new PortalLogEntry
                {
                    ErrorLogs = new List<string> { logEvent.MessageTemplate?.ToString() }
                };
                var json = JsonConvert.SerializeObject(portalLogEntry);
                output.WriteLine(json);
            }
        }

        private static bool IsJson<T>(string json)
        {
            try
            {
                if (string.IsNullOrEmpty(json))
                {
                    return false;
                }

                JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch { return false; }
        }
    }
}
