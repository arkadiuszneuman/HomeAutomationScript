using Autofac;

namespace AutomationRunner.Modules
{
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(ThisAssembly)
               .Where(a => a.Namespace.StartsWith("AutomationRunner.Entities.Services"))
               .AsImplementedInterfaces()
               .AsSelf();
        }
    }
}
