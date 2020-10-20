using System;

namespace Jeeves.Server.Domain
{
    public class ChallengeAttempt
    {
        public Guid Id { get; set; }
        public ChallengeAttemptStatus Status { get; set; }
        public byte[] Solution { get; set; }
    }
}