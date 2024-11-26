namespace Sefia.Common;

public class AppSettings
{
    public bool IsInitialized { get; set; } = false;

    public string ApplicationDomain { get; set; } = string.Empty;
    public string ServingDomain { get; set; } = string.Empty;
    public string WebRoot { get; set; } = string.Empty;
}
