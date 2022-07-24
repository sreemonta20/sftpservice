using Microsoft.AspNetCore.Http;
using sftpservice.Core.IRepositories;
using sftpservice.Core.IServices;
using sftpservice.Core.Models.Common;
using sftpservice.Core.Models.Requests;
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
    /// It is generic class which implements all the methods defined in the <see  cref="ISftpDataService"/>
    /// </summary>
    public class DataService : IDataService
    {
        private readonly ILogService _logService;
        private readonly IGenericRepository<FileDetails> _dataRepo;

        public DataService(ILogService logService, IGenericRepository<FileDetails> dataRepo)
        {
            _logService = logService;
            _dataRepo = dataRepo;
        }
        public async Task<FileDetailsResponse> AddFilesAsync(List<FileDetails> entities)
        {
            try
            {
               
                var state = await this._dataRepo.AddRangeAsync(entities);
                if (state <= 0)
                {
                    return new FileDetailsResponse(false, StatusCodes.Status400BadRequest, ConstantSupplier.SFTP_DATA_SAVE_FAILED_MSG);
                }
                return new FileDetailsResponse(true, StatusCodes.Status200OK, ConstantSupplier.SFTP_DATA_SAVE_SUCCESS_MSG, state);
            }
            catch (Exception Ex)
            {
                return new FileDetailsResponse(false, StatusCodes.Status500InternalServerError, $"{ConstantSupplier.EXCEPTION_MSG}{Ex.Message}{ConstantSupplier.NEW_LINE}{ConstantSupplier.EXCEPTION_INNER_MSG}{Ex.InnerException}.");
            }
        }

        public async Task<FileDetailsResponse> GetAllFilesAsync()
        {
            try
            {
                var listdata = await this._dataRepo.GetAllAsync();
                //if ((listdata == null) && (listdata.Count.Equals(0)))
                if (listdata == null)
                {
                    return new FileDetailsResponse(false, StatusCodes.Status404NotFound, ConstantSupplier.LIST_SFTP_DATA_LIST_RETRIVE_FAILED_MSG);
                }
                return new FileDetailsResponse(true, StatusCodes.Status200OK, ConstantSupplier.LIST_SFTP_DATA_LIST_RETRIVE_SUCCESS_MSG, listdata.ToList());
            }
            catch (Exception Ex)
            {
                return new FileDetailsResponse(false, StatusCodes.Status500InternalServerError, $"{ConstantSupplier.EXCEPTION_MSG}{Ex.Message}{ConstantSupplier.NEW_LINE}{ConstantSupplier.EXCEPTION_INNER_MSG}{Ex.InnerException}.");
            }
        }
    }
}
