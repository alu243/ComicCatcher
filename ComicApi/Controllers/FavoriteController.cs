using Microsoft.AspNetCore.Mvc;

namespace ComicApi.Controllers
{
    [Route("[controller]")]
    public class FavoriteController : Controller
    {
        [HttpGet("{level?}")]
        public async Task<IActionResult> ShowComics()
        {
            return View("ShowComics");
        }
    }
}
