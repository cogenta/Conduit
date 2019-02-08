using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Conduit
{
    internal sealed class AsyncPipelineBuilder<T> : IAsyncPipelineBuilder<T>
    {
        private readonly IServiceCollection _services;
        private readonly string _name;
        private readonly List<Func<IServiceProvider, T, CancellationToken, Task<T>>> _phases = new List<Func<IServiceProvider, T, CancellationToken, Task<T>>>();

        public AsyncPipelineBuilder(IServiceCollection services, string name)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _name = name ?? throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(_name))
            {
                throw new ArgumentException("Cannot have a pipeline with an empty name");
            }
        }

        public IAsyncPipelineBuilder<T> AddDefaultPipeline(Func<T, bool> condition = null)
            => AddAsyncPipeline(null, condition);

        public IAsyncPipelineBuilder<T> AddPhase<TPhase>(Func<T, bool> condition, ServiceLifetime? lifetime)
            where TPhase : class, IAsyncPipelinePhase<T>
        {
            var type = typeof(TPhase);

            if (lifetime.HasValue)
            {
                _services.Add(ServiceDescriptor.Describe(type, type, lifetime.Value));
            }

            _phases.Add((scope, input, cancellationToken) =>
            {
                if (condition == null || condition(input))
                {
                    return ((IAsyncPipelinePhase<T>)scope.GetRequiredService(type)).ProcessAsync(input, cancellationToken);
                }

                return Task.FromResult(input);
            });
            return this;
        }

        public IAsyncPipelineBuilder<T> AddPhaseFromExpression(Func<T, CancellationToken, Task<T>> action, Func<T, bool> condition = null)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _phases.Add((scope, input, cancellationToken) =>
            {
                if (condition == null || condition(input))
                {
                    return action(input, cancellationToken);
                }

                return Task.FromResult(input);
            });

            return this;
        }

        public IAsyncPipelineBuilder<T> AddPipeline(string name, Func<T, bool> condition = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = Constants.DefaultPipeline;
            }

            _phases.Add((scope, input, cancellationToken) =>
            {
                if (condition == null || condition(input))
                {
                    return Task.FromResult(scope.GetRequiredService<IPipelineManager<T>>().Get(name).Execute(input));
                }

                return Task.FromResult(input);
            });

            return this;
        }

        public IAsyncPipelineBuilder<T> AddAsyncPipeline(string name, Func<T, bool> condition = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = Constants.DefaultPipeline;
            }

            _phases.Add((scope, input, cancellationToken) =>
            {
                if (condition == null || condition(input))
                {
                    return scope.GetRequiredService<IAsyncPipelineManager<T>>().Get(name).ExecuteAsync(input, cancellationToken);
                }

                return Task.FromResult(input);
            });

            return this;
        }

        internal List<Func<IServiceProvider, T, CancellationToken, Task<T>>> Build()
        {
            _services.Configure<AsyncPipelineOptions<T>>(_name, options =>
            {
                options.Phases = _phases;
            });
            return _phases;
        }
    }
}
