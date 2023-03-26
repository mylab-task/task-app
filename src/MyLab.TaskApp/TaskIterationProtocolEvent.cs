using System.Runtime.Serialization;
using MyLab.ProtocolStorage.Client.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MyLab.TaskApp
{
    class TaskIterationProtocolEvent : ProtocolEvent
    {
        [JsonProperty("isEmpty")]
        public bool IsEmptyIteration { get; set; }

        [JsonProperty("kicker")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TaskKicker Kicker { get; set; }
    }

    enum TaskKicker
    {
        [EnumMember(Value = "undefined")] 
        Undefined,
        [EnumMember(Value = "api")]
        Api,
        [EnumMember(Value = "scheduler")]
        Scheduler
    }
}
