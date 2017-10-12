using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Messaging.Contract.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Messaging.Api.Controllers
{
    /// <summary>
    /// Authentication API
    /// </summary>
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthSettings _authSettings;

        /// <summary>
        /// Constructor for the dependency injection
        /// </summary>
        public AuthController(IUserRepository userRepository, IOptions<AuthSettings> settings)
        {
            _userRepository = userRepository;
            _authSettings = settings.Value;
        }

        /// <summary>
        /// Returns token that can be used to authenticate API calls.
        /// </summary>
        /// <param name="email">Email identifying a user.</param>
        /// <response code="200">Returns a token if the authentication was successful.</response>
        /// <response code="400">Authentication failed.</response>
        [HttpPost]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> Login([FromBody] string email)
        {
            if (!await _userRepository.IsValidUser(email))
            {
                return BadRequest("Invalid email or password");
            }

            var now = DateTime.UtcNow;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now)
                    .ToUniversalTime()
                    .ToUnixTimeSeconds()
                    .ToString(), ClaimValueTypes.Integer64),
            };
            var signingCredentials = new SigningCredentials(_authSettings.TokenSigningKey, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: "PV247 API",
                audience: "PV247 Students",
                claims: claims,
                notBefore: now,
                expires: now.AddDays(1),
                signingCredentials: signingCredentials);

            var encodedJwt = new JwtSecurityTokenHandler()
                .WriteToken(jwt);

            return Ok(encodedJwt);
        }
    }
}