using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Conduit
{
    internal sealed class PipelineOptions<T>
    {
        internal IEnumerable<Func<IServiceProvider, T, T>> Phases { get; set; } = Enumerable.Empty<Func<IServiceProvider, T, T>>();
    }
}
