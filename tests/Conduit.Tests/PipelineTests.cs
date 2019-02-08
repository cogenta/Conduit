using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Conduit.Tests
{
    public class PipelineTests
    {
        [Fact]
        public void WhenNoPhasesInputIsNotModified()
        {
            var target = CreateTarget(Enumerable.Empty<Func<IServiceProvider, int, int>>(), out _);

            target.Execute(1).Should().Be(1);
        }

        [Fact]
        public void WhenMultiplePhasesPresentInputIsModified()
        {
            Func<IServiceProvider, int, int> phase = (sp, input) => input + 1;
            var phases = Enumerable.Repeat(phase, 4);
            var target = CreateTarget(phases, out _);

            target.Execute(1).Should().Be(5);
        }

        private static Pipeline<int> CreateTarget(IEnumerable<Func<IServiceProvider, int, int>> phases, out Mock<IServiceProvider> serviceProvider)
        {
            serviceProvider = new Mock<IServiceProvider>();

            var scopeFactory = new Mock<IServiceScopeFactory>();
            var scope = new Mock<IServiceScope>();
            scope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
            scopeFactory.Setup(x => x.CreateScope()).Returns(scope.Object);

            return new Pipeline<int>(scopeFactory.Object, phases);
        }
    }
}
