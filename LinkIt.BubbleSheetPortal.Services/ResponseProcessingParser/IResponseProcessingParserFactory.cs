namespace LinkIt.BubbleSheetPortal.Services.ResponseProcessingParser
{
    public interface IResponseProcessingParserFactory
    {
        IResponseProcessingParser GetResponseProcessingParser(int qtiSchemaID);
    }
}
