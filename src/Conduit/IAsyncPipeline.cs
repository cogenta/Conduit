using System.Threading;
using System.Threading.Tasks;

namespace Conduit
{
    /// <summary>
    /// Represents an asynchronous execution pipeline
    /// </summary>
    /// <typeparam name="T">The that is passed through the pipeline</typeparam>
    public interface IAsyncPipeline<T>
    {
        /// <summary>
        /// Executes all the phases in the pipeline
        /// </summary>
        /// <param name="input">The value to pass to the pipeline</param>
        /// <param name="cancellationToken">A cancellation token to pass to all phases of the pipeline</param>
        /// <returns>The modified input</returns>
        Task<T> ExecuteAsync(T input, CancellationToken cancellationToken = default);
    }
}
