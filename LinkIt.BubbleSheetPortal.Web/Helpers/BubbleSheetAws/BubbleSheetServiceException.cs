using System;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws
{
    public class BubbleSheetServiceException : Exception
    {
        public BubbleSheetServiceException(string errorMessage) : base(errorMessage)
        {
            
        }
    }
}