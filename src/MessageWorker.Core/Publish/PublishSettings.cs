using MessageWorker.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Core.Publish
{
    public class PublishSettings<TMessage>
    {
        public ISerializer<TMessage> Serializer { get; set; }
    }
}
