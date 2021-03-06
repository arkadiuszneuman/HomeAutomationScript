﻿using Autofac;
using AutomationRunner.Core.Automations.Specific;

namespace AutomationRunner.Modules.Core
{
    public class AutomationsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(typeof(IEntityStateAutomation).Assembly)
               .Where(a => a.Namespace?.StartsWith("AutomationRunner.Core.Automations") ?? false)
               .AsImplementedInterfaces()
               .AsSelf()
               .SingleInstance();
        }
    }
}
