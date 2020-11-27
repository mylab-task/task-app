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
        Task Perform(CancellationToken cancellationToken);
    }
}
