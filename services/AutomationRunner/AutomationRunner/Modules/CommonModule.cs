using Autofac;
using AutomationRunner.Common.EntityLoader;

namespace AutomationRunner.Modules
{
    public class CommonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(a => a.Namespace.StartsWith("AutomationRunner.Common"))
                .AsImplementedInterfaces()
                .AsSelf();

            builder.RegisterType<EntityLoader>()
                .SingleInstance();
        }
    }
}
