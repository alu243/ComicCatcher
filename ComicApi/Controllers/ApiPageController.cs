using ComicApi.Model;
using ComicCatcherLib.ComicModels;
using ComicCatcherLib.ComicModels.Domains;
using ComicCatcherLib.Utils;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ComicApi.Controllers;

[Route("api/page")]
[ApiController]
public class ApiPageController : Controller
{
    private Dm5 dm5;
    private IHostingEnvironment env;
    private ComicApplication app;

    public ApiPageController(Dm5 comic,
        IHostingEnvironment hostEnvironment,
        ComicApplication comicApplication)
    {
        dm5 = comic;
        env = hostEnvironment;
        app = comicApplication;
    }

    [HttpGet("{page}/{filter?}")]
    public async Task<PageModel> ShowComicsInPage(int page, string? filter)
    {
        
        var pagination = await app.GetPagnitation(page);
        if (page > 1) app.GetPagnitation(page - 1);
        if (page < 300) app.GetPagnitation(page + 1);

        var userId = Request.Cookies["userid"] ?? "";
        var ignoreComics = await app.GetIgnoreComics(userId);
        var favorites = await app.GetFavoriteComicDic(userId);

        var results = pagination.Comics.Select(c => new ComicViewModel()
        {
            Caption = c.Caption,
            IconUrl = c.IconUrl,
            IsFavorite = favorites.ContainsKey(c.Url.GetUrlDirectoryName()),
            IsIgnore = ignoreComics.ContainsKey(c.Url.GetUrlDirectoryName()),
            LastUpdateChapter = c.LastUpdateChapter,
            LastUpdateDate = c.LastUpdateDate,
            Url = c.Url
        }).ToList();

        if (string.IsNullOrEmpty(userId) ||
            "all".Equals(filter, StringComparison.CurrentCultureIgnoreCase))
        {
            // doNothing
        }
        else if (string.IsNullOrEmpty(filter))
        {
            results = results.Where(c => !c.IsIgnore).ToList();
        }
        else if ("favorite".Equals(filter, StringComparison.CurrentCultureIgnoreCase))
        {
            results = results.Where(c => c.IsFavorite).ToList();
        }

        return new PageModel()
        {
            CurrPagination = pagination,
            Comics = results
        };
    }

    [HttpGet("")]
    public async Task<PagesModel> ShowPaginations()
    {
        return new PagesModel()
        {
            Paginations = dm5.GetRoot().Paginations.Select(p => new ComicPagination()
            {
                Url = p.Url,
                Comics = new List<ComicEntity>(),
                Caption = p.Caption,
                ListState = ComicState.Created,
                TabNumber = p.TabNumber
            }).ToList()
        };
    }
}