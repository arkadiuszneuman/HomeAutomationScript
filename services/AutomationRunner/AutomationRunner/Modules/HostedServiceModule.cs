using Autofac;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Modules
{
    public class HostedServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<DaemonService>().As<IHostedService>().SingleInstance();
        }
    }
}
