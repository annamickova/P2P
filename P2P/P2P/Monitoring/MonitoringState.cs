namespace P2P.Monitoring;

/// <summary>
/// Simple monitoring state - tracks basic statistics for the dashboard.
/// </summary>
public static class MonitoringState
{
    private static readonly object _lock = new();
    
    public static DateTime StartTime { get; } = DateTime.Now;
    
    private static int _totalCommands = 0;
    private static int _totalErrors = 0;
    private static string _lastCommand = "-";
    private static string _lastError = "-";

    public static void IncrementCommands()
    {
        lock (_lock)
        {
            _totalCommands++;
        }
    }

    public static void IncrementErrors(string errorMessage)
    {
        lock (_lock)
        {
            _totalErrors++;
            _lastError = errorMessage;
        }
    }

    public static void SetLastCommand(string commandName)
    {
        lock (_lock)
        {
            _lastCommand = commandName;
        }
    }

    public static int TotalCommands
    {
        get { lock (_lock) return _totalCommands; }
    }

    public static int TotalErrors
    {
        get { lock (_lock) return _totalErrors; }
    }

    public static string LastCommand
    {
        get { lock (_lock) return _lastCommand; }
    }

    public static string LastError
    {
        get { lock (_lock) return _lastError; }
    }

    public static TimeSpan Uptime => DateTime.Now - StartTime;
}