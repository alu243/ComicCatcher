using Microsoft.AspNetCore.Mvc;

namespace ComicApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ComicController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "test";
        }

    }
}
