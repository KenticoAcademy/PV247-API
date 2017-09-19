using System;
using System.Threading.Tasks;
using Messaging.Contract.Models;

namespace Messaging.Contract.Repositories
{
    public interface IUserRepository
    {
        Task<bool> IsValidUser(string email /*, string passwordHash */);

        Task<User> Get(Guid appId, string email);

        Task<User> Upsert(Guid appId, User user);
    }
}