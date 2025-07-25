using LinkIt.BubbleSheetPortal.Models.ResponseProcessingDTO;

namespace LinkIt.BubbleSheetPortal.Services.ResponseProcessingParser
{
    public interface IResponseProcessingParser
    {
        BaseResponseProcessingDTO Parse(string rpXml);
    }
}
