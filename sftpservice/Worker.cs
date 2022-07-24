using Newtonsoft.Json;
using sftpservice.Core.IServices;
using sftpservice.Core.Models.Responses;
using sftpservice.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sftpservice
{
    public class Worker : BackgroundService
    {
        private readonly ILogService _logService;
        //private Timer? _timer = null;
        //private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IRemoteService _remoteService;
        //public Worker(ILogService logService, IRemoteService remoteService, IServiceScopeFactory serviceScopeFactory)
        //{
        //    _logService = logService;
        //    _remoteService = remoteService;
        //    _serviceScopeFactory = serviceScopeFactory;
        //}
        public Worker(ILogService logService, IRemoteService remoteService)
        {
            _logService = logService;
            _remoteService = remoteService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logService.LogInfo(String.Format("Worker running at: {0}", DateTimeOffset.Now));
                await DoWork(null);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task DoWork(object? state)
        {

            try
            {
                FileDetailsResponse response = await this._remoteService.DownloadFilesAsync();
                if (response != null && response.Status)
                {
                    if (response.State != null)
                    {
                        string numOfStateWisStrPart = (response.State > 1) ? "s are" : " is";
                        _logService.LogInfo(String.Format("{0} File{1} downloaded. Attempt details are given below: \n{2}", response.State, numOfStateWisStrPart, JsonConvert.SerializeObject(new { response }, Formatting.Indented)));
                    }
                    else
                    {
                        _logService.LogInfo(String.Format("Attempt details are given below: \n{0}", JsonConvert.SerializeObject(new { response }, Formatting.Indented)));
                    }
                }
                else
                {
                    _logService.LogError(String.Format("Attempt details are given below: \n{0}", JsonConvert.SerializeObject(new { response }, Formatting.Indented)));
                }
            }
            catch (Exception Ex)
            {
                _logService.LogError($"{String.Format(ConstantSupplier.BACKGROUND_WORK_ERROR_MSG, nameof(DoWork), Ex.Message)}");
            }
        }
    }
}
