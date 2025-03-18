using InputBindingDb.Repository;
using InputBindingDb.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();



    var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString"); 
    builder.Services.AddDbContext<RecordShopDbContext>(options =>
                           options.UseSqlServer(connectionString));




builder.Services.AddControllers();
builder.Services.AddScoped<IAlbumsRepository, AlbumsRepository>();
builder.Services.AddScoped<IAlbumsService, AlbumsService>();
builder.Services.AddScoped<IArtistsRepository, ArtistsRepository>();
builder.Services.AddScoped<IArtistsService, ArtistsService>();



builder.Build().Run();
