using System.Net;
using System.Text.Json;
using P2P.Utils;

namespace P2P.Monitoring;

/// <summary>
/// Simple HTTP server that serves the monitoring dashboard and API endpoints.
/// </summary>
public class MonitoringServer
{
    private HttpListener _listener;
    private bool _isRunning;
    private CancellationTokenSource _tokenSource;

    public MonitoringServer(string url = "http://localhost:8080/")
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add(url);
        _tokenSource = new CancellationTokenSource();
    }

    public async Task StartAsync()
    {
        _listener.Start();
        _isRunning = true;
        Logger.Info("Monitoring server started on http://localhost:8080/");

        while (_isRunning && !_tokenSource.IsCancellationRequested)
        {
            try
            {
                HttpListenerContext context = await _listener.GetContextAsync();
                _ = HandleRequestAsync(context);
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (Exception ex)
            {
                Logger.Error($"Monitoring server error: {ex.Message}");
            }
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context)
    {
        try
        {
            string path = context.Request.Url.AbsolutePath;
            string method = context.Request.HttpMethod;

            if (path == "/" || path == "/index.html")
            {
                ServeHtmlDashboard(context);
            }
            else if (path == "/api/stats" && method == "GET")
            {
                ServeStats(context);
            }
            else if (path == "/api/shutdown" && method == "POST")
            {
                ShutdownApplication(context);
            }
            else
            {
                context.Response.StatusCode = 404;
                context.Response.Close();
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Error handling request: {ex.Message}");
            context.Response.StatusCode = 500;
            context.Response.Close();
        }
    }

    private void ServeHtmlDashboard(HttpListenerContext context)
    {
        try
        {
            string html = File.ReadAllText("monitor.html");
            
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.StatusCode = 200;
            
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(html);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.Close();
        }
        catch (Exception ex)
        {
            Logger.Error($"Error reading monitor.html: {ex.Message}");
            context.Response.StatusCode = 500;
            context.Response.Close();
        }
    }

    private void ServeStats(HttpListenerContext context)
    {
        var stats = new
        {
            totalCommands = MonitoringState.TotalCommands,
            totalErrors = MonitoringState.TotalErrors,
            lastCommand = MonitoringState.LastCommand,
            lastError = MonitoringState.LastError,
            uptime = MonitoringState.Uptime.ToString(@"hh\:mm\:ss"),
            isRunning = true
        };

        string json = JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 200;

        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(json);
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.Close();
    }

    private void ShutdownApplication(HttpListenerContext context)
    {
        context.Response.StatusCode = 200;
        context.Response.ContentType = "text/plain";
        
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Shutting down.");
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.Close();

        Logger.Info("Shutdown request received from monitoring dashboard");
        _isRunning = false;
        _tokenSource.Cancel();
        
        Task.Delay(500).ContinueWith(_ => Environment.Exit(0));
    }
    
}