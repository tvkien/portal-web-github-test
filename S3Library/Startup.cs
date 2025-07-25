using Amazon.S3;
using Autofac;
using Autofac.Integration.Mvc;
using System.Configuration;

namespace S3Library
{
    public static class Startup
    {
        public static void RegisterS3Service(this ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(c =>
            {
                var s3Domain = ConfigurationManager.AppSettings["S3Domain"];
                var config = new AmazonS3Config
                {
                    ServiceURL = s3Domain
                };
                return new AmazonS3Client(config);
            }).As<IAmazonS3>().InstancePerLifetimeScope();

            containerBuilder.RegisterType<S3Service>().As<IS3Service>().InstancePerHttpRequest();
        }
    }
}
