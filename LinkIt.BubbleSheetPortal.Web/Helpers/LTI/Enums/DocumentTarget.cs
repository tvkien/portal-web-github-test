using LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Utils;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Enums
{
    [JsonConverter(typeof(DocumentTargetConverter))]
    public enum DocumentTarget
    {
        None = 0,
        Iframe,
        Window
    }
}
