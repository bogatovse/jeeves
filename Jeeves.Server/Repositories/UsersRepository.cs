using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jeeves.Server.Domain;

namespace Jeeves.Server.Repositories
{
    public interface IUsersRepository
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> FindUserAsync(Guid userId);
        Task DeleteUserAsync(Guid userId);
    }
    
    public class UsersRepository : IUsersRepository
    {
        public Task<IEnumerable<User>> GetUsersAsync()
        {
            return Task.FromResult(new []
            {
                new User() {Id = Guid.NewGuid(), FirstName = "Sergey", LastName = "Bogatov"},
                new User() {Id = Guid.NewGuid(), FirstName = "Valera", LastName = "Borch"}
            }.AsEnumerable());
        }

        public Task<User> FindUserAsync(Guid userId)
        {
            return null;
        }

        public Task DeleteUserAsync(Guid userId)
        {
            return Task.CompletedTask;
        }
    }
}