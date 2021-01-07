using System;
using System.Net;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.LogDNA;

namespace Serilog
{
    public static class LogdnaExtensions
    {
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
            var bufferMode = "Buffer-{"+ (DurableBufferMode)logDNAConfiguration.BufferMode+ "}.json";
            return sinkConfiguration.DurableHttp(logDNAConfiguration.IngestUrl,
                bufferPathFormat: bufferMode,
                batchPostingLimit: logDNAConfiguration.BatchPostingLimit,
                period: logDNAConfiguration.Period,
                textFormatter: new LogdnaTextFormatter(logDNAConfiguration.AppName ?? "unknown", envName),
                batchFormatter: new LogdnaBatchFormatter(),
                restrictedToMinimumLevel: logDNAConfiguration.RestrictedToMinimumLevel,
                httpClient: new LogdnaHttpClient(logDNAConfiguration.ApiKey));
        }
        public static LoggerConfiguration SinkLogDNA(
            this LoggerSinkConfiguration sinkConfiguration,
            string apiKey,
            string appName = null,
            string commaSeparatedTags = null,
            string ingestUrl = "https://logs.logdna.com/logs/ingest",
            int? batchPostingLimit = null,
            TimeSpan? period = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            DurableBufferMode bufferMode= DurableBufferMode.Hour
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
                restrictedToMinimumLevel:restrictedToMinimumLevel,
                httpClient: new LogdnaHttpClient(apiKey));
        }
    }

  
}
