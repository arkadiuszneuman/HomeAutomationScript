using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using RawRabbit.Configuration;
using RawRabbit.vNext;
using Services.Common.Models;
using Services.Wrapper.HomeAssistant;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.Json;
using Services.Wrapper.HomeAssistant.Config;

namespace Services.Wrapper.HomeAutomation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var environment = Environment.GetEnvironmentVariable("WRAPPER_ENVIRONMENT");

                    config.AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{environment}.json", optional: true)
                        .AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<DaemonConfig>(hostContext.Configuration.GetSection("Daemon"));
                    services.Configure<RawRabbitConfiguration>(hostContext.Configuration.GetSection("Rabbitmq"));
                    services.Configure<MqttConfiguration>(hostContext.Configuration.GetSection("Mqtt"));

                    services.AddSingleton<IHostedService, DaemonService>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync();

        }
    }
}
