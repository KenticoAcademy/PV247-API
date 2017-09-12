using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Messaging.Api.Controllers
{
    /// <summary>
    /// Application space management API
    /// </summary>
    [Route("api/[controller]/")]
    public class AppSpaceController : Controller
    {
        private readonly IAppSpaceRepository _appSpaceRepository;

        /// <summary>
        /// Constructor for the dependency injection.
        /// </summary>
        public AppSpaceController(IAppSpaceRepository appSpaceRepository)
        {
            _appSpaceRepository = appSpaceRepository;
        }

        /// <summary>
        /// Retrieves application space.
        /// </summary>
        /// <param name="appId">Application space ID</param>
        /// <response code="200">Returns the retrieved application space.</response>
        /// <response code="400">If application space with given <paramref name="appId"/> doesn't exist.</response>
        [HttpGet("{appId}")]
        [ProducesResponseType(typeof(AppSpace), 200)]
        public async Task<IActionResult> Get(Guid appId)
        {
            var appSpace = await _appSpaceRepository.Get(appId);
            if (appSpace == null)
                return NotFound();

            // TODO: Map to a viewmodel?
            return Ok(appSpace);
        }

        /// <summary>
        /// Creates a new application space.
        /// </summary>
        /// <response code="201">Returns the created application space.</response>
        [HttpPost]
        [ProducesResponseType(typeof(AppSpace), 201)]
        public async Task<IActionResult> Create()
        {
            var appSpace = await _appSpaceRepository.Upsert(new AppSpace
            {
                Id = Guid.NewGuid(),
                Channels = new List<Channel>()
            });

            return Created($"/api/appSpace/{appSpace.Id}", appSpace);
        }

        /// <summary>
        /// Updates channels in specified application space.
        /// </summary>
        /// <param name="appId">Application space ID</param>
        /// <param name="patch">Description of application space udpate</param>
        /// <response code="200">Everything went well.</response>
        /// <response code="400">Application space specified by given <paramref name="appId"/> does not exist.</response>
        /// <response code="404">Provided patch is malformed.</response>
        [HttpPatch("{appId}")]
        [ProducesResponseType(typeof(AppSpace), 200)]
        public async Task<IActionResult> Patch(Guid appId, [FromBody]JsonPatchDocument<AppSpace> patch)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (patch.Operations.Any(operation => !operation.path.StartsWith("/channels")))
                return BadRequest("Unsupported patch");

            var existingApp = await _appSpaceRepository.Get(appId);
            if (existingApp == null)
                return NotFound();

            try
            {
                patch.ApplyTo(existingApp);
            }
            catch
            {
                return BadRequest("Invalid patch");
            }

            var result = await _appSpaceRepository.Upsert(existingApp);

            return Ok(result);
        }
    }
}
