﻿using System.Text.Json.Serialization;

namespace ComicApi.Model.Requests;

public class FavoriteComic
{
    public string? UserId { get; set; }
    public string? Comic { get; set; }
    public string? ComicName { get; set; }
    public string? IconUrl { get; set; }
}