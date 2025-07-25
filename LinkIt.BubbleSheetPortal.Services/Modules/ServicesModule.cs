using Autofac;

namespace LinkIt.BubbleSheetPortal.Services.Modules
{
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly);
            base.Load(builder);
        }
    }
}
