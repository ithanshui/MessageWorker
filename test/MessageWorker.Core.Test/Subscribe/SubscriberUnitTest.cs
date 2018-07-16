using FakeItEasy;
using FluentAssertions;
using MessageWorker.Abstractions;
using MessageWorker.Core.Subscribe;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MessageWorker.Core.Test.Subscribe
{
    public class SubscriberUnitTest
    {
        [Fact]
        public void Subscriber_RegisterOptionsByTopic()
        {
            IServiceCollection services = new ServiceCollection();

            var testClass = new Subscriber(services, settings => { settings.Consumer = null; });
            testClass.Subscribe<string>("topicName1", options => { options.Deserializer = A.Fake<IDeserializer<string>>(); });

            var provider = services.BuildServiceProvider();
            var optionSnapshot = provider.GetRequiredService<IOptionsSnapshot<SubscribeSettings<string>>>();

            var options1 = optionSnapshot.Get("topicName1");
            options1.Should().NotBeNull();
            options1.Should().BeOfType<SubscribeSettings<string>>()
            .Which.Deserializer.Should().NotBeNull();

            var options2 = optionSnapshot.Get("topicName2");
            options2.Should().NotBeNull();
            options2.Should().BeOfType<SubscribeSettings<string>>()
                 .Which.Deserializer.Should().BeNull();
        }

        [Fact]
        public void Subscriber_RegisterIHostedService()
        {
            IServiceCollection services = new ServiceCollection();

            var testClass = new Subscriber(services, settings => { settings.Consumer = A.Fake<IConsumer>(); });
            testClass.Subscribe<string>("topicName1", options => { options.Deserializer = A.Fake<IDeserializer<string>>(); });

            var provider = services.BuildServiceProvider();
            var hostedService = provider.GetRequiredService<IHostedService>();

            hostedService.Should().NotBeNull()
                .And.BeOfType<SubscribeHostedService<string>>()
                .Which._topicName.Should().Be("topicName1");
        }
    }
}
