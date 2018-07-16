using MessageWorker.Core.Publish;
using MessageWorker.Core.Subscribe;
using System;

namespace MessageWorker.AMQP
{
    public static class SettingsExtensions
    {
        public static void UseAMQPConsummer(this ConsummerSettings settings, Action<AMQPSettings> settignsAction)
        {
            var settingsDict = new AMQPSettings();
            settignsAction(settingsDict);
            settings.Consumer = new AMQPConsumer(settingsDict);
        }

        public static void UseAMQPProducer(this ProducerSettings settings, Action<AMQPSettings> settignsAction)
        {
            var settingsDict = new AMQPSettings();
            settignsAction(settingsDict);
            settings.Producer = new AMQPProducer(settingsDict);
        }
    }
}
