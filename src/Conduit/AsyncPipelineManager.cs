using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Conduit
{
    internal sealed class AsyncPipelineManager<T> : IAsyncPipelineManager<T>
    {
        private readonly IOptionsMonitor<AsyncPipelineOptions<T>> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AsyncPipelineManager(IOptionsMonitor<AsyncPipelineOptions<T>> options, IServiceScopeFactory serviceScopeFactory)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public IAsyncPipeline<T> Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = Constants.DefaultPipeline;
            }

            var options = _options.Get(name) ?? throw new Exception("Cant find it");

            return new AsyncPipeline<T>(_serviceScopeFactory, options.Phases);
        }
    }
}
