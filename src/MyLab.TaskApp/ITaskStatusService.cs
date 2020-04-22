using System;
using MyLab.StatusProvider;

namespace MyLab.TaskApp
{
    /// <summary>
    /// Provides abilities to modify task status
    /// </summary>
    public interface ITaskStatusService
    {
        /// <summary>
        /// Report about task logic starting
        /// </summary>
        void LogicStarted();
        /// <summary>
        /// Report about task logic error
        /// </summary>
        void LogicError(StatusError err);
        /// <summary>
        /// Report about task logic success completion
        /// </summary>
        void LogicCompleted();
        /// <summary>
        /// Gets task status
        /// </summary>
        /// <returns></returns>
        TaskAppStatus GetStatus();
    }

    /// <summary>
    /// Contains extension methods for <see cref="ITaskStatusService"/>
    /// </summary>
    public static class TaskStatusServiceExtensions
    {
        /// <summary>
        /// Report about task logic error
        /// </summary>
        public static void LogicError(this ITaskStatusService srv, Exception e)
        {
            srv.LogicError(new StatusError(e));
        }
    }

    class DefaultTaskStatusService : ITaskStatusService
    {
        private readonly Lazy<TaskAppStatus> _status;

        public DefaultTaskStatusService(IServiceProvider serviceProvider)
        {
            _status = new Lazy<TaskAppStatus>(() =>
            {
                var statusService = (IAppStatusService)serviceProvider.GetService(typeof(IAppStatusService));
                return statusService != null 
                    ? statusService.RegSubStatus<TaskAppStatus>()
                    : new TaskAppStatus();
            });
        }

        public void LogicStarted()
        {
            if(_status.Value == null)
                return;
            
            _status.Value.LastTimeStart = DateTime.Now;
            _status.Value.LastTimeDuration = null;
            _status.Value.Processing = true;
        }

        public void LogicError(StatusError err)
        {
            if (_status.Value == null)
                return;
            _status.Value.LastTimeError = err;
            _status.Value.LastTimeDuration = DateTime.Now - _status.Value.LastTimeStart;
            _status.Value.Processing = false;
        }

        public void LogicCompleted()
        {
            if (_status.Value == null)
                return;
            _status.Value.LastTimeError = null;
            _status.Value.LastTimeDuration = DateTime.Now - _status.Value.LastTimeStart;
            _status.Value.Processing = false;
        }

        public TaskAppStatus GetStatus()
        {
            return _status.Value;
        }
    }
}
