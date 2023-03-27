using System.Threading;
using System.Threading.Tasks;
using MyLab.TaskApp.IterationContext;

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
        Task PerformAsync(TaskIterationContext iterationContext, CancellationToken cancellationToken);  
    }
}
