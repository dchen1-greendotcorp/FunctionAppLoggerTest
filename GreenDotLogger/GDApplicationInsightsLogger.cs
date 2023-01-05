using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Text;

namespace GreenDotLogger
{
    

    public class GDApplicationInsightsLogger : ILogger
    {
        private readonly ApplicationInsightsLogger _appInsightLogger;
        private readonly TelemetryClient _client;
        private readonly string categoryName;
        private readonly ApplicationInsightsLoggerOptions loggerOptions;
        private readonly IMaskService _maskable;

        public GDApplicationInsightsLogger(TelemetryClient client, string categoryName, ApplicationInsightsLoggerOptions loggerOptions, IMaskService maskable)
        {
            _appInsightLogger = new ApplicationInsightsLogger(categoryName,client,  loggerOptions);
            this._client = client;
            this.categoryName = categoryName;
            this.loggerOptions = loggerOptions;
            _maskable = maskable;
        }


        public IDisposable BeginScope<TState>(TState state)
        {
            return _appInsightLogger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _appInsightLogger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //var client=_client;

            //var myformatter = formatter;

            //TState newstate = _maskable.Mask(state);

            Func<TState, Exception, string> func = _maskable.Mask;
            _appInsightLogger.Log(logLevel, eventId, state, exception, func);

            //_appInsightLogger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
