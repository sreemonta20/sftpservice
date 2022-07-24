using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Renci.SshNet;
using Renci.SshNet.Async;
using sftpservice.Core.IServices;
using sftpservice.Core.Models.Common;
using sftpservice.Core.Models.Responses;
using sftpservice.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectionInfo = Renci.SshNet.ConnectionInfo;

namespace sftpservice.Core.Services
{
    /// <summary>
    /// This class which implements <see  cref="IRemoteService"/>.
    /// </summary>
    public class RemoteService : IRemoteService
    {
        /// <summary>
        /// Declaration & Initialization
        /// </summary>
        private readonly ILogService _logService;
        private string _serverDetails = "";
        private readonly SftpClient _sftpClient;
        private readonly IDataService _dataService;
        private readonly IConfiguration _appSettings;
        private readonly AppSettings _settings;
        //private AppSettings _settings;
        /// <summary>
        /// Constructor initialization
        /// </summary>
        public RemoteService(ILogService logService, IDataService dataService, IConfiguration appSettings)
        {
            this._logService = logService;
            _dataService = dataService;
            _appSettings = appSettings;
            _settings = new AppSettings(_appSettings.GetSection(nameof(RemoteServerConfiguration)).Get<RemoteServerConfiguration>(),
                _appSettings.GetSection(nameof(ConnectionStrings)).Get<ConnectionStrings>());
            _sftpClient = ServiceInitialize(_settings.RemoteServerConfiguration);
        }

        /// <summary>
        /// This method downloads files from the given sftp server into local path and sync with database. Every minute, it check whether sftp client is
        /// connected or not. It list the files from sftp server and database. It make a distinct file based on file creation time and file name
        /// by comparing files on the server and on the database. After counting new file, it helps to download into local path and save the details into 
        /// the postgrsql database.
        /// </summary>
        /// <returns></returns>
        public async Task<FileDetailsResponse> DownloadFilesAsync()
        {
            string globalFullServerPath = string.Empty;
            List<string> serverFilesPathList = new();
            List<string> dbFilesPathList = new();
            List<string> newFilesPathList = new();

            
            List<ServerFileInfo> creationTimeWiseDbFileList = new();
            List<ServerFileInfo> newCreationTimeWiseFileInfoList = new();

            List<ServerFileInfo> serverFileInfoList = new();
            List<ServerFileInfo> dbFileInfoList = new();
            List<ServerFileInfo>? newList = new();
            try
            {
                // Check sftpclient is connected or not. If not then connect
                if (!_sftpClient.IsConnected)
                {
                    _sftpClient.Connect();
                }

                // Read local and server path and create server path directory if not exist (not necessary but for consistency server directory
                // is being created so that control can list out all the files from the server path. In case of new created server path, obviously
                // server path will have o files.
                
                var localPath = @"'" + _settings.RemoteServerConfiguration.LocalPathDirectory + "'";
                var serverPath = @"" + _settings.RemoteServerConfiguration.ServerPathDirectory;

                if (!_sftpClient.Exists(serverPath))
                {
                    _sftpClient.CreateDirectory(serverPath);
                }
                if (_sftpClient.Exists(serverPath))
                {
                    globalFullServerPath = Path.GetFullPath(Path.Join(@"" + _settings.RemoteServerConfiguration.ServerPathBaseDirectory, serverPath));
                    serverFilesPathList = ExtensionMethods.GetFiles(globalFullServerPath);
                }

                // Intial Check whether the server path has files or not (new or old doesn't matter)
                if ((serverFilesPathList != null) && (serverFilesPathList.Count > 0))
                {
                    // Getting sftp server file list (of ServerInfo class object)
                    serverFileInfoList = await getFileInfo(globalFullServerPath);

                    if(serverFileInfoList != null)
                    {
                        // Getting db stored file list (of FileDetailsResponse object)
                        var dbFiles = await _dataService.GetAllFilesAsync();
                        if ((dbFiles.ListData != null) && (dbFiles.ListData.Count > 0))
                        {
                            // Creating new list (of ServerInfo class object) out of db stored file list (of FileDetailsResponse object)
                            foreach (var item in dbFiles.ListData)
                            {
                                dbFileInfoList.Add(new ServerFileInfo { FileName = item.FileName, FileCreationTime = item.FileCreationTime });
                            }

                            // Getting new files by comparing sftp server files and db files.
                            if((dbFileInfoList != null) && (!dbFileInfoList.Equals(serverFileInfoList)) 
                                && (dbFileInfoList.Count < serverFileInfoList.Count)) // In case of new file (s).
                            {
                                //newList = serverFileInfoList.Except(dbFileInfoList).ToList();
                                //var list = serverFileInfoList.Where(item => !dbFileInfoList.Any(e => item.FileCreationTime == e.FileCreationTime));
                                //newList = list.ToList();
                                newList = serverFileInfoList.Where(p => dbFileInfoList.All(l => p.FileName != l.FileName && p.FileCreationTime != l.FileCreationTime)).ToList();
                            }
                            //else if((dbFileInfoList != null) && (dbFileInfoList.Equals(serverFileInfoList))) // In case of no new file (s).
                            //{
                            //    newList = null;
                            //}
                            else // In case of no new file (s).
                            {
                                //newList = null;
                                return new FileDetailsResponse(true, StatusCodes.Status404NotFound, ConstantSupplier.NO_NEW_FILES);
                            }
                        }
                        else
                        {
                            newList = serverFileInfoList;// In case of first time downloading of new file (s).
                        }

                        //if((newList != null) && (newList.Count.Equals(0)))
                        //{
                        //    return new FileDetailsResponse(true, StatusCodes.Status404NotFound, ConstantSupplier.NO_NEW_FILES);
                        //}
                        //if ((newList == null) && (newList.Count.Equals(0)))
                        //{
                        //    return new FileDetailsResponse(true, StatusCodes.Status404NotFound, ConstantSupplier.NO_NEW_FILES);
                        //}

                        //Download new file(s)
                        var isDownload = await DownloadServerFilesAsync(localPath, serverPath, globalFullServerPath, newList);
                        if (!isDownload)
                        {
                            return new FileDetailsResponse(false, StatusCodes.Status400BadRequest, ConstantSupplier.DOWNLOAD_FAILED_MSG);
                        }

                        // Creating data to save into postgresql db.
                        List<FileDetails>? entities = new();
                        foreach (var item in newList)
                        {
                            //var serverFile = serverFileInfoList.Where(x => x.FileName == Path.GetFileName(item.FileName)).FirstOrDefault();
                            var sourcePath = Path.GetFullPath(Path.Join(globalFullServerPath, Path.GetFileName(item.FileName)));
                            var destintionPath = Path.GetFullPath(Path.Join(localPath, Path.GetFileName(item.FileName)));

                            //entities.Add(new FileDetails { Id = Guid.NewGuid(), FileName = Path.GetFileName(item.FileName), FileCreationTime = serverFile.FileCreationTime, SourceFilePath = sourcePath, DestinationFilePath = destintionPath });
                            entities.Add(new FileDetails { Id = Guid.NewGuid(), FileName = Path.GetFileName(item.FileName), FileCreationTime = item.FileCreationTime, SourceFilePath = sourcePath, DestinationFilePath = destintionPath });
                        }

                        var result = await _dataService.AddFilesAsync(entities);


                        if (!result.Status)
                        {
                            _logService.LogError($"{JsonConvert.SerializeObject(new { result }, Formatting.Indented)}");
                            return result;
                        }
                        return result;
                    }
                    else
                    {
                        return new FileDetailsResponse(false, StatusCodes.Status404NotFound, ConstantSupplier.NO_FILES_FOUND_SERVER_PATH);
                    }

                }
                else
                {
                    return new FileDetailsResponse(false, StatusCodes.Status404NotFound, ConstantSupplier.NO_FILES_FOUND_SERVER_PATH);
                }

            }
            catch (Exception Ex)
            {
                _logService.LogError($"{String.Format(ConstantSupplier.CONTROLLER_ACTION_ERROR_MSG, nameof(DownloadFilesAsync), Ex.Message)}");
                return new FileDetailsResponse(false, StatusCodes.Status500InternalServerError, Ex.Message);
            }
        }

        /// <summary>
        /// It reads and list all the files according to <see  cref="ServerFileInfo"/> object within a path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private async Task<List<ServerFileInfo>> getFileInfo(string path)
        {
            List<ServerFileInfo> serverFileInfoList = new();
            DirectoryInfo info = new DirectoryInfo(path);
            FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
            foreach (FileInfo file in files)
            {
                await Task.Delay(0);
                serverFileInfoList.Add(new ServerFileInfo { FileName = file.Name, FileCreationTime = file.CreationTime });
            }
            return serverFileInfoList;
        }


        /// <summary>
        /// It only download new files from the sftp server into local path.
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <param name="remoteFilePath"></param>
        /// <param name="newList"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<bool> DownloadServerFilesAsync(string localFilePath, string remoteFilePath, string remoteFullPath, List<ServerFileInfo>? newList)
        {
            bool isdownloaded = false;
            var files = _sftpClient.ListDirectory(remoteFilePath);
            try
            {
                var creationTimeWiseFileList = await getFileInfo(remoteFullPath);
                if (newList != null)
                {
                    foreach (var file in files)
                    {
                        if ((!file.Name.StartsWith(".")) && (!file.Name.EndsWith(".")))
                        {
                            var singleFile = creationTimeWiseFileList.Where(x => x.FileName == file.Name).FirstOrDefault();
                            var isExit = newList.Where(x => x.FileName == singleFile.FileName && x.FileCreationTime == singleFile.FileCreationTime).Any();
                            if (isExit)
                            {
                                var combinedLocalPath = Path.Join(localFilePath.Trim('\''), file.Name);
                                var combinedServerPath = Path.Join(remoteFilePath, file.Name);
                                using (Stream fileStream = File.Create(combinedLocalPath))
                                {
                                    await _sftpClient.DownloadAsync(combinedServerPath, fileStream);
                                }
                                isdownloaded = true;
                            }
                            
                        }

                    }
                }
            }
            catch (Exception Ex)
            {
                throw new Exception($"{ConstantSupplier.EXCEPTION_MSG}{Ex.Message}{ConstantSupplier.NEW_LINE}{ConstantSupplier.EXCEPTION_INNER_MSG}{Ex.InnerException}.");
            }
            return isdownloaded;
        }
        
        /// <summary>
        /// Initializing sftpclient.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private SftpClient ServiceInitialize(RemoteServerConfiguration config)
        {
            try
            {
                
                var serverDetails = ExtensionMethods.ServerDetails(config.Host, config.Port.ToString(), config.UserName, config.Type);
                this._serverDetails = serverDetails;
                var connectionInfo = new ConnectionInfo(config.Host, config.Port, config.UserName, new PasswordAuthenticationMethod(config.UserName, config.Password));
                return new SftpClient(connectionInfo);
            }
            catch (Exception Ex)
            {
                throw new Exception($"{ConstantSupplier.EXCEPTION_MSG}{Ex.Message}{ConstantSupplier.NEW_LINE}{ConstantSupplier.EXCEPTION_INNER_MSG}{Ex.InnerException}.");
            }
        }

    }
}
