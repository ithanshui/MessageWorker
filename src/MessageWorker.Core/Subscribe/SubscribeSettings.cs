using MessageWorker.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Core.Subscribe
{
    public class SubscribeSettings<TMessage>
    {
        public event Action<TMessage> IncomeMessage;
        public event Action<Exception> ErrorProcessMessage;
        public IDeserializer<TMessage> Deserializer { get; set; }

        internal void CallIncomeMessage(TMessage message)
        {
            IncomeMessage?.Invoke(message);
        }

        internal void CallErrorProcessMessage(Exception error)
        {
            ErrorProcessMessage?.Invoke(error);
        }
    }
}
