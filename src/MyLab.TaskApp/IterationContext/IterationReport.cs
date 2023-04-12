using System.Collections.Generic;

namespace MyLab.TaskApp.IterationContext
{
    /// <summary>
    /// Task iteration report
    /// </summary>
    public class IterationReport
    {
        /// <summary>
        /// The identifier which correlate with task iteration
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets iteration workload
        /// </summary>
        public IterationWorkload Workload { get; set; }
        
        /// <summary>
        /// Gets or sets context subject identifier 
        /// </summary>
        public string SubjectId { get; set; }

        /// <summary>
        /// Gets or sets business-level named numeric metrics
        /// </summary>
        public IDictionary<string, double> Metrics { get; set; }
    }
}