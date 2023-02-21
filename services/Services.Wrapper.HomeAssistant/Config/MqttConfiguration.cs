using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Wrapper.HomeAssistant.Config
{
    public class MqttConfiguration
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
    }
}
