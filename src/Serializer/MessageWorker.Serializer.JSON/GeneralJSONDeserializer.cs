using MessageWorker.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Serializer.JSON
{
    internal class GeneralJSONDeserializer<TMessage> : IDeserializer<TMessage>
    {
        public TMessage Deserialize(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<TMessage>(json);
        }
    }
}
