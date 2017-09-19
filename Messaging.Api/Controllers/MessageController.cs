using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Messaging.Api.ViewModels;
using Messaging.Contract.Models;
using Messaging.Contract.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messaging.Api.Controllers
{
    /// <summary>
    /// Message management API
    /// </summary>
    [Authorize]
    [Route("api/app/{appId}/channel/{channelId}/message")]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;

        /// <summary>
        /// Constructor for the dependency injection
        /// </summary>
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        /// <summary>
        /// Returns messages from specified channel of specified application.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="channelId">Channel ID</param>
        /// <param name="lastN">The amount of latest messages that should be returned.</param>
        /// <response code="200">Returns the retrieved messages.</response>
        /// <response code="404">Specified channel not found.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Message>), 200)]
        public async Task<IActionResult> Get(Guid appId, Guid channelId, [FromQuery] int lastN)
        {
            var messages = await _messageService.GetAll(HttpContext.GetCurrentUserId(), appId, channelId, lastN);
            if (messages == null)
                return NotFound("Channel not found");

            return Ok(messages);
        }

        /// <summary>
        /// Creates a new message in specified channel of specified application.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="channelId">Channel ID</param>
        /// <param name="message">The message text and metadata</param>
        /// <response code="201">Returns the created message.</response>
        /// <response code="404">Specified channel not found.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Message), 201)]
        public async Task<IActionResult> Create(Guid appId, Guid channelId, [FromBody] EditedMessage message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var messageDto = new Message
            {
                Value = message.Value,
                CustomData = message.Value
            };

            var created = await _messageService.Create(HttpContext.GetCurrentUserId(), appId, channelId, messageDto);
            if (created == null)
                return NotFound("Channel not found");

            return Created($"/api/app/{appId}/channel/{channelId}/message", created);
        }

        /// <summary>
        /// Changes the text and metadata of specified message.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="channelId">Channel ID</param>
        /// <param name="messageId">Message ID</param>
        /// <param name="messageUpdate">The message text and metadata</param>
        /// <response code="200">Return the updated message.</response>
        /// <response code="404">Specified channel or message not found.</response>
        [HttpPut("{messageId}")]
        [ProducesResponseType(typeof(Message), 200)]
        public async Task<IActionResult> Edit(Guid appId, Guid channelId, Guid messageId, [FromBody] EditedMessage messageUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUser = HttpContext.GetCurrentUserId();
            var message = await _messageService.Get(currentUser, appId, channelId, messageId);
            if (message == null)
                return NotFound("Channel or message not found");

            message.Value = messageUpdate.Value;
            message.CustomData = messageUpdate.CustomData;

            var updated = await _messageService.Edit(currentUser, appId, channelId, message);
            if (updated == null)
                return NotFound("Channel or message not found");

            return Ok(updated);
        }

        /// <summary>
        /// Deletes the specified message.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="channelId">Channel ID</param>
        /// <param name="messageId">Message ID</param>
        /// <response code="200">Specified message has been deleted.</response>
        /// <response code="404">Specified channel or message not found.</response>
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> Delete(Guid appId, Guid channelId, Guid messageId)
        {
            var wasDeleted = await _messageService.Delete(HttpContext.GetCurrentUserId(), appId, channelId, messageId);
            if (!wasDeleted)
                return NotFound("Channel or message not found");

            return Ok();
        }
    }
}