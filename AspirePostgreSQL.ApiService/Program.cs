using AspirePostgreSQL.Database;
using Microsoft.EntityFrameworkCore;
using AspirePostgreSQL.Extensions;
using AspirePostgreSQL.ApiService.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.CustomSchemaIds(id => id.FullName!.Replace('+', '-')));
builder.Services.AddCors();

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("databaseconnectionstring")));

builder.Services.AddScoped<ArticleService>();

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    app.ApplyMigrations();
}

app.MapControllers();

app.Run();
