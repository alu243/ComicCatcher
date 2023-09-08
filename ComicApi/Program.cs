using ComicApi.Controllers;
using ComicApi.Model.Jobs;
using ComicApi.Model.Repositories;
using ComicCatcherLib.ComicModels.Domains;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

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
        opts.WithSimpleSchedule(x => x.WithIntervalInMinutes(10).RepeatForever());
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
