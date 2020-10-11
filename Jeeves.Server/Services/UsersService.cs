using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jeeves.Server.Domain;
using Jeeves.Server.Repositories;

namespace Jeeves.Server.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> FindUserAsync(Guid userId);
        Task DeleteUserAsync(Guid userId);
    }
    
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        
        public UsersService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        
        public Task<IEnumerable<User>> GetUsersAsync()
        {
            return _usersRepository.GetUsersAsync();
        }

        public Task<User> FindUserAsync(Guid userId)
        {
            return _usersRepository.FindUserAsync(userId);
        }
        
        public Task DeleteUserAsync(Guid userId)
        {
            return _usersRepository.DeleteUserAsync(userId);
        }
    }
}