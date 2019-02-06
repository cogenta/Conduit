namespace Conduit
{
    /// <summary>
    /// Represents an execution pipeline
    /// </summary>
    /// <typeparam name="T">The that is passed through the pipeline</typeparam>
    public interface IPipeline<T>
    {
        /// <summary>
        /// Executes all the phases in the pipeline
        /// </summary>
        /// <param name="input">The value to pass to the pipeline</param>
        /// <returns>The modified input</returns>
        T Execute(T input);
    }
}
