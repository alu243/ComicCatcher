using ComicApi.Controllers;
using ComicApi.Model.Jobs;
using ComicApi.Model.Repositories;
using ComicCatcherLib.ComicModels.Domains;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
//builder.Services.AddSystemWebAdapters();
builder.Services.AddServerSideBlazor(c => c.DetailedErrors = true);

builder.Services.AddSingleton<Dm5>();
builder.Services.AddSingleton<ComicApiRepository>();
builder.Services.AddSingleton<ComicApplication>();

// quartz
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddQuartz(quartz =>
{
    // 建立 Job2
    var jobKey2 = new JobKey("UpdateFavoriteByPagination", "UpdateFavoriteGroupByPagination");
    quartz.AddJob<UpdateFavoriteByPaginationJob>(opts =>
    {
        opts.WithIdentity(jobKey2);
        opts.StoreDurably();
    });

    // 建立觸發器，自動執行 Job
    quartz.AddTrigger(opts =>
    {
        opts.ForJob(jobKey2);
        opts.WithIdentity("HelloWordTriggerByPagination", "HelloWordGroupByPagination");
        opts.WithSimpleSchedule(x => x.WithIntervalInHours(24 * 7).RepeatForever());
    });


    quartz.UseMicrosoftDependencyInjectionJobFactory();
    // 建立 Job
    var jobKey = new JobKey("UpdateFavorite", "UpdateFavoriteGroup");
    quartz.AddJob<UpdateFavoriteJob>(opts =>
    {
        opts.WithIdentity(jobKey);
        opts.StoreDurably();
    });

    // 建立觸發器，自動執行 Job
    quartz.AddTrigger(opts =>
    {
        opts.ForJob(jobKey);
        opts.WithIdentity("HelloWordTrigger", "HelloWordGroup");
        opts.WithSimpleSchedule(x => x.WithIntervalInHours(29).RepeatForever());
    });
});



var app = builder.Build();
// web
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.UseStaticFiles();
app.Run();
