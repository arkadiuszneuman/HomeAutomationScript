using Autofac;
using Microsoft.Extensions.Configuration;
using System;

namespace AutomationRunner.Modules
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

                if (Program.CommandLineArguments != null)
                {
                    configurationBuilder.AddCommandLine(Program.CommandLineArguments);
                }

                return configurationBuilder.Build();
            })
            .SingleInstance();
        }
    }
}

