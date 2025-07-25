using System;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws
{
    public class GetRequestTimeoutException : Exception
    {
        public GetRequestTimeoutException(string message) : base(message){}
    }
}