using Autofac;
using Services.Wrapper.HomeAssistant.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Wrapper.HomeAssistant.Modules
{
    public class CommonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ProgramStartedInformation>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
