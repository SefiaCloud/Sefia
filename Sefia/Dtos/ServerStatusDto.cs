namespace Sefia.Dtos
{
    public class ServerStatusDto
    {
        public ServerStatusDto(string status, string version, bool isInitialized)
        {
            Status = status;
            Version = version;
            IsInitialized = isInitialized;
        }

        public string Status { get; set; }
        public string Version { get; set; }
        public bool IsInitialized { get; set; }
    }
}
