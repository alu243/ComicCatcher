﻿using ComicApi.Controllers;
using ComicCatcherLib.ComicModels.Domains;
using Quartz;

namespace ComicApi.Model.Jobs
{
    public class UpdateFavoriteJob : IJob
    {
        private Dm5 dm5;
        private ComicApplication app;
        public UpdateFavoriteJob(Dm5 dm5, ComicApplication app)
        {
            this.dm5 = dm5;
            this.app = app;
        }
        public Task Execute(IJobExecutionContext context)
        {
            this.app.RefreshAllComicsAreFavorite();
            return Task.CompletedTask;
        }
    }
}