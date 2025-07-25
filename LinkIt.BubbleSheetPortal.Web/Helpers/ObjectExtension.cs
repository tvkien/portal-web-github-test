using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public static class ObjectExtension
    {
        public static string SerializeObject(this object obj, bool isCamelCase = false)
        {
            if (isCamelCase)
            {
                return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }
            return JsonConvert.SerializeObject(obj);
        }
    }
}
