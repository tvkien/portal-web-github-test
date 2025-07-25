using System;

namespace LinkIt.BubbleSheetPortal.Common.CustomException
{
    public class IntegrationException : Exception
    {
        public IntegrationException(string message) : base(message)
        {
        }
    }
}
