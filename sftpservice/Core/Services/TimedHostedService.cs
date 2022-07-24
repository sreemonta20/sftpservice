using Newtonsoft.Json;
using sftpservice.Core.IServices;
using sftpservice.Core.Models.Responses;
using sftpservice.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sftpservice.Core.Services
{
    /// <summary>
    /// this class implements the method defined in the <see  cref="IHostedService"/> and in the <see  cref="IDisposable"/>, 
    /// which executes the main work in the background.
    /// </summary>
    public class TimedHostedService : IHostedService, IDisposable
    {
        /// <summary>
        /// Declaration & Initialization
        /// </summary>
        private readonly ILogService _logService;
        private Timer? _timer = null;
        private readonly IRemoteService _remoteService;

        /// <summary>
        /// Constructor initialization
        /// </summary>
        /// <param name="logService"></param>
        /// <param name="remoteService"></param>
        public TimedHostedService(ILogService logService, IRemoteService remoteService)
        {
            _logService = logService;
            _remoteService = remoteService;
        }

        /// <summary>
        /// StartAsync
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logService.LogInfo(String.Format("Worker running at: {0}", DateTimeOffset.Now));

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        /// <summary>
        /// DoWork executes the process in background in every 1 minute.
        /// </summary>
        /// <param name="state"></param>
        private async void DoWork(object? state)
        {
            
            try
            {
                FileDetailsResponse response =  await this._remoteService.DownloadFilesAsync();
                if(response != null && response.Status)
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

        /// <summary>
        /// StopAsync
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logService.LogInfo(String.Format("Worker is stopping at: {0}", DateTimeOffset.Now));

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
