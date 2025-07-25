using Autofac;
using FluentValidation;

namespace LinkIt.BubbleSheetPortal.Validators.Modules
{
    public class ValidatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
               .AsClosedTypesOf(typeof(IValidator<>))
               .AsImplementedInterfaces()
               .AsSelf();
            base.Load(builder);
        }
    }
}
