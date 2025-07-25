using Amazon.DynamoDBv2;
using Amazon.SecretsManager;
using Autofac;
using Autofac.Integration.Mvc;

namespace LinkIt.BubbleSheetPortal.VaultProvider
{
    public static class Startup
    {
        public static void RegisterAmazonService(this ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(c =>
            {
                return new AmazonDynamoDBClient();
            }).As<IAmazonDynamoDB>().InstancePerHttpRequest();

            containerBuilder.Register(c =>
            {
                return new AmazonSecretsManagerClient();
            }).As<IAmazonSecretsManager>().InstancePerHttpRequest();

            containerBuilder.RegisterType<DynamoDBVaultProvider>().As<IVaultProvider>().InstancePerHttpRequest();
            containerBuilder.RegisterType<SecretsManager>().As<ISecretsManager>().InstancePerHttpRequest();
        }
    }
}
