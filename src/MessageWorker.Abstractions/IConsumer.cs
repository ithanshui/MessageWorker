using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageWorker.Abstractions
{
    public interface IConsumer
    {
        event Action<byte[]> IncomeMessage;
        event Action<Exception> ErrorProcessMessage;

        Task SubscribeAsync(string topicName, CancellationToken stoppingToken);
    }
}
