using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Abstractions
{
    public interface ISerializer<T>
    {
        byte[] Serialize(T data);
    }
}
