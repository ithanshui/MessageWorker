using FakeItEasy;
using FluentAssertions;
using MessageWorker.Abstractions;
using MessageWorker.Core.Publish;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MessageWorker.Core.Test.Publish
{
    public class PublisherUnitTest
    {
        [Fact]
        public void Publisher_RegisterOptionsByTopic()
        {
            IServiceCollection services = new ServiceCollection();

            var testClass = new Publisher(services, settings => { settings.Producer = null; });
            testClass.Register<string>("topicName1", options => { options.Serializer = A.Fake<ISerializer<string>>(); });

            var provider = services.BuildServiceProvider();
            var optionSnapshot = provider.GetRequiredService<IOptionsSnapshot<PublishSettings<string>>>();

            var options1 = optionSnapshot.Get("topicName1");
            options1.Should().NotBeNull();
            options1.Should().BeOfType<PublishSettings<string>>()
            .Which.Serializer.Should().NotBeNull();

            var options2 = optionSnapshot.Get("topicName2");
            options2.Should().NotBeNull();
            options2.Should().BeOfType<PublishSettings<string>>()
                 .Which.Serializer.Should().BeNull();
        }

        [Fact]
        public void Publisher_RegisterIPublishService()
        {
            IServiceCollection services = new ServiceCollection();

            var testClass = new Publisher(services, settings => { settings.Producer = null; });
            testClass.Register<string>("topicName1", options => { options.Serializer = A.Fake<ISerializer<string>>(); });

            var provider = services.BuildServiceProvider();
            var publishSerivce = provider.GetRequiredService<IPublishService<string>>();

            publishSerivce.Should().NotBeNull()
                .And.BeOfType<PublishService<string>>()
                .Which._topicName.Should().Be("topicName1");
        }


        [Fact]
        public void Publisher_RegisterIPublishServiceMultiple()
        {
            IServiceCollection services = new ServiceCollection();

            var testClass = new Publisher(services, settings => { settings.Producer = null; });
            testClass
                .Register<string>("topicName1", options => { options.Serializer = A.Fake<ISerializer<string>>(); })
                .Register<int>("topicName2", options => { options.Serializer = A.Fake<ISerializer<int>>(); });

            var provider = services.BuildServiceProvider();

            var publishSerivceString = provider.GetRequiredService<IPublishService<string>>();
            publishSerivceString.Should().NotBeNull()
                .And.BeOfType<PublishService<string>>()
                .Which._topicName.Should().Be("topicName1");


            var publishSerivceInt = provider.GetRequiredService<IPublishService<int>>();
            publishSerivceInt.Should().NotBeNull()
                .And.BeOfType<PublishService<int>>()
                .Which._topicName.Should().Be("topicName2");
        }
    }
}
