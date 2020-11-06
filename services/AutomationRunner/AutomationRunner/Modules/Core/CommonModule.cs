using Autofac;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;

namespace AutomationRunner.Modules.Core
{
    public class CommonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(typeof(DateTimeHelper).Assembly)
                .Where(a => a.Namespace?.StartsWith("AutomationRunner.Core.Common") ?? false)
                .AsImplementedInterfaces()
                .AsSelf();

            builder.RegisterType<HomeAssistantConnector>()
                .SingleInstance();

            builder.RegisterType<HomeAssistantWebSocketConnector>()
               .SingleInstance();
        }
    }
}
