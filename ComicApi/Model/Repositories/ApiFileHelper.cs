using System.IO;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ComicCatcherLib.DbModel;

public class ApiFileHelper
{
    private string baseDir;

    public ApiFileHelper(IHostingEnvironment hostEnvironment)
    {
        var env = hostEnvironment;
        if (!Directory.Exists(Path.Combine(env.ContentRootPath, "comic")))
            Directory.CreateDirectory(Path.Combine(env.ContentRootPath, "comic"));
        baseDir = Path.Combine(env.ContentRootPath, "comic");
    }

    public bool IsExists(string chapter, string file)
    {
        var fullpath = Path.Combine(baseDir, chapter, file);
        return File.Exists(fullpath);
    }

    public bool IsExists(string chapter)
    {
        var fullpath = Path.Combine(baseDir, chapter);
        return Directory.Exists(fullpath);
    }

    public void Delete(string chapter)
    {
        var fullpath = Path.Combine(baseDir, chapter);
        Directory.Delete(fullpath, true);
    }


    public async Task<byte[]> GetFileContent(string chapter, string file)
    {
        if (!IsExists(chapter, file))
        {
            throw new IOException($"No File at {chapter}/{file}");
        }
        var fullpath = Path.Combine(baseDir, chapter, file);
        var content  = await File.ReadAllBytesAsync(fullpath);
        return content;
    }

    public async Task SaveFile(string chapter, string file, byte[] content)
    {
        var dir = Path.Combine(baseDir, chapter);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        var fullpath = Path.Combine(dir, file);

        using (FileStream fs = new FileStream(fullpath, FileMode.CreateNew, FileAccess.Write))
        {
            await fs.WriteAsync(content, 0, content.Length);
        }
    }
}