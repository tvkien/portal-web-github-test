using System;

namespace LinkIt.BubbleSheetPortal.Web.CustomException
{
    public class NoVaultException : Exception
    {
        public NoVaultException() : base("No vault is found")
        {
            
        }
    }
}