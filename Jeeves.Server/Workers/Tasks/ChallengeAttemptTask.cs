using Jeeves.Server.Domain;

namespace Jeeves.Server.Workers.Tasks
{
    public class ChallengeAttemptTask
    {
        public ChallengeAttempt Attempt { get; set; }
        public IChallengeAttemptListener Listener { get; set; }
    }
}