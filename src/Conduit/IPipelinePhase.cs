namespace Conduit
{
    /// <summary>
    /// Defines a phase in the pipeline in which processing can occur
    /// </summary>
    /// <typeparam name="T">The type that is passed through the pipeline</typeparam>
    public interface IPipelinePhase<T>
    {
        /// <summary>
        /// Executes the processing of the input
        /// </summary>
        /// <param name="input">The input to process</param>
        /// <returns>The result of the phase, which could be the modified input</returns>
        T Process(T input);
    }
}
