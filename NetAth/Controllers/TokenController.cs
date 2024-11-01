using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetAth.Data;
using NetAth.Model;
using NetAth.Services;

namespace NetAth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserContext userContext;
        private readonly ITokenService tokenService;

        public TokenController(UserContext context, ITokenService service)
        {
            this.userContext = context ?? throw new ArgumentNullException(nameof(context));
            this.tokenService = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(TokenApiModel tokenApiModel)
        {
            if (tokenApiModel == null)
                return BadRequest("Invalid client request");

            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;

            var principal = tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name;

            var user = userContext.LoginModels.SingleOrDefault(u => u.UserName == username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid client request");

            var newAccessToken = tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            userContext.SaveChanges();

            return Ok(new AuthenticatedResponse()
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
            });
        }

        [HttpPost]
        [Route("revoke")]
        public IActionResult Rovoke()
        {
            var username = User.Identity.Name;

            var user = userContext.LoginModels.SingleOrDefault(u =>u.UserName == username);
            if (user == null) return BadRequest();

            user.RefreshToken = null;
            userContext.SaveChanges();

            return NoContent();
        }
    }
}
