using Microsoft.OpenApi.Models;
using PowerOutageSchedule.Models;
using PowerOutageSchedule.Services.Implementations;
using PowerOutageSchedule.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PowerOutageSchedule API", Version = "v1" });
    c.EnableAnnotations();
});

// Register the data store
builder.Services.AddSingleton<DataStore>();

// Register the services
builder.Services.AddSingleton<IOutageImportService, OutageImportExportService>();
builder.Services.AddSingleton<IOutageExportService, OutageImportExportService>();
builder.Services.AddSingleton<IOutageReadService, OutageReadEditService>();
builder.Services.AddSingleton<IOutageEditService, OutageReadEditService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PowerOutageSchedule API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
