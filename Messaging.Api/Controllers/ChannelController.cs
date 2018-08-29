using System;
using System.Linq;
using System.Threading.Tasks;
using Messaging.Api.Models;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messaging.Api.Controllers
{
    /// <summary>
    /// Channel management API
    /// </summary>
	[Authorize]
	[Route("api/app/{appId}/channel")]
    public class ChannelController : Controller
    {
        private readonly IApplicationRepository _applicationRepository;

        /// <summary>
        /// Constructor for the dependency injection.
        /// </summary>
        public ChannelController(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        /// <summary>
        /// Returns specified channel.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="channelId">Channel ID</param>
        /// <response code="200">Returns the retrieved channel.</response>
        /// <response code="404">Specified application of channel not found.</response>
        [HttpGet("{channelId}")]
        [ProducesResponseType(typeof(Channel), 200)]
        public async Task<IActionResult> GetChannel(Guid appId, Guid channelId)
        {
            var app = await _applicationRepository.Get(appId);
            if (app == null)
                return NotFound();

            var channel = app.Channels.FirstOrDefault(ch => ch.Id == channelId);
            if (channel == null)
                return NotFound();

            return Ok(channel);
        }

        /// <summary>
        /// Creates a new channel in specified application.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="channel">New channel name and custom data</param>
        /// <response code="201">Returns the created channel.</response>
        /// <response code="400">Malformed request</response>
        /// <response code="404">Specified application not found.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Channel), 201)]
        public async Task<IActionResult> CreateChannel(Guid appId, [FromBody]ChannelUpdate channel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var app = await _applicationRepository.Get(appId);
            if (app == null)
                return NotFound();

            var newChannel = new Channel
            {
                Id = Guid.NewGuid(),
                Name = channel.Name,
                CustomData = channel.CustomData
            };
            app.Channels.Add(newChannel);

            await _applicationRepository.Upsert(app);

            return CreatedAtAction(nameof(GetChannel), new { appId, channelId = newChannel.Id }, newChannel);
        }

        /// <summary>
        /// Changes the name and custom data of the specified channel.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="channelId">Channel ID</param>
        /// <param name="channel">New channel name and custom data</param>
        /// <response code="201">Returns the updated channel.</response>
        /// <response code="400">Malformed request</response>
        /// <response code="404">Specified application or channel not found.</response>
        [HttpPut("{channelId}")]
        [ProducesResponseType(typeof(Channel), 200)]
        public async Task<IActionResult> UpdateChannel(Guid appId, Guid channelId, [FromBody]ChannelUpdate channel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var app = await _applicationRepository.Get(appId);
            if (app == null)
                return NotFound();

            var channelToUpdate = app.Channels.FirstOrDefault(ch => ch.Id == channelId);
            if (channelToUpdate == null)
                return NotFound();

            channelToUpdate.Name = channel.Name;
            channelToUpdate.CustomData = channel.CustomData;

            await _applicationRepository.Upsert(app);

            return Ok(channelToUpdate);
        }

        /// <summary>
        /// Deletes the specified channel.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="channelId">Channel ID</param>
        /// <response code="200">Specified channel has been deleted.</response>
        /// <response code="404">Specified application or channel not found.</response>
		[HttpDelete("{channelId}")]
        public async Task<IActionResult> DeleteChannel(Guid appId, Guid channelId)
		{
		    var app = await _applicationRepository.Get(appId);
		    if (app == null)
		        return NotFound();

		    var channelToDelete = app.Channels.FirstOrDefault(channel => channel.Id == channelId);
		    if (channelToDelete == null)
		        return NotFound();

		    app.Channels.Remove(channelToDelete);

		    await _applicationRepository.Upsert(app);

		    return NoContent();
		}
    }
}
