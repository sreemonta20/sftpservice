using Renci.SshNet;
using sftpservice.Core.Models.Common;
using sftpservice.Core.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sftpservice.Core.IServices
{
    /// <summary>
    /// It is an interface which helps to create loosely coupled solution and gives the privilege to work with sftp server.<br/>
    /// </summary>
    public interface IRemoteService
    {
        //public SftpClient ServiceInitialize(ApiSettings apsettings, string _serverDetails);
        //Task<FileDetailsResponse> DownloadFilesAsync(string localFilePath, string remoteFilePath);
        Task<FileDetailsResponse> DownloadFilesAsync();
    }
}
