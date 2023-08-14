using ComicCatcher.Utils;

namespace ComicCatcher.ComicModels;

public class ComicBaseProperty
{
    public string Url { get; set; }
    /// <summary>
    /// 描述(第幾頁的內容或是第幾回)
    /// </summary>
    public string Caption
    {
        get { return this._caption; }
        set { this._caption = value.TrimEscapeString(); }
    }
    private string _caption;
}

public enum ComicState
{
    Created,
    Processing,
    ListLoaded,
    ImageLoaded,
    ListError,
}