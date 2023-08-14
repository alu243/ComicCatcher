var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
//builder.Services.AddSystemWebAdapters();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseAuthorization();

app.MapControllers();

app.Run();
