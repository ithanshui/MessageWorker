using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Abstractions
{
    public interface IDeserializer<T>
    {
        T Deserialize(byte[] data);
    }
}
