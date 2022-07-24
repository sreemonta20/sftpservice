using sftpservice.Core.Models.Common;
using sftpservice.Core.Models.Requests;
using sftpservice.Core.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sftpservice.Core.IServices
{
    /// <summary>
    /// It is an interface which helps to create loosely coupled solution and gives the privilege to work with database.<br/>
    /// </summary>
    public interface IDataService
    {
        Task<FileDetailsResponse> GetAllFilesAsync();
        Task<FileDetailsResponse> AddFilesAsync(List<FileDetails> entities);
    }
}
