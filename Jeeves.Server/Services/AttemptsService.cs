using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jeeves.Server.Domain;
using Jeeves.Server.Repositories;
using Jeeves.Server.Workers;

namespace Jeeves.Server.Services
{
    public interface IAttemptsService
    {
        Task<IEnumerable<ChallengeAttempt>> GetAttemptsAsync();
        Task<ChallengeAttempt> FindAttemptAsync(Guid attemptId);
        Task DeleteAttemptAsync(Guid attemptId);
        Task StopAttemptAsync(Guid attemptId);
        Task<ChallengeAttempt> CreateAttemptAsync(ChallengeAttempt challengeAttempt);
    }
    
    public class AttemptsService : IAttemptsService
    {
        private readonly IWorkersForeman _foreman;
        private readonly IAttemptsRepository _attemptsRepository;

        public AttemptsService(IAttemptsRepository attemptsRepository, IWorkersForeman foreman)
        {
            _foreman = foreman;
            _attemptsRepository = attemptsRepository;
        }
        
        public async Task<ChallengeAttempt> CreateAttemptAsync(ChallengeAttempt challengeAttempt)
        {
            challengeAttempt.Status = ChallengeAttemptStatus.Queued;
            var createdAttempt =  await _attemptsRepository.CreateAttemptAsync(challengeAttempt);
            await _foreman.EnqueueWorkAsync(createdAttempt, null, CancellationToken.None);
            return createdAttempt;
        }
        
        public Task<IEnumerable<ChallengeAttempt>> GetAttemptsAsync()
        {
            return _attemptsRepository.GetAttemptsAsync();
        }

        public Task<ChallengeAttempt> FindAttemptAsync(Guid attemptId)
        {
            return _attemptsRepository.FindAttemptAsync(attemptId);
        }

        public Task DeleteAttemptAsync(Guid attemptId)
        {
            return _attemptsRepository.DeleteAttemptAsync(attemptId);
        }

        public async Task StopAttemptAsync(Guid attemptId)
        {
            await _foreman.StopWorkAsync(attemptId);
        }
    }
}