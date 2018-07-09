using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using MessageWorker.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageWorker.Kafka
{
    public class KafkaProducer : IProducer
    {
        Dictionary<string, object> _configProducer;

        public KafkaProducer(Dictionary<string, object> configProducer)
        {
            _configProducer = configProducer;
        }

        public async Task Send(string topicName, byte[] data)
        {
            using (var producer = new Producer(_configProducer))
            {
                await producer.ProduceAsync(topicName, null, data);
            }
        }

        public async Task SendBatch(string topicName, params byte[][] datas)
        {
            using (var producer = new Producer(_configProducer))
            {
                foreach (var data in datas)
                {
                    await producer.ProduceAsync(topicName, null, data);
                }
            }
        }
    }
}
