# Serilog.Sinks.LogDNA.Mod
github repo@
https://github.com/yuessir/Serilog.Sinks.LogDNA.Mod.git

Platforms - .NET 4.5, netstandard2.0, .net core 3 for Serilog Sinks.

Based off of the work done by iojancode @ https://github.com/iojancode/Serilog.Sinks.LogDNA
```
Install-Package Serilog.Sinks.LogDNA.Mod
```

example
```c#
var log = new LoggerConfiguration()
    .WriteTo.SinkLogDNA(SetupSinkHttpConfiguration())
    .CreateLogger();

 static SinkHttpConfiguration SetupSinkHttpConfiguration()
        {
            return new SinkHttpConfiguration(apikey:"7b1723643523339a3872f2e56c52d741")
            {
                AppName = "appName",
                CommaSeparatedTags = $"App-appName,Host-{Environment.MachineName}"
            };
        }
```

or

```c#
var log = new LoggerConfiguration()
    .WriteToLogDna(x => x.ApiKey = "7b1723643523339a3872f2e56c52d741")
    .CreateLogger();

```
