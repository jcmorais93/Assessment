namespace Storage.Services;

public class Repository : IRepository
{
    private readonly string _filePath;

    public Repository(IConfiguration configuration)
    {
        _filePath = configuration["LogFilePath"] ?? "/tmp/visits.log";
        EnsureDirectoryExists();
        EnsureFileExists();
    }
    
    public async void LogData(Tracker? tracker)
    {
        if (tracker?.IpAddress == null)
        {
            return;
        }

        var referrer = !string.IsNullOrEmpty(tracker.Referer) ? tracker.Referer : "null";
        var userAgent = !string.IsNullOrEmpty(tracker.UserAgent) ? tracker.UserAgent : "null";
        
        var log = $"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}|{referrer}|{userAgent}|{tracker.IpAddress}";
        
        try
        {
            await File.AppendAllTextAsync(_filePath, $"{log}\n");
        }
        catch
        {
            // ignored
        }
    }

    private void EnsureDirectoryExists()
    {
        var directoryPath = Path.GetDirectoryName(_filePath);
        if (Directory.Exists(directoryPath)) return;
        if (directoryPath != null) Directory.CreateDirectory(directoryPath);
    }

    private void EnsureFileExists()
    {
        if (File.Exists(_filePath)) return;
        using (File.Create(_filePath)) { }
    }
}