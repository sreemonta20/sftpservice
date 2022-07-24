using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sftpservice.Core.Models.Requests
{
    /// <summary>
    /// This class carries the server, services, and other responses.
    /// </summary>
    public class FileDetailsRequest
    {
        public string FileName { get; set; } = string.Empty;
        public DateTime FileCreationTime { get; set; } = DateTime.UtcNow;
        public string SourceFilePath { get; set; } = string.Empty;
        public string DestinationFilePath { get; set; } = string.Empty;
    }
}
