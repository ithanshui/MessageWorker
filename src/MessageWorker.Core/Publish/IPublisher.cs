namespace MessageWorker.Core.Publish
{
    public interface IPublisher
    {
        IPublisher Register<TMessage>(string topicName, System.Action<PublishSettings<TMessage>> options);
    }
}