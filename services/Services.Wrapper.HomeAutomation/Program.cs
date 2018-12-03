using MQTTnet;
using MQTTnet.Client;
using RawRabbit.Configuration;
using RawRabbit.vNext;
using Services.Common.Models;
using System;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAutomation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var busConfig = new RawRabbitConfiguration
            {
                Username = "guest",
                Password = "guest",
                Port = 5672,
                VirtualHost = "/",
                Hostnames = { "localhost" },
            };
            var busClient = BusClientFactory.CreateDefault(busConfig);
            busClient.SubscribeAsync<TestModel>((async (msg, context) =>
            {
                Console.WriteLine(msg.Message);
            }));

            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId("Services.Wrapper.HomeAutomation")
                .WithTcpServer("localhost")
                .Build();

            client.Connected += async (s, e) =>
            {
                Console.WriteLine("MQTT connected");
            };

            await client.ConnectAsync(options);

            Console.WriteLine("Arek");

            while (true)
                Console.ReadKey();
        }
    }
}
