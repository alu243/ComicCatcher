using ComicApi.Model.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ComicApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FavoriteComicController : Controller
    {
        private ComicApplication app;

        public FavoriteComicController(ComicApplication comicApplication)
        {
            this.app = comicApplication;
        }

        [HttpPost]
        public async Task<bool> AddFavoriteComic(FavoriteComic request)
        {
            var userId = Request.Cookies["userid"] ?? "";
            if (string.IsNullOrEmpty(userId)) return false;
            request.UserId = userId;
            return await app.AddFavoriteComic(request);
        }

        [HttpDelete]
        public async Task<bool> DeleteFavoriteComic(FavoriteComic request)
        {
            var userId = Request.Cookies["userid"] ?? "";
            if (string.IsNullOrEmpty(userId)) return false;
            request.UserId = userId;
            return await app.DeleteFavoriteComic(request);
        }

    }
}
