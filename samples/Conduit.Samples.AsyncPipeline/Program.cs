using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Conduit.Samples.AsyncPipeline
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection()
                                    .AddAsyncPipeline<int>(Constants.Pipeline1, builder =>
                                    {
                                        builder.AddPhase<Interpreter1>(input => input > 0, ServiceLifetime.Singleton)
                                               .AddPhaseFromExpression((input, ct) => Task.FromResult(input + 1))
                                               .AddDefaultPipeline();
                                    })
                                    .AddDefaultAsyncPipeline<int>(builder =>
                                    {
                                        builder.AddPhase<Interpreter2>(input => input > 5, ServiceLifetime.Singleton);
                                    })
                                    .AddAsyncPipeline<double>(Constants.Pipeline2, builder =>
                                    {
                                        builder.AddPhase<Interpreter1>(lifetime: ServiceLifetime.Singleton);
                                    })
                                    .BuildServiceProvider();

            Console.WriteLine(await services.GetRequiredService<IAsyncPipelineManager<int>>().Get(Constants.Pipeline1).ExecuteAsync(5));
            Console.WriteLine(await services.GetRequiredService<IAsyncPipeline<int>>().ExecuteAsync(5));
            Console.WriteLine(await services.GetRequiredService<IAsyncPipelineManager<double>>().Get(Constants.Pipeline2).ExecuteAsync(5));
        }
    }

    class Constants
    {
        public const string Pipeline1 = nameof(Pipeline1);
        public const string Pipeline2 = nameof(Pipeline2);
    }

    class Interpreter1 : IAsyncPipelinePhase<int>, IAsyncPipelinePhase<double>
    {
        public async Task<int> ProcessAsync(int input, CancellationToken cancellationToken)
        {
            await Task.Delay(250);
            return ++input;
        }

        public async Task<double> ProcessAsync(double input, CancellationToken cancellationToken)
        {
            await Task.Delay(250);
            return input / 2;
        }
    }

    class Interpreter2 : IAsyncPipelinePhase<int>
    {
        public async Task<int> ProcessAsync(int input, CancellationToken cancellationToken)
        {
            await Task.Delay(250);
            return input * 2;
        }
    }
}