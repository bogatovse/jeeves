using System;
using System.Threading;
using System.Threading.Tasks;
using Jeeves.Server.Domain;

namespace Jeeves.Server.Workers
{
    public interface IWorker : IDisposable
    {
        Task ExecuteAsync(ChallengeAttempt challengeAttempt, IChallengeAttemptListener listener, CancellationToken cancellationToken);
    }
    
    public class LocalWorker : IWorker
    {
        public Task ExecuteAsync(ChallengeAttempt challengeAttempt, IChallengeAttemptListener listener, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        public void Dispose()
        {
            
        }
    }
}