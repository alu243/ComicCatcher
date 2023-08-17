using ComicApi.Model.Repositories;
using ComicCatcherLib.ComicModels.Domains;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRazorPages();
//builder.Services.AddSystemWebAdapters();
builder.Services.AddServerSideBlazor(c => c.DetailedErrors = true);

builder.Services.AddSingleton<Dm5>();
builder.Services.AddSingleton<ComicApiRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
