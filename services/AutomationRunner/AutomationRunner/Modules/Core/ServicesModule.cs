﻿using Autofac;
using AutomationRunner.Core.Entities.Services.Models;
using AutomationRunner.Core.Scenes;

namespace AutomationRunner.Modules.Core
{
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(typeof(IService).Assembly)
               .Where(a => a.Namespace?.StartsWith("AutomationRunner.Core.Entities.Services") ?? false)
               .AsImplementedInterfaces()
               .AsSelf();

            builder.RegisterType<ScenesActivator>()
                .SingleInstance();
        }
    }
}
