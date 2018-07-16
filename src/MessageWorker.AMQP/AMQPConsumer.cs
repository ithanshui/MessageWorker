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
    internal class AMQPConsumer : IConsumer
    {
        AMQPSettings _configProducer;

        public AMQPConsumer(AMQPSettings configProducer)
        {
            _configProducer = configProducer;
        }

        public event Action<byte[]> IncomeMessage;
        public event Action<Exception> ErrorProcessMessage;

        const int WAIT_TIME = 10 * 1000;
        ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        Connection _connection = null;
        Session _session = null;

        public async Task SubscribeAsync(string topicName, CancellationToken stoppingToken)
        {
            OpenConnection();

            while (!stoppingToken.IsCancellationRequested)
            {
                //Receive message every seconds;
                await Task.Delay(1000);
                if (_manualResetEvent.WaitOne(WAIT_TIME))
                {
                    ReceiverLink receiver = null;
                    try
                    {
                        receiver = new ReceiverLink(_session, _configProducer.LinkName, topicName);
                        Message message = await receiver.ReceiveAsync();
                        var byteMessage = message.Encode().Buffer;
                        IncomeMessage?.Invoke(byteMessage);
                        receiver.Accept(message);
                    }
                    catch (Exception)
                    {
                        Reconnect();
                    }
                    finally
                    {
                        if (!receiver.IsClosed)
                            await receiver.CloseAsync();
                    }
                }
                else
                {
                    Reconnect();
                }
            }

            if (!_session.IsClosed)
                await _session.CloseAsync();
            if (!_connection.IsClosed)
                await _connection.CloseAsync();
        }

        private async Task Reconnect()
        {
            _manualResetEvent.Reset();
            await OpenConnection();
        }

        private async Task OpenConnection()
        {
            if (_connection?.IsClosed != false)
                _connection.Close();

            var address = new Address(_configProducer.Address.ToString());
            _connection = await Connection.Factory.CreateAsync(address, null, onOpenedConnection);
            _connection.AddClosedCallback(connectionClosed);
        }

        private async void connectionClosed(IAmqpObject sender, Error error)
        {
            await Reconnect();
        }

        private void onOpenedConnection(IConnection connection, Open open)
        {
            if (_session?.IsClosed != false)
                _session.Close();
            _session = new Session((Connection)connection, new Begin() { }, onBeginSession);
        }

        private void onBeginSession(ISession session, Begin begin)
        {
            _manualResetEvent.Set();
        }
    }
}
