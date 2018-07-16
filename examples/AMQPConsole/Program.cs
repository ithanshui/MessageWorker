using MessageWorker.Abstractions;
using MessageWorker.Core;
using MessageWorker.AMQP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AMQPConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder()
                  .ConfigureAppConfiguration((hostingContext, config) =>
                  {
                  })
                  .ConfigureServices((hostContext, services) =>
                  {
                      services.AddProducer(options =>
                      {
                          options.UseAMQPProducer(settings =>
                          {
                              settings.LinkName = "producer-name";
                              settings.Address = new Uri("amqp://username:password@host:1111");
                          });
                      })
                           .Register<string>("topicName1", options =>
                           {
                               options.Serializer = new StringSerializer();
                           });

                      services.AddConsummer(options =>
                      {
                          options.UseAMQPConsummer(settings =>
                          {
                              settings.LinkName = "consummer-name";
                              settings.Address = new Uri("amqp://username:password@host:1111");
                          });
                      }).Subscribe<string>("topicName1", options =>
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
                    Console.WriteLine("InputMessage: ");
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
    }
}
