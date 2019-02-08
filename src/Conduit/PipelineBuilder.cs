using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Conduit
{
    internal sealed class PipelineBuilder<T> : IPipelineBuilder<T>
    {
        private readonly IServiceCollection _services;
        private readonly string _name;
        private readonly List<Func<IServiceProvider, T, T>> _phases = new List<Func<IServiceProvider, T, T>>();

        public PipelineBuilder(IServiceCollection services, string name)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _name = name ?? throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(_name))
            {
                throw new ArgumentException("Cannot have a pipeline with an empty name");
            }
        }

        public IPipelineBuilder<T> AddDefaultPipeline(Func<T, bool> condition = null)
            => AddPipeline(null, condition);

        public IPipelineBuilder<T> AddPhase<TPhase>(Func<T, bool> condition = null, ServiceLifetime? lifetime = null) 
            where TPhase : class, IPipelinePhase<T>
        {
            var type = typeof(TPhase);

            if (lifetime.HasValue)
            {
                _services.Add(ServiceDescriptor.Describe(type, type, lifetime.Value));
            }

            _phases.Add((scope, input) =>
            {
                if (condition == null || condition(input))
                {
                    return ((IPipelinePhase<T>)scope.GetRequiredService(type)).Process(input);
                }

                return input;
            });
            return this;
        }

        public IPipelineBuilder<T> AddPhaseFromExpression(Func<T, T> action, Func<T, bool> condition = null)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _phases.Add((scope, input) =>
            {
                if (condition == null || condition(input))
                {
                    return action(input);
                }

                return input;
            });

            return this;
        }

        public IPipelineBuilder<T> AddPipeline(string name, Func<T, bool> condition = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = Constants.DefaultPipeline;
            }

            _phases.Add((scope, input) =>
            {
                if (condition == null || condition(input))
                {
                    return scope.GetRequiredService<IPipelineManager<T>>().Get(name).Execute(input);
                }

                return input;
            });

            return this;
        }

        internal List<Func<IServiceProvider, T, T>> Build()
        {
            _services.Configure<PipelineOptions<T>>(_name, options =>
            {
                options.Phases = _phases;
            });
            return _phases;
        }
    }
}
