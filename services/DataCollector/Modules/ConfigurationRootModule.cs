using Autofac;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCollector.Modules
{
    public class ConfigurationRootModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c =>
            {
                var environment = Environment.GetEnvironmentVariable("WRAPPER_ENVIRONMENT");

                var configurationBuilder = new ConfigurationBuilder();

                configurationBuilder.AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{environment}.json", optional: true)
                    .AddEnvironmentVariables();

                return configurationBuilder.Build();
            })
            .SingleInstance();
        }
    }
}
