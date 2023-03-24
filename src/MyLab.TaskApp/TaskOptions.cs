using System;

namespace MyLab.TaskApp
{
    /// <summary>
    /// Contains task performing options
    /// </summary>
    public class TaskOptions
    {
        /// <summary>
        /// Idle period between iteration when task os circle
        /// </summary>
        public TimeSpan IdlePeriod { get; set; }

        /// <summary>
        /// Iteration protocol identifier
        /// </summary>
        public string IterationProtocolId { get; set; } = "tasks";
    }
}
