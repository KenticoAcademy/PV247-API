using System.Threading.Tasks;
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

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string userId)
        {
            var user = await _userRepository.Get(userId);

            // TODO: Map to a viewmodel
            return Ok(user);
        }
    }
}
