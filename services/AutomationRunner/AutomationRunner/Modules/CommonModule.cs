using Autofac;

namespace AutomationRunner.Modules
{
    public class CommonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(a => a.Namespace == "AutomationRunner.Common")
                .AsImplementedInterfaces()
                .AsSelf();
        }
    }
}
