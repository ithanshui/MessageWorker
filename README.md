# MessageWorker

| Branch	| Build status |
| ------- | ------------ |
| dev			| [![Build Status](https://travis-ci.org/mukmyash/MessageWorker.svg?branch=dev)](https://travis-ci.org/mukmyash/MessageWorker)|
| master	| [![Build Status](https://travis-ci.org/mukmyash/MessageWorker.svg?branch=master)](https://travis-ci.org/mukmyash/MessageWorker) |

Simple library for create consumer and producer by asynchronous communication.

# Packages
- MessageWorker.Abstraction - NET Core asynchronous communication and startup abstractions for applications.
- MessageWorker.Core - Base class for asynchronous communication
- MessageWorker.Kafka - Consumer\Producer for communication via Kafka. 
- MessageWorker.AMQP - Consumer\Producer for communication via AMQP.

# Simple usage

1. Communication via kafka
    1. Producer
        ``` C#
        services.AddProducer(options =>
        {
            options.UseKafkaProducer(settings =>
            {
                settings.GroupId = "group_id";
                settings.ClientId = "client-id";
                settings.Servers = new List<string>() { "serverName:port" };
                //Auth config
                settings.SecurityProtocol = "SASL_PLAINTEXT";
                settings.SaslConfig = new SaslConfig()
                {
                    Mechanism = "PLAIN",
                    Username = "username",
                    Password = "password"
                };
            });
        })
        // Register MessageModel class for sent message in the topic topicName1  
        .Register<MessageModel>("topicName1", options =>
        {
            // Register JSON serializer
            options.UseJSONSerializer();
        })
        // Register MessageModel2 class for sent message in the topic topicName2  
        .Register<MessageModel2>("topicName2", options =>
        {
            // Register JSON serializer
            options.UseJSONSerializer();
        });;
        ```
    1. Consummer

        ``` C#
        services.AddConsummer(options =>
        {
            options.UseKafkaConsummer(settings =>
            {
                settings.GroupId = "group_id";
                settings.ClientId = "client-id";
                settings.Servers = new List<string>() { "serverName:port" };
                //Auth config
                settings.SecurityProtocol = "SASL_PLAINTEXT";
                settings.SaslConfig = new SaslConfig()
                {
                    Mechanism = "PLAIN",
                    Username = "username",
                    Password = "password"
                };
            });
        })
        // Register MessageModel class for incoming message in the topic topicName1
        .Subscribe<MessageModel>("topicName1", options =>
        {
            // Register JSON deserializer
            options.UseJSONDeserializer();

            // Register event handler for incoming messages
            options.IncomeMessage += (msg) => Console.WriteLine($"INCOME: {msg}");
        });
        ```
1. Communication via AMQP
    1. Producer
        
        ``` C#
        services.AddProducer(options =>
        {
            options.UseAMQPProducer(settings =>
            {
                settings.LinkName = "producer-name";
                settings.Address = new Uri("amqp://username:password@host:1111");
            });
        })
        // Register MessageModel class for sent message in the topic topicName1  
        .Register<MessageModel>("topicName1", options =>
        {
            // Register JSON serializer
            options.UseJSONSerializer();
        })
        // Register MessageModel2 class for sent message in the topic topicName2  
        .Register<MessageModel2>("topicName2", options =>
        {
            // Register JSON serializer
            options.UseJSONSerializer();
        });
        ```
    1. Consummer

        ``` C#
        services.AddConsummer(options =>
        {
            options.UseAMQPConsummer(settings =>
            {
                settings.LinkName = "consummer-name";
                settings.Address = new Uri("amqp://username:password@host:1111");
            });
        })
        // Register MessageModel class for incoming message in the topic topicName1
        .Subscribe<MessageModel>("topicName1", options =>
        {
            // Register JSON deserializer
            options.UseJSONDeserializer();

            // Register event handler for incoming messages
            options.IncomeMessage += (msg) => Console.WriteLine($"INCOME: {msg}");
        });
        ```


## Create custom consummer\producer
Implements interfaces __IConsummer__ and __IProducer__ from MessageWorker.Abstractions.

### Sample
Create inmemory producer and consummer

```
static ObservableCollection<string> MESSAGE_STORE = new ObservableCollection<string>();
```

#### Simple Consummer
``` C#
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
```

#### Simple Producer
``` C#
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
```

For simple usage you may can create settings extensions
``` C#
public static void UserInMemoryConsummer(this ConsummerSettings settings)
{
    settings.Consumer = new MemoryConsumer();
}

public static void UserInMemoryProducer(this ProducerSettings settings)
{
    settings.Producer = new MemoryProducer();
}
```