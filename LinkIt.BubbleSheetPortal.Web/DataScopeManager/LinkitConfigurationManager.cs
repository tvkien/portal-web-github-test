using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.Configugration;
using LinkIt.BubbleSheetPortal.Models.Old.Configugration;
using LinkIt.BubbleSheetPortal.VaultProvider;
using LinkIt.BubbleSheetPortal.VaultProvider.Model;
using LinkIt.BubbleSheetPortal.Web.CustomException;
using LinkIt.BubbleSheetPortal.Web.Resolver;

namespace LinkIt.BubbleSheetPortal.Web.DataScopeManager
{
    public static class LinkitConfigurationManager
    {
        public static bool IsVaultException
        {
            get
            {
                var result = HttpContext.Current.Items["IsNoVaultExceptionKey"] as bool?;
                return result.GetValueOrDefault(false);
            }
            set
            {
                HttpContext.Current.Items["IsNoVaultExceptionKey"] = value;
            }
        }

        public static Vault Vault
        {
            get
            {
                var vault = GetVault();

                if (vault == null)
                {
                    IsVaultException = true;
                    throw new NoVaultException();
                }

                return vault;
            }
        }

        public static Vault GetVault(HttpContext context)
        {
            var code = GetCodeFromUrl(context);
            var key = $"VaultTable-{code}";
            var vault = CacheManager.Get<Vault>(key) ?? GetVault();
            return vault;
        }

        public static bool CanAccessVault
        {
            get
            {
                var vault = GetVault();
                return vault != null;
            }
        }

        private static Vault GetVault()
        {
            var vaultProvider = IoCContainer.GetService<IVaultProvider>(DependencyResolver.Current);
            return vaultProvider.GetVaultByCode();
        }

        private static LinkitSettings LinkitSettings
        {
            get
            {
                var settings = HttpContext.Current != null ? HttpContext.Current.Items[Constanst.LinkitSettingsKey] as LinkitSettings : null;
                if (settings == null || settings.S3Settings == null || settings.DatabaseSettings == null)
                {
                    settings = BuildLinkitSettings(Vault);
                    if (HttpContext.Current != null)
                        HttpContext.Current.Items[Constanst.LinkitSettingsKey] = settings;
                }

                return settings;
            }
        }

        public static LinkitSettings GetLinkitSettings()
        {
            var result = LinkitSettings;
            return result;
        }

        public static S3Settings GetS3Settings()
        {
            var result = LinkitSettings.S3Settings;
            return result;
        }

        public static EmailCredentialSetting GetEmailCredentialSetting(string key)
        {
            return LinkitSettings.EmailCredentialSettings.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase))
                ?? new EmailCredentialSetting
                {
                    Host = "smtp-relay.gmail.com",
                    Port = 587
                };
        }

        public static DatabaseSettings GetDatabaseSettings()
        {
            var result = LinkitSettings.DatabaseSettings;
            return result;
        }
        public static TTLConfigsModel GetTTLConfigs()
        {
            var result = LinkitSettings.TTLConfigs;
            return result;
        }
        public static DatabaseSettings GetDefaultDatabaseSettings()
        {
            return BuildDatabaseSettings(Vault);
        }

        private static DatabaseSettings BuildDatabaseSettings(Vault vault)
        {
            var result = new DatabaseSettings();
            if (vault == null || vault.DBServers == null) return result;

            result.LinkItConnectionString = BuildConnectionString("LinkItConnectionString", vault.DBServers);
            result.S3PortalLinkContext = BuildEdmxConnectionString("LinkItConnectionString", vault.DBServers);
            result.AdminReportingLogConnectionString = BuildConnectionString("AdminReportingLogConnectionString", vault.DBServers);
            result.SessionStateLinkit = BuildConnectionString("SessionStateLinkit", vault.DBServers);
            result.SessionStateSQLConnectionString = BuildConnectionString(vault.ASPState?.SQLConnection);
            result.SessionStateRedisConnectionString = BuildConnectionString(vault.ASPState?.RedisConnection);

            return result;
        }

        private static string BuildConnectionString(string connectionName, List<DatabaseServer> databaseServers, bool isSessionState = false)
        {
            if (string.IsNullOrWhiteSpace(connectionName) || databaseServers == null || databaseServers.Count == 0) return String.Empty;

            var connectionStringFormat = isSessionState == false
                ? "Data Source={0};Initial Catalog={3};User ID={1};Password={2}"
                : "data source={0};user id={1};password={2};";

            var databaseServer = databaseServers.FirstOrDefault(o => o.Name == connectionName);
            if (databaseServer == null) return string.Empty;

            var result = string.Format(connectionStringFormat, databaseServer.DataSource,
                databaseServer.PortalUserID, databaseServer.PortalPassword, databaseServer.DatabaseName);
            return result;
        }

        private static string BuildEdmxConnectionString(string connectionName, List<DatabaseServer> databaseServers)
        {
            if (string.IsNullOrWhiteSpace(connectionName) || databaseServers == null || databaseServers.Count == 0) return String.Empty;

            var connectionStringFormat = "metadata=res://*/Entities.S3PortalLinkContext.csdl|res://*/Entities.S3PortalLinkContext.ssdl|res://*/Entities.S3PortalLinkContext.msl;provider=System.Data.SqlClient;provider connection string=\"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};MultipleActiveResultSets=True\"";

            var databaseServer = databaseServers.FirstOrDefault(o => o.Name == connectionName);
            if (databaseServer == null) return string.Empty;

            var result = string.Format(connectionStringFormat, databaseServer.DataSource, databaseServer.DatabaseName,
                databaseServer.PortalUserID, databaseServer.PortalPassword);
            return result;
        }

        private static LinkitSettings BuildLinkitSettings(Vault vault)
        {
            var result = new LinkitSettings();
            if (vault == null) return result;

            result.S3Settings = BuildS3Settings(vault.VaultS3);
            result.DatabaseSettings = BuildDatabaseSettings(vault);
            result.DatabaseID = vault.DatabaseID;
            result.AppSettings = BuildAppSettings(vault.AppSetting);
            result.TTLConfigs = BuildTTLConfigs(vault.TTLConfigs);
            result.EmailCredentialSettings = BuildEmailSettings(vault.EmailCredentials);
            return result;
        }

        private static List<EmailCredentialSetting> BuildEmailSettings(List<EmailCredential> emailCredentials)
        {
            return emailCredentials.Select(x => new EmailCredentialSetting
            {
                Key = x.Name,
                Host = x.CredentialSetting.Host,
                Port = x.CredentialSetting.Port,
                UserName = x.CredentialSetting.UserName,
                Password = x.CredentialSetting.Password
            }).ToList() ?? new List<EmailCredentialSetting>();
        }

        private static S3Settings BuildS3Settings(VaultS3 vaultS3)
        {
            if (vaultS3 == null) return new S3Settings();

            var result = new S3Settings
            {
                ACTReportBucket = vaultS3.ACTReportBucket,
                AUVirtualTestBucketName = vaultS3.AUVirtualTestBucketName,
                AnswerKeySampleFileBucketName = vaultS3.AnswerKeySampleFileBucketName,
                ChytenReportBucket = vaultS3.ChytenReportBucket,
                GuideBucketName = vaultS3.GuideBucketName,
                LessonBucketName = vaultS3.LessonBucketName,
                RubricBucketName = vaultS3.RubricBucketName,
                S3ItemBucketName = vaultS3.S3ItemBucketName,
                SGOBucketName = vaultS3.SGOBucketName,
                AblesReportBucket = vaultS3.AblesReportBucket,
                ReportPrintingBucketName = vaultS3.ReportPrintingBucketName,
                HelpResourceBucket = vaultS3.HelpResourceBucket,
                NavigatorBucket = vaultS3.NavigatorBucket,
                BubbleSheetBucketName = vaultS3.BubbleSheetBucketName,

                CssBucketName = vaultS3.CssBucketName,
                S3CSSKey = string.Format("https://{0}.s3.amazonaws.com/{1}", vaultS3.CssBucketName, GetSetting(S3SettingKeys.S3CSSKeyFolder)),
                SlideShowKey = string.Format("https://{0}.s3.amazonaws.com/{1}", vaultS3.CssBucketName, GetSetting(S3SettingKeys.SlideShowKeyFolder)),

                S3Domain = GetSetting(S3SettingKeys.S3Domain),
                TLDSBucket = vaultS3.TLDSBucket,
                ExportClassTestAssBucket = vaultS3.ExportClassTestAssBucket,
                DTLBucket = vaultS3.TestResultExtractBucketName,
                S3AssessmentArtifactBucketName = vaultS3.S3AssessmentArtifactBucketName,
                S3AssessmentArtifactFolderName = vaultS3.S3AssessmentArtifactFolderName
            };

            return result;
        }

        private static TTLConfigsModel BuildTTLConfigs(TTLConfigs configs)
        {
            if (configs == null) return new TTLConfigsModel();
            var result = new TTLConfigsModel()
            {
                SGOManagerLog = configs.SGOManagerLog
            };

            return result;
        }
        private static string GetSetting(string name)
        {
            var result = ConfigurationManager.AppSettings[name];
            return result;
        }

        private static LinkIt.BubbleSheetPortal.Models.Configugration.AppSettings BuildAppSettings(LinkIt.BubbleSheetPortal.VaultProvider.Model.AppSettings appSettings)
        {
            var result = new LinkIt.BubbleSheetPortal.Models.Configugration.AppSettings();

            result.DataFileUploadPath = appSettings.DataFileUploadPath;
            result.DataFileUploadPathLocalAppServer = appSettings.DataFileUploadPathLocalAppServer;
            result.ThirdPartyItemMediaPathLocalAppServer = appSettings.ThirdPartyItemMediaPathLocalAppServer;
            result.AblesSecureKey = appSettings.AblesSecureKey;
            result.AblesGenerateReportAPIURL = appSettings.AblesGenerateReportAPIURL;

            return result;
        }
        public static LinkIt.BubbleSheetPortal.Models.Configugration.AppSettings AppSettings
        {
            get
            {
                var result = LinkitSettings.AppSettings;
                return result;
            }

        }

        public static string BuildConnectionString(DatabaseServer databaseServer)
        {
            if (databaseServer == null) return String.Empty;
            string connectionStringFormat = "Data Source={0};Initial Catalog={3};User ID={1};Password={2};TrustServerCertificate=True";
            var result = string.Format(connectionStringFormat,
                databaseServer.DataSource,
                databaseServer.PortalUserID,
                databaseServer.PortalPassword,
                databaseServer.DatabaseName);
            return result;
        }

        public static string BuildConnectionString(RedisServer databaseServer)
        {
            if (databaseServer == null) return String.Empty;
            string connectionStringFormat = "{0}:{1},ssl={2},user={3},password={4},abortConnect=false,connectTimeout={5},syncTimeout={6}";
            var result = string.Format(connectionStringFormat,
                databaseServer.RedisCacheName,
                databaseServer.RedisCachePort,
                databaseServer.RedisCacheSSL,
                databaseServer.RedisCacheUserName,
                databaseServer.RedisCachePassword,
                databaseServer.ConnectTimeout ?? 30000,
                databaseServer.SyncTimeout ?? 30000);
            return result;
        }

        public static string GetCodeFromUrl(HttpContext context)
        {
            if (context != null && context.Request != null)
            {
                string subDomain = context.Request.Url.Host.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                return subDomain;
            }
            return string.Empty;
        }
    }
}
