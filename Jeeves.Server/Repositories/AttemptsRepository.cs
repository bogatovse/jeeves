using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jeeves.Server.Domain;

namespace Jeeves.Server.Repositories
{
    public interface IAttemptsRepository
    {
        Task<IEnumerable<ChallengeAttempt>> GetAttemptsAsync();
        Task<ChallengeAttempt> FindAttemptAsync(Guid attemptId);
        Task DeleteAttemptAsync(Guid attemptId);
        Task<ChallengeAttempt> CreateAttemptAsync(ChallengeAttempt challengeAttempt);
    }
    
    public class AttemptsRepository : IAttemptsRepository
    {
        public Task<IEnumerable<ChallengeAttempt>> GetAttemptsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ChallengeAttempt> FindAttemptAsync(Guid attemptId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAttemptAsync(Guid attemptId)
        {
            throw new NotImplementedException();
        }

        public Task<ChallengeAttempt> CreateAttemptAsync(ChallengeAttempt challengeAttempt)
        {
            throw new NotImplementedException();
        }
    }
}