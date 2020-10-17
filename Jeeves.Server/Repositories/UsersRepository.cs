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

    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions<UsersRepository> options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
    }
    
    public class UsersRepository : DbContext, IUsersRepository
    {
        private readonly UsersContext _usersContext;
        
        public UsersRepository(UsersContext usersContext)
        {
            _usersContext = usersContext;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _usersContext.Users.ToListAsync(CancellationToken.None);
        }

        public async Task<User> FindUserAsync(Guid userId)
        {
            return await _usersContext.Users.FindAsync(userId);
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            await _usersContext.Users.FindAsync(userId);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var createdUser = await _usersContext.Users.AddAsync(user);
            return createdUser.Entity;
        }
    }
}