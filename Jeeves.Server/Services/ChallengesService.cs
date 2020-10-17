using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jeeves.Server.Domain;
using Jeeves.Server.Repositories;

namespace Jeeves.Server.Services
{
    public interface IChallengesService
    {
        Task<IEnumerable<Challenge>> GetChallengesAsync();
        Task<Challenge> FindChallengeAsync(Guid challengeId);
        Task DeleteChallengeAsync(Guid challengeId);
        Task<Challenge> CreateChallengeAsync(Challenge challenge);
    }
    
    public class ChallengesService : IChallengesService
    {
        private readonly IChallengesRepository _challengesRepository;
        
        public ChallengesService(IChallengesRepository challengesRepository)
        {
            _challengesRepository = challengesRepository;
        }
        
        public Task<IEnumerable<Challenge>> GetChallengesAsync()
        {
            return _challengesRepository.GetChallengesAsync();
        }

        public Task<Challenge> FindChallengeAsync(Guid challengeId)
        {
            return _challengesRepository.FindChallengeAsync(challengeId);
        }

        public Task DeleteChallengeAsync(Guid challengeId)
        {
            return _challengesRepository.DeleteChallengeAsync(challengeId);
        }

        public Task<Challenge> CreateChallengeAsync(Challenge challenge)
        {
            return _challengesRepository.CreateChallengeAsync(challenge);
        }
    }
}