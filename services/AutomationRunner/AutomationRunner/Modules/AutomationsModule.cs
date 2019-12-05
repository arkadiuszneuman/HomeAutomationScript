using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Modules
{
    public class AutomationsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(ThisAssembly)
               .Where(a => a.Namespace.StartsWith("AutomationRunner.Automations"))
               .AsImplementedInterfaces()
               .AsSelf();
        }
    }
}
