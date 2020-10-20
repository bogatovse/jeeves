using AutoMapper;
using Jeeves.Server.Domain;
using Jeeves.Server.Representations;

namespace Jeeves.Server.Mappings
{
    public class ChallengeAttemptProfile : Profile
    {
        public ChallengeAttemptProfile()
        {
            CreateMap<ChallengeAttempt, JeevesAttempt>();
        }
    }
}