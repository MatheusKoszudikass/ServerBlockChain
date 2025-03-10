using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using ServerBlockChain.Entities.Enum;
using ServerBlockChain.Util;

namespace ServerBlockChain.Entities
{
    public class LogEntry
    {
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public string Ip { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public string ProcessId { get; set; } = string.Empty;
        public string Architecture { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserDomain { get; set; } = string.Empty;
        public string ThreadId { get; set; } = string.Empty;
        public string Exception { get; set; } = string.Empty;

        [JsonConverter(typeof(ObjectToStringConverter))]
        public object? Source { get; set; }
        public string StackTrace { get; set; } = string.Empty;

        [JsonIgnore] private IntPtr Handle { get; set; }
    }
}