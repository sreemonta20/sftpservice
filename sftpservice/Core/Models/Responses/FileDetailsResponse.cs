using Newtonsoft.Json;
using sftpservice.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sftpservice.Core.Models.Responses
{
    public class FileDetailsResponse
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<FileDetails>? ListData { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FileDetails? Data { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? State { get; set; }

        public FileDetailsResponse(bool status, int statuscode, string message, List<FileDetails>? listData)
        {
            this.Status = status;
            this.StatusCode = statuscode;
            this.Message = message;
            this.ListData = listData;
        }

        public FileDetailsResponse(bool status, int statuscode, string message, FileDetails? data)
        {
            this.Status = status;
            this.StatusCode = statuscode;
            this.Message = message;
            this.Data = data;
        }

        public FileDetailsResponse(bool status, int statuscode, string message, int? state)
        {
            this.Status = status;
            this.StatusCode = statuscode;
            this.Message = message;
            this.State = state;
        }
        public FileDetailsResponse(bool status, int statuscode, string message)
        {
            this.Status = status;
            this.StatusCode = statuscode;
            this.Message = message;
        }
        public FileDetailsResponse(string message)
        {
            this.Message = message;
        }
    }
}
