using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetAth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        [HttpGet, Authorize(Roles = "Manager")]
        public IEnumerable<string> Get()
        {
            string[] strings = ["John Doe", "Jane Doe"];
            return strings;
        }
    }
}
