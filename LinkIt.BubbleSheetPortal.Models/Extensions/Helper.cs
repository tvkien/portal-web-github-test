using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Models.Extensions
{
    public static class Helper
    {
        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);

            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
