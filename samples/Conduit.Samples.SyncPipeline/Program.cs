using Microsoft.Extensions.DependencyInjection;
using System;

namespace Conduit.Samples.SyncPipeline
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection()
                                    .AddPipeline<int>(Constants.Pipeline1, builder =>
                                    {
                                        builder.AddPhase<Interpreter1>(input => input > 0, ServiceLifetime.Singleton)
                                               .AddPhaseFromExpression(input => input + 1)
                                               .AddDefaultPipeline();
                                    })
                                    .AddDefaultPipeline<int>(builder =>
                                    {
                                        builder.AddPhase<Interpreter2>(input => input > 5, ServiceLifetime.Singleton);
                                    })
                                    .AddPipeline<double>(Constants.Pipeline2, builder =>
                                    {
                                        builder.AddPhase<Interpreter1>(lifetime: ServiceLifetime.Singleton);
                                    })
                                    .BuildServiceProvider();

            Console.WriteLine(services.GetRequiredService<IPipelineManager<int>>().Get(Constants.Pipeline1).Execute(5));
            Console.WriteLine(services.GetRequiredService<IPipeline<int>>().Execute(5));
            Console.WriteLine(services.GetRequiredService<IPipelineManager<double>>().Get(Constants.Pipeline2).Execute(5));
        }
    }

    class Constants
    {
        public const string Pipeline1 = nameof(Pipeline1);
        public const string Pipeline2 = nameof(Pipeline2);
    }

    class Interpreter1 : IPipelinePhase<int>, IPipelinePhase<double>
    {
        public int Process(int input)
        {
            return ++input;
        }

        public double Process(double input)
        {
            return input / 2;
        }
    }

    class Interpreter2 : IPipelinePhase<int>
    {
        public int Process(int input)
        {
            return input * 2;
        }
    }
}
