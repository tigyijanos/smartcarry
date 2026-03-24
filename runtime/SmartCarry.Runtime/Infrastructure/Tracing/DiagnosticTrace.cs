using System.Text;
using BepInEx;
using SmartCarry.Runtime.Configuration;
using System.Collections.Concurrent;

namespace SmartCarry.Runtime;

internal static class DiagnosticTrace
{
    private static DiagnosticLogLevel currentLevel = DiagnosticLogLevel.Off;
    private static string? traceFilePath;
    private static readonly ConcurrentDictionary<string, int> SampleCounts = new();

    public static DiagnosticLogLevel CurrentLevel => currentLevel;

    public static void Configure(DiagnosticLogLevel level)
    {
        currentLevel = level;
        if (currentLevel == DiagnosticLogLevel.Off)
        {
            traceFilePath = null;
        }
    }

    public static void StartSession()
    {
        if (currentLevel == DiagnosticLogLevel.Off)
        {
            return;
        }

        traceFilePath = Path.Combine(Paths.BepInExRootPath, "SmartCarry.trace.log");
        try
        {
            File.WriteAllText(traceFilePath, string.Empty, Encoding.UTF8);
            SampleCounts.Clear();
        }
        catch
        {
        }

        Info("session", $"=== Session started {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} ===");
    }

    public static void Info(string category, string message)
    {
        if (currentLevel < DiagnosticLogLevel.Info)
        {
            return;
        }

        WriteLine(category, message, DiagnosticLogLevel.Info);
    }

    public static void Trace(string category, string message)
    {
        if (currentLevel < DiagnosticLogLevel.Trace)
        {
            return;
        }

        WriteLine(category, message, DiagnosticLogLevel.Trace);
    }

    public static void Error(string category, string message)
    {
        if (currentLevel < DiagnosticLogLevel.Error)
        {
            return;
        }

        WriteLine(category, message, DiagnosticLogLevel.Error);
    }

    public static void InfoSample(string category, string key, string message, int initialLimit = 10, int every = 50)
    {
        if (currentLevel < DiagnosticLogLevel.Info)
        {
            return;
        }

        var count = SampleCounts.AddOrUpdate(key, 1, static (_, existing) => existing + 1);
        if (count <= initialLimit || (every > 0 && count % every == 0))
        {
            WriteLine(category, $"#{count} {message}", DiagnosticLogLevel.Info);
        }
    }

    private static void WriteLine(string category, string message, DiagnosticLogLevel level)
    {
        var line = $"[{DateTime.Now:HH:mm:ss.fff}] [{category}] {message}";

        try
        {
            if (level == DiagnosticLogLevel.Error)
            {
                SmartCarryPlugin.Logger?.LogError(line);
            }
            else
            {
                SmartCarryPlugin.Logger?.LogInfo(line);
            }
        }
        catch
        {
        }

        if (string.IsNullOrWhiteSpace(traceFilePath))
        {
            return;
        }

        try
        {
            File.AppendAllText(traceFilePath, line + Environment.NewLine, Encoding.UTF8);
        }
        catch
        {
        }
    }
}
