﻿using System.IO;
using System.Runtime.InteropServices;
using NPLEditor;
using NPLEditor.Data;
using Serilog;

// Ensure DPI-Awareness isn't lost for the dotnet tool.
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    [DllImport("user32.dll")]
    static extern bool SetProcessDPIAware();

    SetProcessDPIAware();
}

// Create the logs directory.
Directory.CreateDirectory(NPLEditor.AppSettings.LogsPath);

// The general log file should always regenerate.
if (File.Exists(NPLEditor.AppSettings.AllLogPath)) File.Delete(NPLEditor.AppSettings.AllLogPath);

// Create the serilog logger.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.File(AppSettings.AllLogPath,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose,
        rollOnFileSizeLimit: true)
    .WriteTo.File(AppSettings.ImportantLogPath,
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning,
        rollOnFileSizeLimit: true)
    .WriteTo.Debug(
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
    .WriteTo.Console(
    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
    .WriteTo.NPLEditorSink(
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose)
    .CreateLogger();

// Log that main initialize begins.
Log.Information("");
Log.Information($"{FontAwesome.Flag} INITIALIZE");
Log.Information("");

// Main initialize.
using var game = new Main();
game.Run();