namespace Conduit
{
    /// <summary>
    /// Manages the creation of pipelines for multiple types
    /// </summary>
    /// <typeparam name="T">The that is passed through the pipeline</typeparam>
    public interface IPipelineManager<T>
    {
        /// <summary>
        /// Returns the pipeline with the specified name
        /// </summary>
        /// <param name="name">The name of the pipeline</param>
        /// <returns>The asynchronous pipeline</returns>
        IPipeline<T> Get(string name);
    }
}
