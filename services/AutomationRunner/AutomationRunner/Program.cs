﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AutomationRunner
{
    class Program
    {
        public static string[] CommandLineArguments { get; set; }

        public static async Task Main(string[] args)
        {
            CommandLineArguments = args;

            var builder = new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(ConfigureContainer)
                .ConfigureHostConfiguration(ConfigureHostConfiguration)
                .ConfigureLogging(ConfigureLogging);

            await builder.RunConsoleAsync();

        }

        private static void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAssemblyModules(typeof(Program).Assembly);
        }

        private static void ConfigureHostConfiguration(IConfigurationBuilder configHost)
        {
            var environment = Environment.GetEnvironmentVariable("WRAPPER_ENVIRONMENT");

            configHost.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables();
        }

        private static void ConfigureLogging(HostBuilderContext hostContext, ILoggingBuilder logging)
        {
            logging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
            logging.AddConsole();

            var elasticUri = hostContext.Configuration["ElasticConfiguration:Uri"];
            var elasticIndexFormat = hostContext.Configuration["ElasticConfiguration:IndexFormat"];

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = elasticIndexFormat,
                    MinimumLogEventLevel = LogEventLevel.Verbose,
                    FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate)
                })
                .MinimumLevel.Verbose()
                .CreateLogger();

            logging.AddSerilog();
        }
    }
}
