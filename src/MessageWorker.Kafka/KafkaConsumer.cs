using Confluent.Kafka;
using MessageWorker.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageWorker.Kafka
{
    internal class KafkaConsumer : IConsumer
    {
        Dictionary<string, object> _configConsummer;

        public KafkaConsumer(Dictionary<string, object> configConsummer)
        {
            _configConsummer = configConsummer;
        }

        public event Action<byte[]> IncomeMessage;
        public event Action<Exception> ErrorProcessMessage;

        public async Task SubscribeAsync(string topicName, CancellationToken stoppingToken)
        {
            using (var consumer = new Consumer(_configConsummer))
            {
                //consumer.OnError += (_, error)=> 
                //    => _logger.LogError($"Error: {error}");

                //consumer.OnConsumeError += (_, error)
                //    => _logger.LogError($"Consume error: {error}");

                consumer.OnPartitionsAssigned += (_, partitions)
                    => consumer.Assign(partitions);

                consumer.OnPartitionsRevoked += (_, partitions)
                    => consumer.Unassign();

                consumer.Subscribe(topicName);

                while (!stoppingToken.IsCancellationRequested)
                {
                    if (consumer.Consume(out var response, TimeSpan.FromMilliseconds(10)))
                    {
                        try
                        {
                            IncomeMessage?.Invoke(response.Value);
                            await consumer.CommitAsync(response);
                        }
                        catch (Exception e)
                        {
                            ErrorProcessMessage?.Invoke(e);
                        }
                    }
                }
            }
        }
    }
}
