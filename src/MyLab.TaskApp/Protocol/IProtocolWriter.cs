using System;
using System.Reflection;
using System.Threading.Tasks;
using MyLab.ProtocolStorage.Client;
using MyLab.TaskApp.IterationContext;

namespace MyLab.TaskApp.Protocol
{
    interface IProtocolWriter
    {
        Task WriteAsync(TaskIterationContext ctx, TimeSpan iterationDuration, Exception error = null);
    }

    class ProtocolWriter : IProtocolWriter
    {
        private readonly SafeProtocolIndexerV1 _indexer;

        public TaskKicker TaskKicker { get; set; }

        public string ProtocolType { get; set; }

        public string ProtocolId { get; set; }

        public ProtocolWriter(SafeProtocolIndexerV1 indexer)
        {
            _indexer = indexer;
        }
        public Task WriteAsync(TaskIterationContext ctx, TimeSpan iterationDuration, Exception error = null)
        {
            return _indexer.PostEventAsync(
                ProtocolId ?? ProtocolEventConstants.DefaultProtocolId, 
                new TaskIterationProtocolEvent
                {
                    Id = ctx.Report?.CorrelationId,
                    DateTime = ctx.StartAt,
                    Metrics = ctx.Report?.Metrics,
                    Subject = ctx.Report?.SubjectId,
                    Type = ProtocolType ?? Assembly.GetEntryAssembly()?.GetName().Name ?? ProtocolEventConstants.DefaultType,
                    TraceId = ctx.Id,
                    Kicker = TaskKicker,
                    Workload = ctx.Report?.Workload ?? IterationWorkload.Undefined,
                    Duration = iterationDuration,
                    Error = error
                }
            );
        }
    }
}
