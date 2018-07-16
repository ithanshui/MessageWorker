using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.AMQP
{
    public class AMQPSettings
    {
        public Uri Address { get; set; }
        public string LinkName { get; set; }
    }
}
