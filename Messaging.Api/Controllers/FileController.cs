using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Messaging.Api.Controllers
{
    [Authorize]
    [Route("api/file")]
    public class FileController : Controller
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
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

            return Created("/api/file/{fileId}", uploadedFiles);
        }

        [HttpGet("{fileId}")]
        public async Task<ActionResult> GetMetadata(Guid fileId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var metadata = await _fileService.GetFileMetadata(fileId);

            return Ok(metadata);
        }

        [HttpGet("{fileId}/download-link")]
        public async Task<ActionResult> GetDownloadLink(Guid fileId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var uri = await _fileService.GetDownloadUrl(fileId);

            return Ok(uri);
        }
    }
}