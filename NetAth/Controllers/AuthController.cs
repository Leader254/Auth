using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NetAth.Data;
using NetAth.Model;
using NetAth.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NetAth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserContext _userContext;
        private readonly ITokenService _tokenService;

        public AuthController(UserContext userContext, ITokenService tokenService)
        {
            this._userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            this._tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }
        [HttpPost, Route("Login")]
        public IActionResult Login([FromBody] LoginModel payload)
        {
            if(payload == null)
            {
                return BadRequest("Invalid client request");
            }

            var user = _userContext.LoginModels.FirstOrDefault(u => 
            (u.UserName == payload.UserName) && (u.Password == payload.Password)
            );

            if (user is null)
                return Unauthorized();

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, payload.UserName),
                new(ClaimTypes.Role, "Manager")
            }; 

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            _userContext.SaveChangesAsync();

            return Ok(new AuthenticatedResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken
            });
        }
    }
}
