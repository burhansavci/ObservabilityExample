namespace ObservabilityExample.Infrastructure.Logging
{
    public class FluentdOptions
    {
        public bool Enabled { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Tag { get; set; }
    }
}