using System;

namespace MessageWorker.Core.Subscribe
{
    public interface ISubscriber
    {
        ISubscriber Subscribe<TMessage>(string topicName, Action<SubscribeSettings<TMessage>> options);
    }
}