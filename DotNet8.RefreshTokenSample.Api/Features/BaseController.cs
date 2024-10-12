using DotNet8.RefreshTokenSample.Api.Shared;

namespace DotNet8.RefreshTokenSample.Api.Features
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult Content(object obj)
        {
            return Content(obj.ToJson(), "application/json");
        }
    }
}
