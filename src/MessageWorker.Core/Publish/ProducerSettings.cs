using MessageWorker.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Core.Publish
{
    public class ProducerSettings
    {
        public IProducer Producer { get; set; }
    }
}
