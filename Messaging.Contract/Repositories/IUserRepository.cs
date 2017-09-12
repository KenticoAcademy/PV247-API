using System;
using System.Threading.Tasks;
using Messaging.Contract.Models;

namespace Messaging.Contract.Repositories
{
    public interface IUserRepository
    {
        Task<User> Get(Guid appId, string email);

        Task<User> Upsert(Guid appId, User user);
    }
}