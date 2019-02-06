using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Conduit.Tests
{
    public class AsyncPipelineTests
    {
        [Fact]
        public async Task WhenNoPhasesInputIsNotModified()
        {
            var target = CreateTarget(Enumerable.Empty<Func<IServiceProvider, int, CancellationToken, Task<int>>>(), out _);

            (await target.ExecuteAsync(1, default)).Should().Be(1);
        }

        [Fact]
        public async Task WhenMultiplePhasesPresentInputIsModified()
        {
            Func<IServiceProvider, int, CancellationToken, Task<int>> phase = (sp, input, ct) => Task.FromResult(input + 1);
            var phases = Enumerable.Repeat(phase, 4);
            var target = CreateTarget(phases, out _);

            (await target.ExecuteAsync(1, default)).Should().Be(5);
        }

        private static AsyncPipeline<int> CreateTarget(IEnumerable<Func<IServiceProvider, int, CancellationToken, Task<int>>> phases, out Mock<IServiceProvider> serviceProvider)
        {
            serviceProvider = new Mock<IServiceProvider>();

            var scopeFactory = new Mock<IServiceScopeFactory>();
            var scope = new Mock<IServiceScope>();
            scope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
            scopeFactory.Setup(x => x.CreateScope()).Returns(scope.Object);

            return new AsyncPipeline<int>(scopeFactory.Object, phases);
        }
    }
}
