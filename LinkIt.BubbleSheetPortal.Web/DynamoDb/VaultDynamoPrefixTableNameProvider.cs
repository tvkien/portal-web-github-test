using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.DynamoConnector.DynamoPrefixTableNameProvider;
using LinkIt.BubbleSheetPortal.VaultProvider;
using LinkIt.BubbleSheetPortal.VaultProvider.Model;
using LinkIt.BubbleSheetPortal.Web.CustomException;
using LinkIt.BubbleSheetPortal.Web.Resolver;

namespace LinkIt.BubbleSheetPortal.Web.DynamoDb
{
    public class VaultDynamoPrefixTableNameProvider:IDynamoPrefixTableNameProvider
    {
        private static Vault Vault
        {
            get
            {
                var vaultProvider = IoCContainer.GetService<IVaultProvider>(DependencyResolver.Current);
                var vault = vaultProvider.GetVaultByCode();

                if (vault == null)
                {
                    throw new NoVaultException();
                }

                return vault;
            }
        }

        public string DynamoPrefixTableName
        {
            get { return Vault.AppSetting.DynamoPrefixTableName; }
        }
    }
}
