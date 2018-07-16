using Amqp;
using Amqp.Framing;
using MessageWorker.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageWorker.AMQP
{
    internal class AMQPProducer : IProducer
    {
        AMQPSettings _configProducer;

        public AMQPProducer(AMQPSettings configProducer)
        {
            _configProducer = configProducer;
        }

        public async Task Send(string topicName, byte[] data)
        {
            var address = new Address(_configProducer.Address.ToString());
            Connection connection = null;
            Session session = null;
            SenderLink sender = null;
            try
            {
                connection = await Connection.Factory.CreateAsync(address);
                session = new Session(connection);
                sender = new SenderLink(session, _configProducer.LinkName, topicName);

                Message message = new Message(data);
                await sender.SendAsync(message);
            }
            finally
            {
                if (!sender.IsClosed)
                    await sender.CloseAsync();
                if (!session.IsClosed)
                    await session.CloseAsync();
                if (!connection.IsClosed)
                    await connection.CloseAsync();
            }
        }

        public async Task SendBatch(string topicName, params byte[][] datas)
        {
            SenderLink sender = null;
            Connection connection = null;
            Session session = null;
            try
            {
                var address = new Address(_configProducer.Address.ToString());
                connection = await Connection.Factory.CreateAsync(address);
                session = new Session(connection);
                sender = new SenderLink(session, _configProducer.LinkName, topicName);

                List<Task> runedTask = new List<Task>(datas.Length);
                foreach (var data in datas)
                {
                    Message message = new Message(data);
                    runedTask.Add(sender.SendAsync(message));
                }
                Task.WaitAll(runedTask.ToArray());
            }
            finally
            {
                if (!sender.IsClosed)
                    await sender.CloseAsync();
                if (!session.IsClosed)
                    await session.CloseAsync();
                if (!connection.IsClosed)
                    await connection.CloseAsync();
            }

        }
        
    }
}
