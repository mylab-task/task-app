using System;
using MyLab.StatusProvider;

namespace MyLab.TaskApp
{
    /// <summary>
    /// Task application specific status
    /// </summary>
    public class TaskAppStatus : ICloneable
    {
        /// <summary>
        /// Last time when an application logic was started
        /// </summary>
        public DateTime? LastTimeStart { get; set; }
        /// <summary>
        /// Duration of application task logic performing
        /// </summary>
        public TimeSpan? LastTimeDuration { get; set; }
        /// <summary>
        /// Error description which occured at last logic performing
        /// </summary>
        public StatusError LastTimeError { get; set; }

        /// <summary>
        /// Determines that task perform itself logic at this time
        /// </summary>
        public bool Processing { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="TaskAppStatus"/>
        /// </summary>
        public TaskAppStatus()
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="TaskAppStatus"/>
        /// </summary>
        public TaskAppStatus(TaskAppStatus origin)
        {
            LastTimeDuration = origin.LastTimeDuration;
            LastTimeStart = origin.LastTimeStart;
            LastTimeError = origin.LastTimeError;
            Processing = origin.Processing;
        }

        public object Clone()
        {
            return new TaskAppStatus(this);
        }
    }
}
