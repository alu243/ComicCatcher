using ComicApi.Model.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ComicApi.Controllers;

[ApiController]
[Route("api/favoritechapter")]
public class ApiFavoriteChapterController : Controller
{
    private ComicApplication app;

    public ApiFavoriteChapterController(ComicApplication comicApplication)
    {
        this.app = comicApplication;
    }

    [HttpPost]
    public async Task<bool> AddFavoriteChapter(FavoriteChapter request)
    {
        try
        {
            var userId = Request.Cookies["userid"] ?? "";
            if (string.IsNullOrEmpty(userId)) return false;
            request.UserId = userId;
            return await app.AddFavoriteChapter(request);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}