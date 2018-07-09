using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Core.Subscribe
{
    internal class Subscriber : ISubscriber
    {
        IServiceCollection _services;
        Action<ConsummerSettings> _consummerSettingsAction;

        public Subscriber(
            IServiceCollection services,
            Action<ConsummerSettings> consummerSettingsAction)
        {
            _services = services;
            _consummerSettingsAction = consummerSettingsAction;
        }

        public ISubscriber Subscribe<TMessage>(string topicName, Action<SubscribeSettings<TMessage>> options)
        {
            _services.Configure<SubscribeSettings<TMessage>>(topicName, options);

            _services.AddSingleton<IHostedService, SubscribeHostedService<TMessage>>(provider =>
            {
                var subscribeOptions = provider.GetRequiredService<IOptionsSnapshot<SubscribeSettings<TMessage>>>();
                var consummerOptions = new ConsummerSettings();
                _consummerSettingsAction(consummerOptions);

                return new SubscribeHostedService<TMessage>(
                    consummerOptions,
                    subscribeOptions,
                    topicName
                    );
            });
            return this;
        }
    }
}
