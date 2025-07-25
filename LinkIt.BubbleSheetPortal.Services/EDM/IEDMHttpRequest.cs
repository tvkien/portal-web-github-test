namespace LinkIt.BubbleSheetPortal.Services.EDM
{
    public interface IEDMHttpRequest
    {
        T SendPostRequest<T>(string requestUri, object payload = null);
        T SendGetRequest<T>(string requestUri);
        T SendDeleteRequest<T>(string requestUri, object payload = null);
    }
}
