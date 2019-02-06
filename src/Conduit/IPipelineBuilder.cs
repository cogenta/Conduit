using Microsoft.Extensions.DependencyInjection;
using System;

namespace Conduit
{
    /// <summary>
    /// Builder interface for constructing a pipeline of the specified type
    /// </summary>
    /// <typeparam name="T">The that is passed through the pipeline</typeparam>
    public interface IPipelineBuilder<T>
    {
        /// <summary>
        /// Adds a delegate as a phase in the pipeline
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <param name="condition">Only execute when the specified condition is met</param>
        /// <returns>The modified pipeline builder</returns>
        IPipelineBuilder<T> AddPhaseFromExpression(Func<T, T> action, Func<T, bool> condition = null);

        /// <summary>
        /// Adds a phase of the specified type
        /// </summary>
        /// <typeparam name="TPhase">The type of phase to add</typeparam>
        /// <param name="condition">Only execute when the specified condition is met</param>
        /// <param name="lifetime">optionally binds the phase with the specified lifetime</param>
        /// <returns>The modified pipeline builder</returns>
        IPipelineBuilder<T> AddPhase<TPhase>(Func<T, bool> condition = null, ServiceLifetime? lifetime = null) 
            where TPhase : class, IPipelinePhase<T>;

        /// <summary>
        /// Adds an existing pipeline to this pipeline
        /// </summary>
        /// <param name="name">The name of the pipeline to add</param>
        /// <param name="condition">Only execute when the specified condition is met</param>
        /// <returns>The modified pipeline builder</returns>
        IPipelineBuilder<T> AddPipeline(string name, Func<T, bool> condition = null);

        /// <summary>
        /// Adds the default pipeline to this pipeline
        /// </summary>
        /// <param name="condition">Only execute when the specified condition is met</param>
        /// <returns>The modified pipeline builder</returns>
        IPipelineBuilder<T> AddDefaultPipeline(Func<T, bool> condition = null);
    }
}
