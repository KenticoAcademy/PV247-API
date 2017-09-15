using System;
using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Services;
using Microsoft.AspNetCore.Mvc;

namespace Messaging.Api.Controllers
{
    [Route("api/app/{appId}/channel/{channelId}/message")]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid appId, Guid channelId, [FromQuery] int lastN)
        {
            var messages = await _messageService.GetAll(appId, channelId, lastN);
            if (messages == null)
                return NotFound("Channel not found");

            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid appId, Guid channelId, [FromBody] Message message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _messageService.Create(appId, channelId, message);
            if (created == null)
                return NotFound("Channel not found");

            return Created($"/api/app/{appId}/channel/{channelId}/message", created);
        }

        [HttpPut("{messageId}")]
        public async Task<IActionResult> Edit(Guid appId, Guid channelId, Guid messageId, [FromBody] Message message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            message.Id = messageId;

            var updated = await _messageService.Edit(appId, channelId, message);
            if (updated == null)
                return NotFound("Channel or message not found");

            return Ok(updated);
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> Delete(Guid appId, Guid channelId, Guid messageId)
        {
            var wasDeleted = await _messageService.Delete(appId, channelId, messageId);
            if (!wasDeleted)
                return NotFound("Channel or message not found");

            return Ok();
        }
    }
}