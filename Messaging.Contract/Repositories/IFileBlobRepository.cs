using System;
using System.IO;
using System.Threading.Tasks;
using Messaging.Contract.Models;

namespace Messaging.Contract.Repositories
{
    public interface IFileBlobRepository
    {
        Task UploadFile(FileMetadata fileMetadata, Stream fileStream);

        Task<Uri> GetDownloadUrl(FileMetadata fileMetadata);
    }
}