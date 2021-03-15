using Microsoft.AspNetCore.Mvc;

namespace Musix4u_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        public long? UserId => User.HasClaim(x => x.Type.Equals("userId")) ? long.Parse(User.FindFirst("userId").Value) : null;
    }
}