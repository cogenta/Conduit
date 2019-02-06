using Conduit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for the Conduit library
    /// </summary>
    public static class ConduitServiceExtensions
    {
        /// <summary>
        /// Adds the default pipeline for a given type
        /// </summary>
        /// <typeparam name="T">The that is passed through the pipeline</typeparam>
        /// <param name="services">The service collection to bind to</param>
        /// <param name="builderAction">The builder of the pipeline</param>
        /// <returns>The modified service collection</returns>
        public static IServiceCollection AddDefaultPipeline<T>(this IServiceCollection services, Action<IPipelineBuilder<T>> builderAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (builderAction == null)
            {
                throw new ArgumentNullException(nameof(builderAction));
            }

            return services.BuildPipeline(builderAction).AddPipelineServices<T>();
        }

        /// <summary>
        /// Adds a pipeline with the specified name which can be retrieved from IPipelineManger&lt;T&gt;.Get(name)
        /// </summary>
        /// <typeparam name="T">The that is passed through the pipeline</typeparam>
        /// <param name="services">The service collection to bind to</param>
        /// <param name="name">The name of the pipeline</param>
        /// <param name="builderAction">The builder of the pipeline</param>
        /// <returns>The modified service collection</returns>
        public static IServiceCollection AddPipeline<T>(this IServiceCollection services, string name, Action<IPipelineBuilder<T>> builderAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (builderAction == null)
            {
                throw new ArgumentNullException(nameof(builderAction));
            }

            return services.BuildPipeline(builderAction, name).AddPipelineServices<T>();
        }

        /// <summary>
        /// Adds the default pipeline for a given type
        /// </summary>
        /// <typeparam name="T">The that is passed through the pipeline</typeparam>
        /// <param name="services">The service collection to bind to</param>
        /// <param name="builderAction">The builder of the pipeline</param>
        /// <returns>The modified service collection</returns>
        public static IServiceCollection AddDefaultAsyncPipeline<T>(this IServiceCollection services, Action<IAsyncPipelineBuilder<T>> builderAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (builderAction == null)
            {
                throw new ArgumentNullException(nameof(builderAction));
            }

            return services.BuildPipeline(builderAction).AddPipelineServices<T>();
        }

        /// <summary>
        /// Adds a pipeline with the specified name which can be retrieved from IAsyncPipelineManger&lt;T&gt;.Get(name)
        /// </summary>
        /// <typeparam name="T">The that is passed through the pipeline</typeparam>
        /// <param name="services">The service collection to bind to</param>
        /// <param name="name">The name of the pipeline</param>
        /// <param name="builderAction">The builder of the pipeline</param>
        /// <returns>The modified service collection</returns>
        public static IServiceCollection AddAsyncPipeline<T>(this IServiceCollection services, string name, Action<IAsyncPipelineBuilder<T>> builderAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (builderAction == null)
            {
                throw new ArgumentNullException(nameof(builderAction));
            }

            return services.BuildPipeline(builderAction, name).AddPipelineServices<T>();
        }

        private static IServiceCollection BuildPipeline<T>(this IServiceCollection services, Action<IPipelineBuilder<T>> builderAction, string name = null)
        {
            var builder = new PipelineBuilder<T>(services, name ?? Constants.DefaultPipeline);
            builderAction(builder);
            builder.Build();
            return services;
        }

        private static IServiceCollection BuildPipeline<T>(this IServiceCollection services, Action<IAsyncPipelineBuilder<T>> builderAction, string name = null)
        {
            var builder = new AsyncPipelineBuilder<T>(services, name ?? Constants.DefaultPipeline);
            builderAction(builder);
            builder.Build();
            return services;
        }

        private static IServiceCollection AddPipelineServices<T>(this IServiceCollection services)
        {
            services.TryAddSingleton<IPipelineManager<T>, PipelineManager<T>>();
            services.TryAddSingleton<IAsyncPipelineManager<T>, AsyncPipelineManager<T>>();
            services.TryAddSingleton(sp => sp.GetRequiredService<IPipelineManager<T>>().Get(Constants.DefaultPipeline));
            services.TryAddSingleton(sp => sp.GetRequiredService<IAsyncPipelineManager<T>>().Get(Constants.DefaultPipeline));

            return services;
        }
    }
}
