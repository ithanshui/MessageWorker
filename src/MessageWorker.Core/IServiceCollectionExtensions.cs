using MessageWorker.Core.Publish;
using MessageWorker.Core.Subscribe;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Core
{
    public static class IServiceCollectionExtensions
    {
        public static ISubscriber AddConsummer(this IServiceCollection services, Action<ConsummerSettings> settings)
        {
            services.AddOptions();
            return new Subscriber(services, settings);
        }

        public static IPublisher AddProducer(this IServiceCollection services, Action<ProducerSettings> settings)
        {
            services.AddOptions();
            return new Publisher(services, settings);
        }
    }
}
