using System.Threading;
using System.Threading.Tasks;

namespace Jeeves.Server.Workers
{
    public interface IWorkerCreator
    {
        Task<IWorker> CreateWorkerAsync(CancellationToken cancellationToken);
    }

    public class LocalWorkerCreator : IWorkerCreator
    {
        public Task<IWorker> CreateWorkerAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IWorker>(new LocalWorker());
        }
    }
}