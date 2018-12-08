using Autofac;
using Microsoft.Extensions.Configuration;
using Services.Wrapper.HomeAssistant.Config;
using System;
using System.Linq;

namespace Services.Wrapper.HomeAssistant.Modules
{
    public class ConfigsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var configTypes = ThisAssembly.GetTypes()
                .Where(t => t?.Namespace?.StartsWith("Services.Wrapper.HomeAssistant.Config",
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
