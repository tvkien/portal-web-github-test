using System;

namespace LinkIt.BubbleSheetPortal.Common.CustomException
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message)
        {

        }
    }
}
