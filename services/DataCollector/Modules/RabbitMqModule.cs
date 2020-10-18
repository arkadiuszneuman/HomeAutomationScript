using Autofac;
using DataCollector.Config;
using DataCollector.RabbitMq;
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

            builder.RegisterType<RabbitManager>()
                .SingleInstance();
        }
    }
}
