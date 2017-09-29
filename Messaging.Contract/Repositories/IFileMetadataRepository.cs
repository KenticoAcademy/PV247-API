using System;
using System.Threading.Tasks;
using Messaging.Contract.Models;

namespace Messaging.Contract.Repositories
{
    public interface IFileMetadataRepository
    {
        Task<FileMetadata> CreateFileMetadata(FileMetadata fileMetadata);

        Task<FileMetadata> GetFileMetadata(Guid fileId);
    }
}