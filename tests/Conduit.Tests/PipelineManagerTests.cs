using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Conduit.Tests
{
    public class PipelineManagerTests
    {
        [Theory]
        [InlineData(default(string))]
        [InlineData("")]
        [InlineData("  ")]
        public void WhenNameIsNotSuppliedThenNameTransformedToDefault(string name)
        {
            var target = CreateTarget(out var options);
            options.Setup(x => x.Get(Constants.DefaultPipeline)).Returns(new AsyncPipelineOptions<int>());

            target.Get(name).Should().NotBeNull();

            options.Verify(x => x.Get(Constants.DefaultPipeline), Times.Once);
        }

        [Fact]
        public void WhenNameIsSuppliedThenLoadsCorrectly()
        {
            var target = CreateTarget(out var options);
            var name = "testing";
            options.Setup(x => x.Get(name)).Returns(new AsyncPipelineOptions<int>());

            target.Get(name).Should().NotBeNull();

            options.Verify(x => x.Get(name), Times.Once);
        }

        private AsyncPipelineManager<int> CreateTarget(out Mock<IOptionsMonitor<AsyncPipelineOptions<int>>> options)
        {
            options = new Mock<IOptionsMonitor<AsyncPipelineOptions<int>>>();

            return new AsyncPipelineManager<int>(options.Object, new Mock<IServiceScopeFactory>().Object);
        }
    }
}
