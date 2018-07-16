using MessageWorker.Core;
using MessageWorker.Core.Publish;
using MessageWorker.Core.Subscribe;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Kafka
{
    public static class SettingsExtensions
    {
        public static void UseKafkaConsummer(this ConsummerSettings settings, Action<KafkaSettings> settignsAction)
        {
            var settingsDict = new KafkaSettings();
            settignsAction(settingsDict);
            settings.Consumer = new KafkaConsumer(settingsDict.GetDictionaryConfig());
        }

        public static void UseKafkaProducer(this ProducerSettings settings, Action<KafkaSettings> settignsAction)
        {
            var settingsDict = new KafkaSettings();
            settignsAction(settingsDict);
            settings.Producer = new KafkaProducer(settingsDict.GetDictionaryConfig());
        }
    }
}
