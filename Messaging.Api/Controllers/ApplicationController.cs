using System;
using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
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
    }
}
