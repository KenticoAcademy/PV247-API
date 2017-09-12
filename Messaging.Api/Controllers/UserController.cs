using System;
using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Messaging.Api.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> Get(string email)
        {
            var user = await _userRepository.Get(email);

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
