using RabbitMQ.Client;
using RawRabbit.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Wrapper.HomeAssistant.Config
{
    public class RabbitMqConfiguration
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
    }
}
