using MessageWorker.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Serializer.JSON
{
    internal class GeneralJSONSerializer<TMessage> : ISerializer<TMessage>
    {
        public byte[] Serialize(TMessage data)
        {
            var json = JsonConvert.SerializeObject(data);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
