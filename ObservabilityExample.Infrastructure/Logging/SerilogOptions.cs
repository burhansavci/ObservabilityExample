using System.Collections.Generic;

namespace ObservabilityExample.Infrastructure.Logging
{
    public class SerilogOptions
    {
        public bool ConsoleEnabled { get; set; }
        public string Level { get; set; }
        public IEnumerable<string> ExcludePaths { get; set; }
    }
}