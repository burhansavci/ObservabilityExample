using System.Collections.Generic;

namespace ObservabilityExample.Services.Products
{
    public class JaegerOptions
    {
        public string ServiceName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public int MaxPayloadSizeInBytes { get; set; } = 4096;
        public string Sampler { get; set; }
        public double MaxTracesPerSecond { get; set; } = 5;
        public double SamplingRate { get; set; } = 0.2;
        public IEnumerable<string> ExcludePaths { get; set; }
    }
}