using System;
using System.Threading.Tasks;

namespace MessageWorker.Abstractions
{
    public interface IProducer
    {
        Task Send(string topicName, byte[] data);
        Task SendBatch(string topicName, params byte[][] data);
    }
}
