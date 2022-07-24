﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sftpservice.Core.IServices
{
    /// <summary>
    /// It define the various logs. 
    /// </summary>
    /// <returns>interface</returns>
    public interface ILogService
    {
        /// <summary>
        /// LogDebug
        /// </summary>
        /// <param name="message"></param>
        void LogDebug(string message);
        /// <summary>
        /// LogInfo
        /// </summary>
        /// <param name="message"></param>
        void LogInfo(string message);
        /// <summary>
        /// LogError
        /// </summary>
        /// <param name="message"></param>
        void LogError(string message);
        /// <summary>
        /// LogCritical
        /// </summary>
        /// <param name="message"></param>
        void LogCritical(string message);
        /// <summary>
        /// LogWarning
        /// </summary>
        /// <param name="message"></param>
        void LogWarning(string message);
    }
}
