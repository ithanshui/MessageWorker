using MessageWorker.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Core.Publish
{
    internal class Publisher : IPublisher
    {
        IServiceCollection _services;
        Action<ProducerSettings> _producerSettingsAction;

        public Publisher(IServiceCollection services, Action<ProducerSettings> producerSettingsAction)
        {
            _services = services;
            _producerSettingsAction = producerSettingsAction;
        }

        public IPublisher Register<TMessage>(string topicName, Action<PublishSettings<TMessage>> options)
        {
            _services.Configure<PublishSettings<TMessage>>(topicName, options);

            _services.AddScoped<IPublishService<TMessage>>(provider =>
            {
                var producerSettings = new ProducerSettings();
                _producerSettingsAction?.Invoke(producerSettings);
                var subscribeOptions = provider.GetRequiredService<IOptionsSnapshot<PublishSettings<TMessage>>>();

                return new PublishService<TMessage>(
                    producerSettings,
                    subscribeOptions,
                    topicName
                    );
            });
            return this;
        }
    }
}
