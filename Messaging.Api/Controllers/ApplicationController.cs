using System;
using System.Threading.Tasks;
using Messaging.Api.Models;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messaging.Api.Controllers
{
    /// <summary>
    /// Application management API
    /// </summary>
    [Route("api/app/")]
    public class ApplicationController : Controller
    {
        private readonly IApplicationRepository _applicationRepository;

        /// <summary>
        /// Constructor for the dependency injection.
        /// </summary>
        public ApplicationController(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        /// <summary>
        /// Retrieves an application metadata.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <response code="200">Returns the retrieved application.</response>
        /// <response code="404">If application with given <paramref name="appId"/> doesn't exist.</response>
        [HttpGet("{appId}")]
        [ProducesResponseType(typeof(ApplicationResponse), 200)]
        public async Task<IActionResult> Get(Guid appId)
        {
            var app = await _applicationRepository.Get(appId);
            if (app == null)
                return NotFound();

            return Ok(GetResponseModel(app));
        }

        /// <summary>
        /// Creates a new application.
        /// </summary>
        /// <param name="applicationMetadata">Application custom data</param>
        /// <response code="201">Returns the created application.</response>
        /// <response code="400">Malformed request</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApplicationResponse), 201)]
        public async Task<IActionResult> Create([FromBody]ApplicationUpdate applicationMetadata)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var app = await _applicationRepository.Upsert(new Application
            {
                Id = Guid.NewGuid(),
                CustomData = applicationMetadata.CustomData
            });

            return CreatedAtAction(nameof(Get), new { appId = app.Id }, GetResponseModel(app));
        }

        /// <summary>
        /// Updates the metadata of specified application.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="applicationMetadata">New application custom data</param>
        /// <response code="200">Returns the updated application.</response>
        /// <response code="404">If application with given <paramref name="appId"/> doesn't exist.</response>
        [Authorize]
        [HttpPut("{appId}")]
        [ProducesResponseType(typeof(ApplicationResponse), 200)]
        public async Task<IActionResult> Update(Guid appId, [FromBody] ApplicationUpdate applicationMetadata)
        {
            var app = await _applicationRepository.Get(appId);
            if (app == null)
                return NotFound();

            app.CustomData = applicationMetadata.CustomData;

            var updatedApp = await _applicationRepository.Upsert(app);

            return Ok(GetResponseModel(updatedApp));
        }

        private ApplicationResponse GetResponseModel(Application application) =>
            new ApplicationResponse
            {
                Id = application.Id,
                CustomData = application.CustomData
            };
    }
}
