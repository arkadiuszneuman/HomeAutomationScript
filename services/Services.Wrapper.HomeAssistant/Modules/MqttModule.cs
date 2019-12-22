using Autofac;
using MQTTnet;
using Services.Wrapper.HomeAssistant.MQTT;
using Services.Wrapper.HomeAssistant.MQTT.Topics.SubscribedTopics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Wrapper.HomeAssistant.Modules
{
    public class MqttModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MqttManager>()
                .SingleInstance();

            builder.RegisterType<MqttFactory>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(type => type.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISubscribedTopic<>)) && type.IsClass)
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
