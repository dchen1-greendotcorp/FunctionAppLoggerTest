using GreenDotShares;
using FunctionAppLoggerTest;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]
namespace FunctionAppLoggerTest
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            //settings include app insight connection
            builder.ConfigurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.GetContext().Configuration;

            //register GDApplicationInsights logger provider
            builder.Services.AddGDApplicationInsights(config);

            //implement your own MaskService and register here
            builder.Services.AddSingleton<IMaskService, MaskService>();

            // set logger filters 
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter<GDApplicationInsightsLoggerProvider>(
                       "", LogLevel.Information);
                loggingBuilder.AddFilter<GDApplicationInsightsLoggerProvider>(
                       "", LogLevel.Trace);

                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);//Trace
            });

            //log startup information
            var serviceProvider = builder.Services.BuildServiceProvider();
            var GDLoggerProvider = serviceProvider.GetRequiredService<GDApplicationInsightsLoggerProvider>();
            var logger = GDLoggerProvider.CreateLogger("Startup");
            logger.LogInformation("Got Here in Startup");
        }
    }
}
