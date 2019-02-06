using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Conduit
{
    internal sealed class AsyncPipelineOptions<T>
    {
        internal IEnumerable<Func<IServiceProvider, T, CancellationToken, Task<T>>> Phases { get; set; } = Enumerable.Empty<Func<IServiceProvider, T, CancellationToken, Task<T>>>();
    }
}
