﻿namespace Conduit
{
    /// <summary>
    /// Manages the creation of pipelines for multiple types
    /// </summary>
    /// <typeparam name="T">The type that is passed through the pipeline</typeparam>
    public interface IAsyncPipelineManager<T>
    {
        /// <summary>
        /// Returns the pipeline with the specified name
        /// </summary>
        /// <param name="name">The name of the pipeline</param>
        /// <returns>The asynchronous pipeline</returns>
        IAsyncPipeline<T> Get(string name);
    }
}
