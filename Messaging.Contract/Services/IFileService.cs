using System;
using System.IO;
using System.Threading.Tasks;
using Messaging.Contract.Models;

namespace Messaging.Contract.Services
{
    public interface IFileService
    {
        Task<FileMetadata> UploadFile(string userId, string fileName, long fileSize, Stream fileStream);

        Task<Uri> GetDownloadUrl(Guid fileId);

        Task<FileMetadata> GetFileMetadata(Guid fileId);
    }
}