using System.Threading;
using System.Threading.Tasks;

namespace MyLab.TaskApp
{
    /// <summary>
    /// Represent a primary task-application logic
    /// </summary>
    public interface ITaskLogic
    {
        /// <summary>
        /// Performs a task logic
        /// </summary>
        Task<IterationDesc> Perform(CancellationToken cancellationToken);  
    }

    /// <summary>
    /// Contains task iteration result
    /// </summary>
    public class IterationDesc
    {
        public static readonly IterationDesc EmptyIteration = new IterationDesc{ IsEmptyIteration = true };
        public bool IsEmptyIteration { get; set; }
    }
}
