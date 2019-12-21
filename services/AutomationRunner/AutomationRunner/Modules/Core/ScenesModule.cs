using Autofac;
using AutomationRunner.Core.Automations.Specific;
using AutomationRunner.Core.Scenes.Specific;

namespace AutomationRunner.Modules.Core
{
    public class ScenesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(typeof(IScene).Assembly)
               .Where(a => a.Namespace.StartsWith("AutomationRunner.Core.Scenes"))
               .AsImplementedInterfaces()
               .AsSelf()
               .SingleInstance();
        }
    }
}
