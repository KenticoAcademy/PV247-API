using System.Threading.Tasks;
using Messaging.Contract.Models;

namespace Messaging.Contract.Repositories
{
    public interface IUserRepository
    {
        Task<User> Get(string email);

        Task<User> Upsert(User user);
    }
}