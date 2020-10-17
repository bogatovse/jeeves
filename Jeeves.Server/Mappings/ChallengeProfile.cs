using AutoMapper;
using Jeeves.Server.Domain;
using Jeeves.Server.Representations;

namespace Jeeves.Server.Mappings
{
    public class ChallengeProfile : Profile
    {
        public ChallengeProfile()
        {
            CreateMap<Challenge, JeevesChallenge>();
        }
    }
}