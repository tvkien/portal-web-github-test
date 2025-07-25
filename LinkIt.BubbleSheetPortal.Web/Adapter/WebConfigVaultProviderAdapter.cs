using System;
using System.Collections.Generic;
using System.Configuration;
using LinkIt.BubbleSheetPortal.VaultProvider;
using LinkIt.BubbleSheetPortal.VaultProvider.Model;

namespace LinkIt.BubbleSheetPortal.Web.Adapter
{
    public class WebConfigVaultProviderAdapter : IVaultProvider
    {
        public Vault GetVault(string code = "")
        {
            var result = new Vault();
            result.DBServers = new List<DatabaseServer>();
            result.DBServers.Add(CreateDatabaseServer("LinkItConnectionString"));
            result.DBServers.Add(CreateDatabaseServer("S3PortalLinkContext"));
            result.DBServers.Add(CreateDatabaseServer("LinkIt.BubbleSheetPortal.Data.Properties.Settings.AdminReportingLogConnectionString"));
            result.DBServers.Add(CreateDatabaseServer("IsolatingTestTakerConnectionString"));
            result.DBServers.Add(CreateDatabaseServer("SessionStateLinkit"));

            result.VaultS3 = new VaultS3
            {
                ACTReportBucket = GetSetting("ACTReportBucket"),
            };

            return result;
        }

        public Vault GetByEnvironmentIdentifier(string environmentIdentifier)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Vault> GetAllVaults()
        {
            throw new NotImplementedException();
        }

        public string GetConnectionString(string code = "", bool isSessionState = false)
        {
            throw new NotImplementedException();
        }

        public string GetIsolatingConnectionString(string code = "")
        {
            throw new NotImplementedException();
        }

        public string GetLogConnectionString(string code = "")
        {
            throw new NotImplementedException();
        }

        public string GetSessionStateConnectionString(string code = "")
        {
            throw new NotImplementedException();
        }

        public Vault GetDefaultVault()
        {
            throw new NotImplementedException();
        }

        private DatabaseServer CreateDatabaseServer(string name)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name].ConnectionString;
            var result = new DatabaseServer
            {
                DatabaseName = Common.ConnectionStringParser.GetDatabaseName(connectionString),
                DataSource = Common.ConnectionStringParser.GetServerName(connectionString),
                PortalUserID = Common.ConnectionStringParser.GetUsername(connectionString),
                PortalPassword = Common.ConnectionStringParser.GetPassword(connectionString),
                Name = name
            };

            return result;
        }

        private string GetSetting(string name)
        {
            var result = ConfigurationManager.AppSettings[name];
            return result;
        }

        public List<string> GetAllEnvironmentIdentifier()
        {
            throw new NotImplementedException();
        }
    }
}