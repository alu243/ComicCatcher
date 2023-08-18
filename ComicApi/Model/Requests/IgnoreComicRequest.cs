using System.Text.Json.Serialization;

namespace ComicApi.Model.Requests;

public class IgnoreComicRequest
{
    public string Comic { get; set; }
    public string? ComicName { get; set; }
    [JsonIgnore] public string? UserId { get; set; }
}