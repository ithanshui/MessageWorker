using MessageWorker.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageWorker.Core.Publish
{
    internal class PublishService<TMessage> : IPublishService<TMessage>
    {
        PublishSettings<TMessage> _publishSettings;
        ProducerSettings _producerSettings;
        string _topicName;

        public PublishService(
            ProducerSettings producerSettings,
            IOptionsSnapshot<PublishSettings<TMessage>> publishSettings,
            string topicName)
        {
            _publishSettings = publishSettings.Get(topicName);
            _producerSettings = producerSettings;
            _topicName = topicName;
        }

        public void Send(TMessage msg)
        {
            var data = _publishSettings.Serializer.Serialize(msg);
            _producerSettings.Producer.Send(_topicName, data);
        }

        public void SendBatch(params TMessage[] msgs)
        {
            var data = msgs.Select(msg => _publishSettings.Serializer.Serialize(msg)).ToArray();
            _producerSettings.Producer.SendBatch(_topicName, data);
        }
    }
}
