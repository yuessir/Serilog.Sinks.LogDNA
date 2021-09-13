using System;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.Serialization;
using System.Xml.Schema;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.Http;
using Serilog.Sinks.LogDNA;

namespace Serilog
{


    public static class LogdnaExtensions
    {
        public static LoggerConfiguration WriteToLogDna(this LoggerSinkConfiguration sinkConfiguration, Action<ISinkHttpConfiguration> ex)
        {
            var config = new SinkHttpConfiguration();
            ex(config);
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (string.IsNullOrWhiteSpace(config.ApiKey)) throw new ArgumentNullException(nameof(config.ApiKey));
            if (string.IsNullOrWhiteSpace(config.IngestUrl)) throw new ArgumentNullException(nameof(config.IngestUrl));

            config.IngestUrl += (config.IngestUrl.Contains("?") ? "&" : "?") + $"hostname={Dns.GetHostName().ToLower()}";
            if (config.CommaSeparatedTags != null) config.IngestUrl += $"&tags={WebUtility.UrlEncode(config.CommaSeparatedTags)}";

            var envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                          Environment.GetEnvironmentVariable("ASPNET_ENVIRONMENT");
            if (sinkConfiguration == null)
                throw new ArgumentNullException(nameof(sinkConfiguration));
            config.Period = (config.Period ?? TimeSpan.FromSeconds(2.0));
            ITextFormatter textFormatter = new LogdnaTextFormatter(config.AppName ?? "unknown", envName);
            IBatchFormatter batchFormatter = new LogdnaBatchFormatter();
            IHttpClient httpClient = new LogdnaHttpClient(config.ApiKey);
            return sinkConfiguration.Http(config.IngestUrl, config.BatchPostingLimit, config.QueueLimit.Value, config.Period.Value,
                textFormatter, batchFormatter, config.RestrictedToMinimumLevel, httpClient);


        }
        public static LoggerConfiguration WriteToLogDnaDurable(this LoggerSinkConfiguration sinkConfiguration, Action<ISinkHttpConfiguration> ex)
        {
            var config = new SinkHttpConfiguration();
            ex(config);
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (string.IsNullOrWhiteSpace(config.ApiKey)) throw new ArgumentNullException(nameof(config.ApiKey));
            if (string.IsNullOrWhiteSpace(config.IngestUrl)) throw new ArgumentNullException(nameof(config.IngestUrl));

            config.IngestUrl += (config.IngestUrl.Contains("?") ? "&" : "?") + $"hostname={Dns.GetHostName().ToLower()}";
            if (config.CommaSeparatedTags != null) config.IngestUrl += $"&tags={WebUtility.UrlEncode(config.CommaSeparatedTags)}";

            var envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                          Environment.GetEnvironmentVariable("ASPNET_ENVIRONMENT");
            if (sinkConfiguration == null)
                throw new ArgumentNullException(nameof(sinkConfiguration));
            config.Period = (config.Period ?? TimeSpan.FromSeconds(2.0));
            ITextFormatter textFormatter = new LogdnaTextFormatter(config.AppName ?? "unknown", envName);
            IBatchFormatter batchFormatter = new LogdnaBatchFormatter();
            IHttpClient httpClient = new LogdnaHttpClient(config.ApiKey);
            return sinkConfiguration.DurableHttp(config.IngestUrl, config.BufferPathFormat, config.BufferFileSizeLimitBytes, config.RetainedBufferFileCountLimit, config.BatchPostingLimit, config.Period.Value, textFormatter, batchFormatter, config.RestrictedToMinimumLevel);


        }
        public static LoggerConfiguration HttpLogDna(this LoggerSinkConfiguration sinkConfiguration, string apiKey,
            string appName = null,
            string commaSeparatedTags = null,
            string requestUri = "https://logs.logdna.com/logs/ingest",
            int batchPostingLimit = 1000,
            int? queueLimit = null,
            TimeSpan? period = null,
            ITextFormatter textFormatter = null,
            IBatchFormatter batchFormatter = null,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            if (string.IsNullOrWhiteSpace(requestUri)) throw new ArgumentNullException(nameof(requestUri));

            requestUri += (requestUri.Contains("?") ? "&" : "?") + $"hostname={Dns.GetHostName().ToLower()}";
            if (commaSeparatedTags != null) requestUri += $"&tags={WebUtility.UrlEncode(commaSeparatedTags)}";

            var envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                          Environment.GetEnvironmentVariable("ASPNET_ENVIRONMENT");
            if (sinkConfiguration == null)
                throw new ArgumentNullException(nameof(sinkConfiguration));
            period = (period ?? TimeSpan.FromSeconds(2.0));
            textFormatter = textFormatter ?? new LogdnaTextFormatter(appName ?? "unknown", envName);
            batchFormatter = batchFormatter ?? new LogdnaBatchFormatter();
            IHttpClient httpClient = new LogdnaHttpClient(apiKey);
            return sinkConfiguration.Http(requestUri, batchPostingLimit, queueLimit.Value, period.Value,
              textFormatter, batchFormatter, restrictedToMinimumLevel, httpClient);

        }
        public static LoggerConfiguration DurableHttpLogDna(
            this LoggerSinkConfiguration sinkConfiguration,
            string apiKey,
            string appName = null,
            string commaSeparatedTags = null,
            string requestUri = "https://logs.logdna.com/logs/ingest",
            string bufferPathFormat = "Buffer-{Date}.json",
            long? bufferFileSizeLimitBytes = null,
            int? retainedBufferFileCountLimit = 31,
            int batchPostingLimit = 1000,
            TimeSpan? period = null,
            ITextFormatter textFormatter = null,
            IBatchFormatter batchFormatter = null,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose)
        {
            if (sinkConfiguration == null)
                throw new ArgumentNullException(nameof(sinkConfiguration));

            requestUri += (requestUri.Contains("?") ? "&" : "?") + $"hostname={Dns.GetHostName().ToLower()}";
            if (commaSeparatedTags != null) requestUri += $"&tags={WebUtility.UrlEncode(commaSeparatedTags)}";

            var envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                          Environment.GetEnvironmentVariable("ASPNET_ENVIRONMENT");
            period = new TimeSpan?(period ?? TimeSpan.FromSeconds(2.0));
            textFormatter = textFormatter ?? new LogdnaTextFormatter(appName ?? "unknown", envName);
            batchFormatter = batchFormatter ?? new LogdnaBatchFormatter();
            IHttpClient httpClient = new LogdnaHttpClient(apiKey);
            return sinkConfiguration.DurableHttp(requestUri, bufferPathFormat, bufferFileSizeLimitBytes, retainedBufferFileCountLimit, batchPostingLimit, period.Value, textFormatter, batchFormatter, restrictedToMinimumLevel);
        }
        [Obsolete]
        public static LoggerConfiguration LogDNA(
            this LoggerSinkConfiguration sinkConfiguration,
            string apiKey,
            string appName = null,
            string commaSeparatedTags = null,
            string ingestUrl = "https://logs.logdna.com/logs/ingest",
            int? batchPostingLimit = null,
            int? queueLimit = null,
            TimeSpan? period = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            if (string.IsNullOrWhiteSpace(ingestUrl)) throw new ArgumentNullException(nameof(ingestUrl));

            ingestUrl += (ingestUrl.Contains("?") ? "&" : "?") + $"hostname={Dns.GetHostName().ToLower()}";
            if (commaSeparatedTags != null) ingestUrl += $"&tags={WebUtility.UrlEncode(commaSeparatedTags)}";

            var envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                Environment.GetEnvironmentVariable("ASPNET_ENVIRONMENT");

            return sinkConfiguration.Http(ingestUrl,
                batchPostingLimit: batchPostingLimit ?? 50,
                queueLimit: queueLimit ?? 100,
                period: period ?? TimeSpan.FromSeconds(15),
                textFormatter: new LogdnaTextFormatter(appName ?? "unknown", envName),
                batchFormatter: new LogdnaBatchFormatter(),
                restrictedToMinimumLevel: restrictedToMinimumLevel,
                httpClient: new LogdnaHttpClient(apiKey));
        }
        [Obsolete]
        public static LoggerConfiguration SinkLogDNA(
            this LoggerSinkConfiguration sinkConfiguration,
            ISinkHttpConfiguration logDNAConfiguration)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (string.IsNullOrWhiteSpace(logDNAConfiguration.ApiKey)) throw new ArgumentNullException(nameof(logDNAConfiguration.ApiKey));
            if (string.IsNullOrWhiteSpace(logDNAConfiguration.IngestUrl)) throw new ArgumentNullException(nameof(logDNAConfiguration.IngestUrl));

            logDNAConfiguration.IngestUrl += (logDNAConfiguration.IngestUrl.Contains("?") ? "&" : "?") + $"hostname={Dns.GetHostName().ToLower()}";
            if (logDNAConfiguration.CommaSeparatedTags != null) logDNAConfiguration.IngestUrl += $"&tags={WebUtility.UrlEncode(logDNAConfiguration.CommaSeparatedTags)}";

            var envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                          Environment.GetEnvironmentVariable("ASPNET_ENVIRONMENT");
            var bufferMode = "Buffer-{" + (DurableBufferMode)logDNAConfiguration.BufferMode + "}.json";
            return sinkConfiguration.DurableHttp(logDNAConfiguration.IngestUrl,
                bufferPathFormat: bufferMode,
                batchPostingLimit: logDNAConfiguration.BatchPostingLimit,
                period: logDNAConfiguration.Period,
                textFormatter: new LogdnaTextFormatter(logDNAConfiguration.AppName ?? "unknown", envName),
                batchFormatter: new LogdnaBatchFormatter(),
                restrictedToMinimumLevel: logDNAConfiguration.RestrictedToMinimumLevel,
                httpClient: new LogdnaHttpClient(logDNAConfiguration.ApiKey));
        }
        [Obsolete]
        public static LoggerConfiguration SinkLogDNA(
            this LoggerSinkConfiguration sinkConfiguration,
            string apiKey,
            string appName = null,
            string commaSeparatedTags = null,
            string ingestUrl = "https://logs.logdna.com/logs/ingest",
            int? batchPostingLimit = null,
            TimeSpan? period = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            DurableBufferMode bufferMode = DurableBufferMode.Hour
          )
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            if (string.IsNullOrWhiteSpace(ingestUrl)) throw new ArgumentNullException(nameof(ingestUrl));

            ingestUrl += (ingestUrl.Contains("?") ? "&" : "?") + $"hostname={Dns.GetHostName().ToLower()}";
            if (commaSeparatedTags != null) ingestUrl += $"&tags={WebUtility.UrlEncode(commaSeparatedTags)}";

            var envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                          Environment.GetEnvironmentVariable("ASPNET_ENVIRONMENT");
            return sinkConfiguration.DurableHttp(ingestUrl,
                bufferPathFormat: "Buffer-{" + bufferMode + "}.json",
                batchPostingLimit: batchPostingLimit ?? 50,
                period: period ?? TimeSpan.FromSeconds(15),
                textFormatter: new LogdnaTextFormatter(appName ?? "unknown", envName),
                batchFormatter: new LogdnaBatchFormatter(),
                restrictedToMinimumLevel: restrictedToMinimumLevel,
                httpClient: new LogdnaHttpClient(apiKey));
        }
    }


}
