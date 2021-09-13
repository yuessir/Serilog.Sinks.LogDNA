using System;
using Serilog.Events;

namespace Serilog.Sinks.LogDNA
{

    public enum DurableBufferMode
    {
        Date,
        HalfHour,
        Hour
    }
    public interface ISinkHttpConfiguration
    {
       
        string ApiKey { get; set; }
        string AppName { get; set; }
        string CommaSeparatedTags { get; set; }
        string IngestUrl { get; set; }
        int BatchPostingLimit { get; set; }

        int? QueueLimit { get; set; }
        TimeSpan? Period { get; set; }
        LogEventLevel RestrictedToMinimumLevel { get; set; }
        DurableBufferMode BufferMode { get; set; }
        string BufferPathFormat { get; set; } 
        long? BufferFileSizeLimitBytes { get; set; }
        int? RetainedBufferFileCountLimit { get; set; }
    }
    public class SinkHttpConfiguration : ISinkHttpConfiguration
    {
        public SinkHttpConfiguration()
        {
        }
        public SinkHttpConfiguration(string apiKey)
        {
            ApiKey = apiKey;
        }
        public string ApiKey { get; set; }
        public string AppName { get; set; }
        public string CommaSeparatedTags { get; set; }
        public string IngestUrl { get; set; } = "https://logs.logdna.com/logs/ingest";
        public int BatchPostingLimit { get; set; } = 50;
        public int? QueueLimit { get; set; } = 100;
        public TimeSpan? Period { get; set; } = TimeSpan.FromSeconds(15);
        public LogEventLevel RestrictedToMinimumLevel { get; set; } = LevelAlias.Minimum;
        public DurableBufferMode BufferMode { get; set; } = DurableBufferMode.Hour;
        public string BufferPathFormat { get; set; } = "Buffer-{Date}.json";
        public long? BufferFileSizeLimitBytes { get; set; }
        public int? RetainedBufferFileCountLimit { get; set; } = 31;
    }
}