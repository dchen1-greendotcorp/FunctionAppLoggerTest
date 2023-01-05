using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Logging;

namespace GreenDotLogger
{
    public static class GreendotCoreStartHelper
    {
        public static IServiceCollection AddGDApplicationInsights(this IServiceCollection serviceCollection,IConfiguration configuration )
        {
            var telemetryConfig = TelemetryConfiguration.CreateDefault();

            telemetryConfig.ConnectionString = configuration["ApplicationInsights:ConnectionString"];

            TelemetryClient telemetryClient = new TelemetryClient(telemetryConfig);

            //We need Define our own service data
            telemetryClient.Context.GlobalProperties.Add(LogComponentInfo.ServiceOffering, configuration[LogComponentInfo.ServiceOffering]);
            telemetryClient.Context.GlobalProperties.Add(LogComponentInfo.ServiceLine, configuration[LogComponentInfo.ServiceLine]);
            telemetryClient.Context.GlobalProperties.Add(LogComponentInfo.ServiceComponenet, configuration[LogComponentInfo.ServiceComponenet]);
            telemetryClient.Context.GlobalProperties.Add(LogComponentInfo.Catalog, configuration[LogComponentInfo.Catalog]);
            telemetryClient.Context.GlobalProperties.Add(LogComponentInfo.Environment, configuration[LogComponentInfo.Environment]);

            serviceCollection.AddSingleton(telemetryConfig);
            serviceCollection.AddSingleton(telemetryClient);

            serviceCollection.AddSingleton<ApplicationInsightsLoggerOptions>();

            serviceCollection.AddSingleton<IMaskService, MaskService>();

            serviceCollection.AddSingleton<ILoggerProvider, GDApplicationInsightsLoggerProvider>();

            serviceCollection.AddSingleton<GDApplicationInsightsLoggerProvider>();

           return serviceCollection;
        }

        
    }
}
