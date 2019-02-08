using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Conduit
{
    internal sealed class AsyncPipeline<T> : IAsyncPipeline<T>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly List<Func<IServiceProvider, T, CancellationToken, Task<T>>> _types;

        public AsyncPipeline(IServiceScopeFactory serviceScopeFactory, IEnumerable<Func<IServiceProvider, T, CancellationToken, Task<T>>> phases)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _types = (phases ?? throw new ArgumentNullException(nameof(phases))).ToList();
        }

        public async Task<T> ExecuteAsync(T input, CancellationToken cancellationToken)
        {
            var current = input;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                foreach (var type in _types)
                {
                    current = await type(scope.ServiceProvider, current, cancellationToken);
                }
            }

            return current;
        }
    }
}
