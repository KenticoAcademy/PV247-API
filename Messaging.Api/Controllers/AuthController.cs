using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Messaging.Api.Models;
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
        /// <param name="credentials">Provide an email identifying a user.</param>
        /// <response code="200">Returns a token if the authentication was successful.</response>
        /// <response code="400">Authentication failed.</response>
        [HttpPost]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        public async Task<IActionResult> Login([FromBody] LoginCredentials credentials)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _userRepository.IsValidUser(credentials.Email))
            {
                return BadRequest("Invalid email");
            }

            var now = DateTime.UtcNow;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, credentials.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now)
                    .ToUniversalTime()
                    .ToUnixTimeSeconds()
                    .ToString(), ClaimValueTypes.Integer64),
            };
            var signingCredentials = new SigningCredentials(_authSettings.TokenSigningKey, SecurityAlgorithms.HmacSha256);

            var expiration = now.AddDays(1);
            var jwt = new JwtSecurityToken(
                issuer: "PV247 API",
                audience: "PV247 Students",
                claims: claims,
                notBefore: now,
                expires: expiration,
                signingCredentials: signingCredentials);

            var encodedJwt = new JwtSecurityTokenHandler()
                .WriteToken(jwt);

            return Ok(new LoginResponse
            {
                Token = encodedJwt,
                Expiration = expiration
            });
        }
    }
}