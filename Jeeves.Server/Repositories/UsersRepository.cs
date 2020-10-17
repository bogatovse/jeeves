using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jeeves.Server.Domain;
using Microsoft.EntityFrameworkCore;

namespace Jeeves.Server.Repositories
{
    public interface IUsersRepository
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> FindUserAsync(Guid userId);
        Task DeleteUserAsync(Guid userId);
        Task<User> CreateUserAsync(User user);
    }
    
    public class UsersRepository : DbContext, IUsersRepository
    {
        public Task<IEnumerable<User>> GetUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> FindUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<User> CreateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}