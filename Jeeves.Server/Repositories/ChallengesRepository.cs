using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jeeves.Server.Domain;

namespace Jeeves.Server.Repositories
{
    public interface IChallengesRepository
    {
        Task<IEnumerable<Challenge>> GetChallengesAsync();
        Task<Challenge> FindChallengeAsync(Guid challengeId);
        Task DeleteChallengeAsync(Guid challengeId);
        Task<Challenge> CreateChallengeAsync(Challenge challenge);
    }
    
    public class ChallengesRepository : IChallengesRepository
    {
        public Task<IEnumerable<Challenge>> GetChallengesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Challenge> FindChallengeAsync(Guid challengeId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteChallengeAsync(Guid challengeId)
        {
            throw new NotImplementedException();
        }

        public Task<Challenge> CreateChallengeAsync(Challenge challenge)
        {
            throw new NotImplementedException();
        }
    }
}