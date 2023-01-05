using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GreenDotLogger
{
    [ProviderAlias(Alias)]
    public class GDApplicationInsightsLoggerProvider : ILoggerProvider
    {
        internal const string Alias = "GDApplicationInsights";

        private readonly TelemetryClient _client;
        private readonly ApplicationInsightsLoggerOptions _loggerOptions;
        private readonly IMaskService _maskable;

        private bool _disposed;

        public GDApplicationInsightsLoggerProvider(TelemetryClient client, IOptions<ApplicationInsightsLoggerOptions> loggerOptions, IMaskService maskable)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _loggerOptions = loggerOptions?.Value ?? throw new ArgumentNullException(nameof(loggerOptions));
            _maskable = maskable?? throw new ArgumentNullException(nameof(maskable)); ;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new GDApplicationInsightsLogger(_client, categoryName, _loggerOptions,_maskable);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_client != null)
                {
                    try
                    {
                        _client.Flush();

                        // This sleep isn't ideal, but the flush is async so it's the best we have right now. This is
                        // being tracked at https://github.com/Microsoft/ApplicationInsights-dotnet/issues/407
                        Thread.Sleep(2000);
                    }
                    catch
                    {
                        // Ignore failures on dispose
                    }
                }

                _disposed = true;
            }
        }
    }
}
