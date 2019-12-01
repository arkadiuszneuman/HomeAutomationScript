using Autofac;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomationRunner.Modules
{
    public class ConfigsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var configTypes = ThisAssembly.GetTypes()
                .Where(t => t?.Namespace?.StartsWith("AutomationRunner.Config",
                    StringComparison.InvariantCultureIgnoreCase) ?? false);

            foreach (var configType in configTypes)
            {
                builder.Register(c =>
                {
                    var configurationRoot = c.Resolve<IConfigurationRoot>();
                    var configSectionName = configType.Name.Replace("Configuration", "").Replace("Config", "");

                    var configuration = Activator.CreateInstance(configType);
                    configurationRoot.GetSection(configSectionName).Bind(configuration);

                    return configuration;
                })
                .As(configType)
                .SingleInstance();
            }
        }
    }
}
