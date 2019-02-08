using System.Threading;
using System.Threading.Tasks;

namespace Conduit
{
    /// <summary>
    /// Defines a phase in the pipeline in which processing can occur asynchronously
    /// </summary>
    /// <typeparam name="T">The type that is passed through the pipeline</typeparam>
    public interface IAsyncPipelinePhase<T>
    {
        /// <summary>
        /// Executes the processing of the input
        /// </summary>
        /// <param name="input">The input to process</param>
        /// <param name="cancellationToken">A cancellation token from the host pipeline</param>
        /// <returns>The result of the phase, which could be the modified input</returns>
        Task<T> ProcessAsync(T input, CancellationToken cancellationToken);
    }
}
