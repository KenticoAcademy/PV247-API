using System;
using System.Threading.Tasks;
using Messaging.Contract.Models;

namespace Messaging.Contract.Repositories
{
    public interface IApplicationRepository
    {
        Task<Application> Get(Guid appId);

        Task<Application> Upsert(Application app);
    }
}