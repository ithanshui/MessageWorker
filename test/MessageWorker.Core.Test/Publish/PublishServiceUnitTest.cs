using FakeItEasy;
using MessageWorker.Abstractions;
using MessageWorker.Core.Publish;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MessageWorker.Core.Test.Publish
{
    public class PublishServiceUnitTest
    {
        [Fact]
        public void PublishService_GetOptionByTopicName()
        {
            var moqProducerSettings = new ProducerSettings()
            {
                Producer = A.Fake<IProducer>()
            };

            var moqOptionsSnapshot = A.Fake<IOptionsSnapshot<PublishSettings<string>>>();
            A.CallTo(() => moqOptionsSnapshot.Get(A<string>.That.Not.IsEqualTo("topicName")))
                .Throws(new Exception("Not valid copnfiguration name."));

            var publishSerivce =
                new PublishService<string>(moqProducerSettings, moqOptionsSnapshot, "topicName");
        }

        [Fact]
        public void PublishService_Send_CallSerializer()
        {
            var moqProducerSettings = new ProducerSettings()
            {
                Producer = A.Fake<IProducer>()
            };

            var moqSerializer = A.Fake<ISerializer<string>>();
            var moqPublishSettings = new PublishSettings<string>()
            {
                Serializer = moqSerializer
            };
            var moqOptionsSnapshot = A.Fake<IOptionsSnapshot<PublishSettings<string>>>();
            A.CallTo(() => moqOptionsSnapshot.Get(A<string>._)).Returns(moqPublishSettings);

            var publishSerivce =
                new PublishService<string>(moqProducerSettings, moqOptionsSnapshot, "topicName");

            publishSerivce.Send("test msg");

            A.CallTo(() => moqSerializer.Serialize("test msg")).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void PublishService_Send_CallProducer()
        {
            var moqProducer = A.Fake<IProducer>();
            var moqProducerSettings = new ProducerSettings()
            {
                Producer = moqProducer
            };

            var moqSerializer = A.Fake<ISerializer<string>>();
            A.CallTo(() => moqSerializer.Serialize(A<string>._))
                .Returns(new byte[] { 0x0, 0x1, 0x2 });
            var moqPublishSettings = new PublishSettings<string>()
            {
                Serializer = moqSerializer
            };
            var moqOptionsSnapshot = A.Fake<IOptionsSnapshot<PublishSettings<string>>>();
            A.CallTo(() => moqOptionsSnapshot.Get(A<string>._)).Returns(moqPublishSettings);

            var publishSerivce
                = new PublishService<string>(moqProducerSettings, moqOptionsSnapshot, "topicName");

            publishSerivce.Send("test msg");

            A.CallTo(() => moqProducer.Send("topicName", A<byte[]>.That.IsSameSequenceAs(new byte[] { 0x0, 0x1, 0x2 })))
            .MustHaveHappenedOnceExactly();
        }
    }
}
