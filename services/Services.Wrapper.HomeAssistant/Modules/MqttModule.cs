using Autofac;
using MQTTnet;
using Services.Wrapper.HomeAssistant.MQTT;
using System;
using System.Collections.Generic;
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
        }
    }
}
