using RawRabbit.Configuration;
using RawRabbit.vNext;
using Services.Common.Models;
using System;

namespace Services.Wrapper.HomeAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var busConfig = new RawRabbitConfiguration
            {
                Username = "guest",
                Password = "guest",
                Port = 5672,
                VirtualHost = "/",
                Hostnames = { "localhost" }
            };
            var busClient = BusClientFactory.CreateDefault(busConfig);
            busClient.SubscribeAsync<TestModel>((async (msg, context) =>
            {
                Console.WriteLine(msg.Message);
            }));

            Console.WriteLine("Arek");

            while(true)
                Console.ReadKey();
        }
    }
}
