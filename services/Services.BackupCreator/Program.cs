using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Services.BackupCreator
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
                .ConfigureLogging(ConfigureLogging);

            await builder.RunConsoleAsync();

            //Console.WriteLine("Hello World!");

            //RunProgramRunExample().GetAwaiter().GetResult();

            //new CreatePackedBackupTask().Pack();

            //Console.ReadKey();
        }

        private static void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAssemblyModules(typeof(Program).Assembly);
        }

        private static void ConfigureLogging(HostBuilderContext hostContext, ILoggingBuilder logging)
        {
            logging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
            logging.AddConsole();
        }
    }
}
