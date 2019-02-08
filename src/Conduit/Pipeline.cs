using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Conduit
{
    internal sealed class Pipeline<T> : IPipeline<T>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly List<Func<IServiceProvider, T, T>> _types;

        public Pipeline(IServiceScopeFactory serviceScopeFactory, IEnumerable<Func<IServiceProvider, T, T>> phases)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _types = (phases ?? throw new ArgumentNullException(nameof(phases))).ToList();
        }

        public T Execute(T input)
        {
            var current = input;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                foreach (var type in _types)
                {
                    current = type(scope.ServiceProvider, current);
                }
            }

            return current;
        }
    }
}
