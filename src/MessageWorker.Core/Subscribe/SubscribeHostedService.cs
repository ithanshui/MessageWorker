using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageWorker.Core.Subscribe
{
    internal class SubscribeHostedService<TMessage> : BackgroundService
    {
        internal SubscribeSettings<TMessage> _subscribeSettings;
        internal ConsummerSettings _consummerSettings;

        internal string _topicName;

        internal SubscribeHostedService(
            ConsummerSettings consummerSettings,
            IOptionsSnapshot<SubscribeSettings<TMessage>> subscribeSettings,
            string topicName)
        {
            _consummerSettings = consummerSettings;
            _subscribeSettings = subscribeSettings.Get(topicName);
            _topicName = topicName;

            _consummerSettings.Consumer.IncomeMessage += Consumer_IncomeMessage;
            _consummerSettings.Consumer.ErrorProcessMessage += Consumer_ErrorProcessMessage;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => _consummerSettings.Consumer.SubscribeAsync(_topicName, stoppingToken));
            return Task.CompletedTask;
        }

        private void Consumer_ErrorProcessMessage(Exception obj)
        {
            _subscribeSettings.CallErrorProcessMessage(obj);
        }

        private void Consumer_IncomeMessage(byte[] data)
        {
            var obj = _subscribeSettings.Deserializer.Deserialize(data);
            _subscribeSettings.CallIncomeMessage(obj);
        }
    }
}
