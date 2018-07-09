using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using MessageWorker.Core;
using MessageWorker.Abstractions;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Collections.Specialized;
using Microsoft.Extensions.Hosting;
using MessageWorker.Serializer.JSON;

namespace ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddProducer(options =>
                    {
                        options.Producer = new MemoryProducer();
                    })
                         .Register<string>("MESSAGE_STORE", options =>
                         {
                             options.Serializer = new StringSerializer();
                         });

                    services.AddConsummer(settings =>
                    {
                        settings.Consumer = new MemoryConsumer();
                    }).Subscribe<string>("MESSAGE_STORE", options =>
                    {
                        options.Deserializer = new StringDeserializer();
                        options.IncomeMessage += (msg) => Console.WriteLine($"INCOME: {msg}");
                    });

                    services.AddSingleton<IHostedService, MainService>();
                });
            builder.RunConsoleAsync().Wait();
        }


        class MainService : BackgroundService
        {
            IPublishService<string> _publishService;

            public MainService(IPublishService<string> publishService)
            {
                _publishService = publishService;
            }

            protected override Task ExecuteAsync(CancellationToken stoppingToken)
            {
                while (true)
                {
                    Console.Write("InputMessage: ");
                    var msg = Console.ReadLine();
                    if (msg.Equals("END"))
                        break;

                    _publishService.Send(msg);
                }

                return Task.CompletedTask;
            }
        }

        class StringDeserializer : IDeserializer<string>
        {
            public string Deserialize(byte[] data)
            {
                return Encoding.UTF8.GetString(data);
            }
        }
        class StringSerializer : ISerializer<string>
        {
            public byte[] Serialize(string data)
            {
                return Encoding.UTF8.GetBytes(data);
            }
        }

        static ObservableCollection<string> MESSAGE_STORE = new ObservableCollection<string>();

        class MemoryProducer : IProducer
        {
            public Task Send(string topicName, byte[] data)
            {
                MESSAGE_STORE.Add(Encoding.UTF8.GetString(data));
                return Task.CompletedTask;
            }

            public Task SendBatch(string topicName, params byte[][] datas)
            {
                foreach (var data in datas)
                    MESSAGE_STORE.Add(Encoding.UTF8.GetString(data));
                return Task.CompletedTask;
            }
        }

        class MemoryConsumer : IConsumer
        {
            public event Action<byte[]> IncomeMessage;
            public event Action<Exception> ErrorProcessMessage;

            public Task SubscribeAsync(string topicName, CancellationToken stoppingToken)
            {
                MESSAGE_STORE.CollectionChanged += MESSAGE_STORE_CollectionChanged;
                return Task.CompletedTask;
            }

            private void MESSAGE_STORE_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action != NotifyCollectionChangedAction.Add)
                    return;

                foreach (var message in e.NewItems)
                {
                    IncomeMessage?.Invoke(Encoding.UTF8.GetBytes(message.ToString()));
                }
            }
        }

    }
}
