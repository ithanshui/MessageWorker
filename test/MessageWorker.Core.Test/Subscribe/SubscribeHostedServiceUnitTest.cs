using FakeItEasy;
using FluentAssertions;
using MessageWorker.Abstractions;
using MessageWorker.Core.Subscribe;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MessageWorker.Core.Test.Subscribe
{
    public class SubscribeHostedServiceUnitTest
    {
        [Fact]
        public async Task SubscribeHostedService_CallSubscribeAsyncConsummer()
        {
            var moqIConsumer = A.Fake<IConsumer>();
            var moqConsummerSettings = new ConsummerSettings()
            {
                Consumer = moqIConsumer
            };

            var optionsSnapshot = A.Fake<IOptionsSnapshot<SubscribeSettings<string>>>();

            var testClass = new SubscribeHostedService<string>(moqConsummerSettings, optionsSnapshot, "topicName");

            await testClass.StartAsync(CancellationToken.None);

            A.CallTo(() => moqIConsumer.SubscribeAsync("topicName", A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SubscribeHostedService_CallDeserializer()
        {
            var moqIConsumer = A.Fake<IConsumer>();
            var moqConsummerSettings = new ConsummerSettings()
            {
                Consumer = moqIConsumer
            };
            var moqDeserializer = A.Fake<IDeserializer<string>>();
            var moqSubscribeSettings = new SubscribeSettings<string>()
            {
                Deserializer = moqDeserializer
            };
            var optionsSnapshot = A.Fake<IOptionsSnapshot<SubscribeSettings<string>>>();
            A.CallTo(() => optionsSnapshot.Get(A<string>._)).Returns(moqSubscribeSettings);

            var testClass = new SubscribeHostedService<string>(moqConsummerSettings, optionsSnapshot, "topicName");

            moqIConsumer.IncomeMessage += Raise.FreeForm.With(new byte[] { 0x0, 0x1, 0x2 });

            A.CallTo(() => moqDeserializer.Deserialize(A<byte[]>.That.IsSameSequenceAs(new byte[] { 0x0, 0x1, 0x2 })))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SubscribeHostedService_CallEvent()
        {
            var moqIConsumer = A.Fake<IConsumer>();
            var moqConsummerSettings = new ConsummerSettings()
            {
                Consumer = moqIConsumer
            };
            var moqDeserializer = A.Fake<IDeserializer<string>>();
            A.CallTo(() => moqDeserializer.Deserialize(A<byte[]>._))
                .Returns("test msg");
            var moqSubscribeSettings = new SubscribeSettings<string>()
            {
                Deserializer = moqDeserializer
            };
            bool isCallIncomeMessage = false;
            moqSubscribeSettings.IncomeMessage += (data) =>
            {
                isCallIncomeMessage = true;
                data.Should().BeEquivalentTo("test msg");
            };

            var optionsSnapshot = A.Fake<IOptionsSnapshot<SubscribeSettings<string>>>();
            A.CallTo(() => optionsSnapshot.Get(A<string>._)).Returns(moqSubscribeSettings);

            var testClass = new SubscribeHostedService<string>(moqConsummerSettings, optionsSnapshot, "topicName");

            moqIConsumer.IncomeMessage += Raise.FreeForm.With(new byte[] { 0x0, 0x1, 0x2 });

            isCallIncomeMessage.Should().BeTrue();
        }
    }
}
