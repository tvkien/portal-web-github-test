namespace LinkIt.BubbleSheetPortal.Services.ResponseProcessingParser
{
    public class ResponseProcessingParserFactory : IResponseProcessingParserFactory
    {
        private readonly IResponseProcessingParser _dragDropNumericalParser;
        public ResponseProcessingParserFactory(IResponseProcessingParser dragDropNumericalParser)
        {
            _dragDropNumericalParser = dragDropNumericalParser;
        }

        public IResponseProcessingParser GetResponseProcessingParser(int qtiSchemaID)
        {
            switch (qtiSchemaID)
            {
                case 35:
                {
                    return _dragDropNumericalParser;
                }
                default:
                    return null;
            }
        }
    }
}
