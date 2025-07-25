namespace LinkIt.BubbleSheetPortal.Services.Reporting
{
    public interface IReportingHttpClient
    {
        T Get<T>(string requestUri);
        T Put<T>(string requestUri, object payload = null);
    }
}
