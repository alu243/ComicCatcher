﻿using Microsoft.AspNetCore.Mvc;

namespace ComicApi.Controllers
{
    [Route("[controller]")]
    public class PageController : Controller
    {
        [HttpGet("{page}")]
        public async Task<IActionResult> ShowComics(int page)
        {
            return View("ShowComics");
        }
    }
}
