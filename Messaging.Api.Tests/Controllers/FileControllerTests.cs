﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Messaging.Api.Models;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Messaging.Api.Tests.Controllers
{
    public class FileControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly IFileBlobRepository _fileBlobRepository;
        private readonly IFileMetadataRepository _fileMetadataRepository;

        public FileControllerTests(WebApplicationFactory<Startup> factory)
        {
            _fileBlobRepository = Substitute.For<IFileBlobRepository>();
            _fileMetadataRepository = Substitute.For<IFileMetadataRepository>();

            _factory = factory.WithWebHostBuilder(builder => builder
                .ConfigureTestServices(services => services
                    .AddScoped(_ => _fileBlobRepository)
                    .AddScoped(_ => _fileMetadataRepository)));
        }

        [Fact]
        public async Task UploadFile_Accepted()
        {
            var client = await _factory.CreateAuthenticatedClient();
            _fileMetadataRepository.CreateFileMetadata(Arg.Any<FileMetadata>())
                .Returns(call => call.Arg<FileMetadata>());

            var fileBytes = Encoding.UTF8.GetBytes("test");
            var fileName = "image.png";
            var response = await client.PostAsync("/api/v2/file", new MultipartFormDataContent
            {
                {new ByteArrayContent(fileBytes), "files", fileName},
                {new ByteArrayContent(Encoding.UTF8.GetBytes("test2")), "files", "image2.jpg"}
            });

            var uploadedFiles = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<List<FileMetadata>>();
            Assert.Equal(2, uploadedFiles.Count);

            var file = uploadedFiles.First();
            Assert.Equal(fileName, file.Name);
            Assert.Equal(fileBytes.Length, file.FileSize);
        }

        [Fact]
        public async Task GetMetadata_ExistingFile_Ok()
        {
            var fileId = Guid.NewGuid();
            var client = await _factory.CreateAuthenticatedClient();
            _fileMetadataRepository.GetFileMetadata(fileId)
                .Returns(new FileMetadata {Id = fileId});

            var response = await client.GetAsync($"/api/v2/file/{fileId}");

            var retrievedMetadata = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<FileMetadata>();
            Assert.NotNull(retrievedMetadata);
        }

        [Fact]
        public async Task GetDownloadLink_ExistingFile_Ok()
        {
            var fileId = Guid.NewGuid();
            var fakeBlobUrl = new Uri("http://image.url");
            var client = await _factory.CreateAuthenticatedClient();
            _fileMetadataRepository.GetFileMetadata(fileId)
                .Returns(new FileMetadata {Id = fileId});
            _fileBlobRepository.GetDownloadUrl(Arg.Is<FileMetadata>(metadata => metadata.Id == fileId))
                .Returns(fakeBlobUrl);

            var response = await client.GetAsync($"/api/v2/file/{fileId}/download-link");

            var downloadLink = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<DownloadLinkResponse>();

            Assert.Equal(fakeBlobUrl, downloadLink.FileUri);
        }
    }
}
