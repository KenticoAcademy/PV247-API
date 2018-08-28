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

        [HttpGet("{channelId}")]
        public async Task<IActionResult> GetChannel(Guid appId, Guid channelId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var app = await _applicationRepository.Get(appId);
            if (app == null)
                return NotFound();

            var channel = app.Channels.FirstOrDefault(ch => ch.Id == channelId);
            if (channel == null)
                return NotFound();

            return Ok(channel);
        }

        [HttpPost]
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

        [HttpPut("{channelId}")]
        public async Task<IActionResult> UpdateChannel(Guid appId, Guid channelId, [FromBody] ChannelUpdate channel)
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
