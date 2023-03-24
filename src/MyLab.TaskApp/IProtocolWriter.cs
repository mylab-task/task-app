using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyLab.ProtocolStorage.Client;
using MyLab.ProtocolStorage.Client.Models;

namespace MyLab.TaskApp
{
    interface IProtocolWriter
    {
        Task WriteAsync(IterationDesc iterationDesc);
    }

    class ProtocolWriter : IProtocolWriter
    {
        private readonly SafeProtocolIndexerV1 _indexer;
        private readonly string _protocolId;

        public TaskKicker TaskKicker { get; set; }

        public ProtocolWriter(SafeProtocolIndexerV1 indexer, string protocolId)
        {
            _indexer = indexer;
            _protocolId = protocolId;
        }
        public Task WriteAsync(IterationDesc iterationDesc)
        {
            return _indexer.PostEventAsync(_protocolId, new TaskIterationProtocolEvent()
            {
                IsEmptyIteration = iterationDesc.IsEmptyIteration,
                Kicker = TaskKicker
            });
        }
    }
}
