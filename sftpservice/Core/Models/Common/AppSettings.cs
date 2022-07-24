using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sftpservice.Core.Models.Common
{
    public class AppSettings
    {
        public AppSettings(RemoteServerConfiguration? remoteServerConfiguration, ConnectionStrings? connectionStrings)
        {
            RemoteServerConfiguration = remoteServerConfiguration;
            ConnectionStrings = connectionStrings;
        }

        public RemoteServerConfiguration? RemoteServerConfiguration { get; set; } 
        public ConnectionStrings? ConnectionStrings { get; set; }

    }
}
