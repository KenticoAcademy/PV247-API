using System;
using System.Threading.Tasks;
using Messaging.Contract.Models;

namespace Messaging.Contract.Repositories
{
    public interface IAppSpaceRepository
    {
        Task<AppSpace> Get(Guid appId);

        Task<AppSpace> Upsert(AppSpace app);
    }
}