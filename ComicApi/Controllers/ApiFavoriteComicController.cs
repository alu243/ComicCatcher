using ComicApi.Model;
using ComicApi.Model.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ComicApi.Controllers
{
    [ApiController]
    [Route("api/favoritecomic")]
    public class ApiFavoriteComicController : Controller
    {
        private ComicApplication app;

        public ApiFavoriteComicController(ComicApplication comicApplication)
        {
            this.app = comicApplication;
        }

        [HttpGet]
        public async Task<FavoriteComicModel> GetComicsAreFavorite()
        {
            var userId = Request.Cookies["userid"] ?? "";
            var comics =  await app.GetComicsAreFavorite(userId);
            return new FavoriteComicModel()
            {
                Comics = comics
            };
        }


        [HttpPost]
        public async Task<bool> AddFavoriteComic(FavoriteComic request)
        {
            try
            {
                var userId = Request.Cookies["userid"] ?? "";
                if (string.IsNullOrEmpty(userId)) return false;
                request.UserId = userId;
                return await app.AddFavoriteComic(request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
