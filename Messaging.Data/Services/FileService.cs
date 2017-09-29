using System;
using System.IO;
using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Messaging.Contract.Services;

namespace Messaging.Data.Services
{
    internal class FileService : IFileService
    {
        private readonly IFileMetadataRepository _metadataRepository;
        private readonly IFileBlobRepository _blobRepository;

        public FileService(IFileBlobRepository blobRepository, IFileMetadataRepository metadataRepository)
        {
            _blobRepository = blobRepository;
            _metadataRepository = metadataRepository;
        }

        public async Task<FileMetadata> UploadFile(string userId, string fileName, long fileSize, Stream fileStream)
        {
            var metadata = await _metadataRepository.CreateFileMetadata(new FileMetadata
            {
                Id = Guid.NewGuid(),
                CreatedBy = userId, 
                Extension = Path.GetExtension(fileName),
                FileSize = fileSize,
                Name = fileName
            });

            await  _blobRepository.UploadFile(metadata, fileStream);

            return metadata;
        }

        public async Task<Uri> GetDownloadUrl(Guid fileId)
        {
            var file = await _metadataRepository.GetFileMetadata(fileId);
            if (file == null)
                return null;

            return await _blobRepository.GetDownloadUrl(file);
        }

        public async Task<FileMetadata> GetFileMetadata(Guid fileId)
        {
            return await _metadataRepository.GetFileMetadata(fileId);
        }
    }
}