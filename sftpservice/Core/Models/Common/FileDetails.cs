using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sftpservice.Core.Models.Common
{
    /// <summary>
    /// This class plays the important role throughout the application. With the help of this, user can do various db operations,<para />
    /// download files, check files whether new or old to download in the local path etc.
    /// </summary>
    public class FileDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        //public DateTime FileCreationTime { get; set; } = DateTime.UtcNow;
        public DateTime FileCreationTime { get; set; }
        public string SourceFilePath { get; set; } = string.Empty;
        public string DestinationFilePath { get; set; } = string.Empty;
    }
}
