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
    quartz.UseMicrosoftDependencyInjectionJobFactory();

    // 建立 Job2
    var jobKey2 = new JobKey("UpdateFavoritesByPagination", "UpdateFavoritesByPaginationGroup");
    quartz.AddJob<UpdateFavoriteByPaginationJob>(opts =>
    {
        opts.WithIdentity(jobKey2);
        opts.StoreDurably();
    });

    // 建立觸發器，自動執行 Job
    quartz.AddTrigger(opts =>
    {
        opts.ForJob(jobKey2);
        opts.WithIdentity("UpdateFavoritesByPaginationTrigger", "UpdateFavoritesByPaginationTriggerGroup");
        opts.WithSimpleSchedule(x => x.WithIntervalInHours(1).RepeatForever());
    });


    // 建立 Job
    var jobKey = new JobKey("UpdateAllFavorites", "UpdateAllFavoritesGroup");
    quartz.AddJob<UpdateFavoriteJob>(opts =>
    {
        opts.WithIdentity(jobKey);
        opts.StoreDurably();
    });

    // 建立觸發器，自動執行 Job
    quartz.AddTrigger(opts =>
    {
        opts.ForJob(jobKey);
        opts.WithIdentity("UpdateAllFavoritesTrigger", "UpdateAllFavoritesTriggerGroup");
        opts.WithSimpleSchedule(x => x.WithIntervalInHours(14).RepeatForever());
    });
});


//builder.Services.AddHttpClient("proxy", client =>
//{
//    //var handler = new SocketsHttpHandler() { UseCookies = true, Proxy = null };
//    //_httpClient = new HttpClient(handler);// { BaseAddress = baseAddress };
//    //client.DefaultRequestHeaders.Add("Referrer-Policy", "unsafe-url");
//}).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler()
//{
//    UseCookies = true,
//    Proxy = null,
//    MaxConnectionsPerServer = int.MaxValue
//});

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.UseStaticFiles();
app.Run();
