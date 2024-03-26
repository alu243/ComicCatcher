using ComicApi.Controllers;
using ComicCatcherLib.ComicModels.Domains;
using Quartz;

namespace ComicApi.Model.Jobs
{
    public class UpdateFavoriteJob : IJob
    {
        private ComicApplication app;
        public UpdateFavoriteJob(ComicApplication app)
        {
            this.app = app;
        }
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} start [UpdateFavoriteJob] job");
            this.app.RefreshAllComicsAreFavorite().Wait();
            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} end [UpdateFavoriteJob] job");
            return Task.CompletedTask;
        }
    }

    public class UpdateFavoriteByPaginationJob : IJob
    {
        private ComicApplication app;
        public UpdateFavoriteByPaginationJob(ComicApplication app)
        {
            this.app = app;
        }
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} start [UpdateFavoriteByPaginationJob] job");
            this.app.RefreshPagesComicsAreFavorite(3).Wait();
            Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} end [UpdateFavoriteByPaginationJob] job");
            return Task.CompletedTask;
        }
    }
}
