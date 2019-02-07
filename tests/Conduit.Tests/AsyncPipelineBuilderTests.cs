using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Conduit.Tests
{
    public class AsyncPipelineBuilderTests
    {
        public class AddDefaultPipeline : BuilderTestBase
        {
            [Fact]
            public void AddsToBuiltPhases()
            {
                var target = CreateTarget(out var services);

                target.AddDefaultPipeline();

                VerifyPhases(target, p => p.Count == 1);
            }
        }

        public class AddPhase : BuilderTestBase
        {
            [Fact]
            public void WhenServiceLifetimeSuppliedAddToServiceCollection()
            {
                var target = CreateTarget(out var services);
                var lifetime = ServiceLifetime.Scoped;

                target.AddPhase<FakePhase>(lifetime: lifetime);

                services.Verify(x => x.Add(It.Is<ServiceDescriptor>(y => y.ServiceType == typeof(FakePhase) && y.ImplementationType == typeof(FakePhase) && y.Lifetime == lifetime)), Times.Once);
            }

            [Fact]
            public void WhenServiceLifetimeNotSuppliedDoNotAddToServiceCollection()
            {
                var target = CreateTarget(out var services);

                target.AddPhase<FakePhase>();

                services.Verify(x => x.Add(It.IsAny<ServiceDescriptor>()), Times.Never);
            }

            [Fact]
            public void AddsToBuiltPhases()
            {
                var target = CreateTarget(out var services);

                target.AddPhase<FakePhase>();

                VerifyPhases(target, p => p.Count == 1);
            }
        }

        public class AddPhaseFromExpression : BuilderTestBase
        {
            [Fact]
            public void AddsToBuiltPhases()
            {
                var target = CreateTarget(out var services);

                target.AddPhaseFromExpression((input, ct) => Task.FromResult(input + 1));

                VerifyPhases(target, p => p.Count == 1);
            }

            [Fact]
            public void WhenActionIsNullThrowArgumentNullException()
            {
                var target = CreateTarget(out var services);

                Action act = () => target.AddPhaseFromExpression(null);

                act.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        public class AddPipeline : BuilderTestBase
        {
            [Fact]
            public void AddsToBuiltPhases()
            {
                var target = CreateTarget(out var services);

                target.AddPipeline("test");

                VerifyPhases(target, p => p.Count == 1);
            }
        }

        public class AddAsyncPipeline : BuilderTestBase
        {
            [Fact]
            public void AddsToBuiltPhases()
            {
                var target = CreateTarget(out var services);

                target.AddAsyncPipeline("test");

                VerifyPhases(target, p => p.Count == 1);
            }
        }

        class FakePhase : IAsyncPipelinePhase<int>
        {
            public Task<int> ProcessAsync(int input, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        public abstract class BuilderTestBase
        {
            protected IAsyncPipelineBuilder<int> CreateTarget(out Mock<IServiceCollection> services, string name = null)
            {
                var lst = new List<ServiceDescriptor>();
                services = new Mock<IServiceCollection>();
                services.Setup(x => x.GetEnumerator()).Returns(() => lst.GetEnumerator());
                services.Setup(x => x.Add(It.IsAny<ServiceDescriptor>())).Callback((ServiceDescriptor sd) => lst.Add(sd));
                return new AsyncPipelineBuilder<int>(services.Object, name ?? Constants.DefaultPipeline);
            }

            protected void VerifyPhases(IAsyncPipelineBuilder<int> builder, Func<List<Func<IServiceProvider, int, CancellationToken, Task<int>>>, bool> verification)
            {
                // Workaround for exposing an internal type
                if (!(builder is AsyncPipelineBuilder<int> asyncBuilder))
                    throw new Exception($"Builder must be of type {nameof(AsyncPipelineBuilder<int>)}");

                verification(asyncBuilder.Build()).Should().BeTrue();
            }
        }
    }
}
