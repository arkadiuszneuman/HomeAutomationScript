using Autofac;
using DataCollector.Config;
using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.vNext;

namespace DataCollector.Modules
{
    public class RabbitMqModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c =>
            {
                var rabbitMqConfig = c.Resolve<RabbitMqConfig>();
                var busConfig = new RawRabbitConfiguration
                {
                    Port = rabbitMqConfig.Port,
                    Hostnames = { rabbitMqConfig.Hostname }
                };

                return BusClientFactory.CreateDefault(busConfig);
            })
            .As<IBusClient>()
            .SingleInstance();
        }
    }
}
