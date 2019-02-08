using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Conduit
{
    internal sealed class PipelineManager<T> : IPipelineManager<T>
    {
        private readonly IOptionsMonitor<PipelineOptions<T>> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PipelineManager(IOptionsMonitor<PipelineOptions<T>> options, IServiceScopeFactory serviceScopeFactory)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public IPipeline<T> Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = Constants.DefaultPipeline;
            }

            var options = _options.Get(name) ?? throw new Exception("Cant find it");

            return new Pipeline<T>(_serviceScopeFactory, options.Phases);
        }
    }
}
