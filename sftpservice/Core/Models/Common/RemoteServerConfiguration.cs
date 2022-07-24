using sftpservice.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sftpservice.Core.Models.Common
{
    public class RemoteServerConfiguration
    {
        public string Type { get; set; } = ConstantSupplier.REMOTE_TYPE_SFTP;
        public string HostName { get; set; } = ConstantSupplier.REMOTE_HOST_NAME;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 2222;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ServerPathBaseDirectory { get; set; } = string.Empty;
        public string ServerPathDirectory { get; set; } = string.Empty;
        public string LocalPathDirectory { get; set; } = string.Empty;
        
    }
}
