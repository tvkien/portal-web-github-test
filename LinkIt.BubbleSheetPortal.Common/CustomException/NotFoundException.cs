using System;

namespace LinkIt.BubbleSheetPortal.Common.CustomException
{
    public class NotFoundException: Exception
    {
        public NotFoundException(string message) : base(message)
        {

        }
    }
}
