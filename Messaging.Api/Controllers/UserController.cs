using System;
using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Messaging.Api.Controllers
{
    /// <summary>
    /// User management API
    /// </summary>
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Constructor for the dependency injection.
        /// </summary>
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Retrieves metadata for user with given <paramref name="email"/>.
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <response code="200">Returns the retrieved user metadata.</response>
        /// <response code="400">If user with given <paramref name="email"/> doesn't exist.</response>
        [HttpGet("{email}")]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Get(string email)
        {
            var user = await _userRepository.Get(email);
            if (user == null)
            {
                return NotFound();
            }

            // TODO: Map to a viewmodel
            return Ok(user);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> Upsert(Guid userId, [FromBody]string email)
        {
            var user = new User
            {
                Id = userId,
                Email = email
            };
            var result = await _userRepository.Upsert(user);

            return Created($"api/{userId}", result);
        }
    }
}
