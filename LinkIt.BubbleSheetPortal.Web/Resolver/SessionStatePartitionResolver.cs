using System;
using System.Web;
using LinkIt.BubbleSheetPortal.Web.CustomException;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;

namespace LinkIt.BubbleSheetPortal.Web.Resolver
{
    public class SessionStatePartitionResolver : IPartitionResolver
    {
        public void Initialize()
        {
            //throw new NotImplementedException();
        }

        public string ResolvePartition(object key)
        {
            try
            {
                var result = LinkitConfigurationManager.GetDatabaseSettings().SessionStateLinkit;
                return result;
            }
            catch (NoVaultException)
            {
                var result = LinkitConfigurationManager.GetDefaultDatabaseSettings().SessionStateLinkit;
                return result;
            }
        }
    }
}