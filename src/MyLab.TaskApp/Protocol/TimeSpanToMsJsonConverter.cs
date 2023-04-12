using System;
using Newtonsoft.Json;

namespace MyLab.TaskApp.Protocol
{
    class TimeSpanToMsJsonConverter : JsonConverter<TimeSpan>
    {
        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            writer.WriteValue(Math.Round(value.TotalMilliseconds).ToString("F0"));
        }

        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}