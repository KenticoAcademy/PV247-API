using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messaging.Api.Models;
using Messaging.Contract.Models;
using Messaging.Contract.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Messaging.Api.Controllers
{
    /// <summary>
    /// File management API
    /// </summary>
    [Authorize]
    [Route("api/file")]
    public class FileController : Controller
    {
        private readonly IFileService _fileService;

        /// <summary>
        /// Constructor for the dependency injection
        /// </summary>
        /// <param name="fileService"></param>
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Uploads a new file and creates corresponding file metadata.
        /// </summary>
        /// <param name="files">Files from the file input.</param>
        /// <response code="201">Returns metadata for the uploaded files.</response>
        /// <response code="400">Malformed request</response>
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<FileMetadata>), 201)]
        public async Task<ActionResult> UploadFile(IEnumerable<IFormFile> files)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var uploadedFiles = new List<FileMetadata>();

            foreach (var formFile in files.Where(f => f.Length > 0))
            {
                using (var stream = formFile.OpenReadStream())
                {
                    var metadata = await _fileService.UploadFile(HttpContext.GetCurrentUserId(), formFile.FileName, formFile.Length, stream);
                    uploadedFiles.Add(metadata);
                }
            }

            return Accepted(uploadedFiles);
        }

        /// <summary>
        /// Returns metadata for the specified file.
        /// </summary>
        /// <param name="fileId">File ID</param>
        /// <response code="200">Returns metadata for specified file.</response>
        /// <response code="400">Malformed request</response>
        /// <response code="404">Specified file doesn't exist.</response>
        [HttpGet("{fileId}")]
        [ProducesResponseType(typeof(FileMetadata), 200)]
        public async Task<ActionResult> GetMetadata(Guid fileId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var metadata = await _fileService.GetFileMetadata(fileId);
            if (metadata == null)
                return NotFound("File not found");

            return Ok(metadata);
        }

        /// <summary>
        /// Returns download URL for the specified file.
        /// </summary>
        /// <param name="fileId">File ID</param>
        /// <response code="200">Returns the download URL.</response>
        /// <response code="400">Malformed request</response>
        /// <response code="404">Specified file doesn't exist.</response>
        [HttpGet("{fileId}/download-link")]
        [ProducesResponseType(typeof(DownloadLinkResponse), 200)]
        public async Task<ActionResult> GetDownloadLink(Guid fileId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var uri = await _fileService.GetDownloadUrl(fileId);
            if (uri == null)
                return NotFound("File not found");

            return Ok(new DownloadLinkResponse { FileUri = uri });
        }
    }
}