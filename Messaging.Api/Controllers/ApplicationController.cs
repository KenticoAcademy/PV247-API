using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Messaging.Api.Services;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
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
        private readonly IChannelPatchService _channelPatchService;

        /// <summary>
        /// Constructor for the dependency injection.
        /// </summary>
        public ApplicationController(IApplicationRepository applicationRepository, IChannelPatchService channelPatchService)
        {
            _applicationRepository = applicationRepository;
            _channelPatchService = channelPatchService;
        }

        /// <summary>
        /// Retrieves an application metadata.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <response code="200">Returns the retrieved application.</response>
        /// <response code="404">If application with given <paramref name="appId"/> doesn't exist.</response>
        [HttpGet("{appId}")]
        [ProducesResponseType(typeof(Application), 200)]
        public async Task<IActionResult> Get(Guid appId)
        {
            var app = await _applicationRepository.Get(appId);
            if (app == null)
                return NotFound();

            return Ok(app);
        }

        /// <summary>
        /// Creates a new application.
        /// </summary>
        /// <response code="201">Returns the created application.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Application), 201)]
        public async Task<IActionResult> Create()
        {
            var app = await _applicationRepository.Upsert(new Application
            {
                Id = Guid.NewGuid()
            });

            return CreatedAtAction(nameof(Get), new { appId = app.Id }, app);
        }

        /// <summary>
        /// Updates channels in specified application.
        /// </summary>
        /// <remarks>
        /// Sample patch:
        /// 
        ///     [
        ///       {
        ///         "path": "/channels/-",
        ///         "op": "add",
        ///         "value": {
        ///           "name": "My new channel"
        ///         }
        ///       }
        ///     ]     
        ///
        /// Supported operations are "add", "replace", and "remove".
        /// 
        /// The path can specify the whole "/channels" collection or a single channel by its ID.
        /// With the "add" operation, you can use the "-" character instead of the ID to add the item at the end of the collection.
        /// </remarks>
        /// <param name="appId">Application ID</param>
        /// <param name="patch">Description of application update</param>
        /// <response code="200">Everything went well.</response>
        /// <response code="404">Application specified by given <paramref name="appId"/> does not exist.</response>
        /// <response code="400">Provided patch is malformed.</response>
        [Authorize]
        [HttpPatch("{appId}")]
        [ProducesResponseType(typeof(Application), 200)]
        public async Task<IActionResult> Patch(Guid appId, [FromBody]JsonPatchDocument<Application> patch)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingApp = await _applicationRepository.Get(appId);
            if (existingApp == null)
                return NotFound();

            try
            {
                patch = _channelPatchService.ConvertOperations(patch, existingApp);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                patch.ApplyTo(existingApp);
            }
            catch
            {
                return BadRequest("Invalid patch");
            }

            var result = await _applicationRepository.Upsert(existingApp);

            return Ok(result);
        }
    }
}
