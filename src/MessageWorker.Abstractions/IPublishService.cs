using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Abstractions
{
    public interface IPublishService<TMessage>
    {
        void Send(TMessage msg);
        void SendBatch(params TMessage[] msgs);
    }
}
