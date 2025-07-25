using System;
using System.Web.Mvc;
using FluentValidation;
using FluentValidation.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.App_Start
{
    public static class ValidationConfiguration
    {
        public static void Configure()
        {
            HtmlHelper.ClientValidationEnabled = false;
            HtmlHelper.UnobtrusiveJavaScriptEnabled = true;

            ValidatorOptions.ResourceProviderType = typeof(Envoc.Core.Shared.Model.ValidationMessageTemplates);

            FluentValidationModelValidatorProvider.Configure(provider =>
            {
                provider.ValidatorFactory = new DependencyResolverValidatorFactory();
            });
        }

        public class DependencyResolverValidatorFactory : ValidatorFactoryBase
        {
            public override IValidator CreateInstance(Type validatorType)
            {
                return (IValidator)DependencyResolver.Current.GetService(validatorType);
            }
        }
    }
}