using AutoMapper;
using Jeeves.Server.Domain;
using Jeeves.Server.Representations;

namespace Jeeves.Server.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, JeevesUser>();
        }
    }
}