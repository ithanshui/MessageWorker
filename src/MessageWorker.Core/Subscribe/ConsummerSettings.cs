using MessageWorker.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Core.Subscribe
{
    public class ConsummerSettings
    {
        public IConsumer Consumer { get; set; }
    }
}
