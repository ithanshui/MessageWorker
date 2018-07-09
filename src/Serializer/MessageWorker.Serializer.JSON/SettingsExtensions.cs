using MessageWorker.Core;
using MessageWorker.Core.Publish;
using MessageWorker.Core.Subscribe;
using System;

namespace MessageWorker.Serializer.JSON
{
    public static class SettingsExtensions
    {
        public static void UseJSONDeserializer<TMessage>(this SubscribeSettings<TMessage> settigs)
        {
            settigs.Deserializer = new GeneralJSONDeserializer<TMessage>();
        }

        public static void UseJSONSerializer<TMessage>(this PublishSettings<TMessage> settigs)
        {
            settigs.Serializer = new GeneralJSONSerializer<TMessage>();
        }

    }
}
