# Building a Blazor Web App with a PostgreSQL CRUD Microservice Using Aspire .NET 9

## 1. Create .NET Aspire 9 Starter Application

We run Visual Studio 2022 Community Edition and Create a New Project

We select the **.NET Aspire 9 Starter App** project template

![image](https://github.com/user-attachments/assets/5e31cf38-1f0a-4398-9591-7f8fb1f81e20)

We input the project name and location and press the Next button

![image](https://github.com/user-attachments/assets/d2c4ffec-a540-45fe-a871-13de0ef03e46)

We select the .NET 9 framework and press the Create button

![image](https://github.com/user-attachments/assets/59ddc3c2-0fe0-4f4d-8295-9c6ff617d14e)

We can verify the project folders and files structure

![image](https://github.com/user-attachments/assets/b1ec8fdd-362f-42af-a64c-4d4ce49e45d1)

## 2. Load the PostgreSQL Nuget package (AspirePostgreSQL.AppHost project)

![image](https://github.com/user-attachments/assets/9111f764-0ba2-4ffe-a8dd-ac2f22b148a3)

## 3. We modify the middleware(Program.cs) (AspirePostgreSQL.AppHost project)

We define the database username and password:

```csharp
var contentplatformdbpassword = builder.AddParameter("contentplatform-db-password");
var contentplatformdbusername = builder.AddParameter("contentplatform-db-username");
```

We also register the database server and assign the username, password and port for accesing:

```csharp
var postgreServer = builder.AddPostgres("contentplatform-db", contentplatformdbusername, contentplatformdbpassword, port: 5432)
    .WithDataVolume()
    .WithPgAdmin();
```

We create the database:

```csharp
var postgredatabase = postgreServer.AddDatabase("contentplatform");
```

This code sets up an **API Service project** with a **PostgreSQL database** reference, registering it in the application's builder so that it can be managed and injected into other parts of the application as needed

```csharp
var apiService = builder.AddProject<Projects.AspirePostgreSQL_ApiService>("apiservice").WithReference(postgredatabase);
```

This code sets up a **Web Frontend project** that connects to a **PostgreSQL database**, can interact with **external HTTP endpoints**, and relies on an **API Service** (apiService) for additional backend functionality

```csharp
builder.AddProject<Projects.AspirePostgreSQL_Web>("webfrontend")
    .WithReference(postgredatabase)
    .WithExternalHttpEndpoints()
    .WithReference(apiService);
```

We review the middleware whole code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var contentplatformdbpassword = builder.AddParameter("contentplatform-db-password");
var contentplatformdbusername = builder.AddParameter("contentplatform-db-username");

var postgreServer = builder.AddPostgres("contentplatform-db", contentplatformdbusername, contentplatformdbpassword, port: 5432)
    .WithDataVolume()
    .WithPgAdmin();

var postgredatabase = postgreServer.AddDatabase("contentplatform");

var apiService = builder.AddProject<Projects.AspirePostgreSQL_ApiService>("apiservice").WithReference(postgredatabase);

builder.AddProject<Projects.AspirePostgreSQL_Web>("webfrontend")
    .WithReference(postgredatabase)
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
```

## 4. We manage the PostgreSQL database secrets (AspirePostgreSQL.AppHost project)

We right click on the project name and select the **Manage User Secrets...** menu option

![image](https://github.com/user-attachments/assets/83e61803-5d35-437c-bc49-53367975abfe)

We add the secrets in the **secrets.json** file

```json
{
  "Parameters:contentplatform-db-username": "postgres",
  "Parameters:contentplatform-db-password": "3AfRV)vhP23aj1!!wHU{Hc"
}
```

## 5. Load the Nuget packages (AspirePostgreSQL.ApiService)

These packages together allow the project to use Entity Framework Core for database access (specifically PostgreSQL) and provide Swagger-based API documentation

![image](https://github.com/user-attachments/assets/eb91cd58-69e0-42b0-9772-a9f8119e3b9f)

**Microsoft.EntityFrameworkCore (9.0.0-rc.2.24474.1)**: This is the core package for Entity Framework Core, which is an Object-Relational Mapping (ORM) framework for .NET

It allows developers to work with databases using .NET objects and LINQ queries instead of raw SQL

The version here is a release candidate (rc), meaning it's close to stable but not fully released

**Microsoft.EntityFrameworkCore.Tools (9.0.0-rc.2.24474.1)**: This package provides tools for Entity Framework Core, enabling features like migrations, model scaffolding, and database updates via the command line

It’s essential for managing schema changes in a database through code

**Npgsql.EntityFrameworkCore.PostgreSQL (9.0.0-rc.2)**: This package is the PostgreSQL provider for Entity Framework Core

It allows EF Core to connect to PostgreSQL databases specifically, translating LINQ queries into PostgreSQL-compatible SQL

This package is needed if the project uses PostgreSQL as its database

**Swashbuckle.AspNetCore (6.9.0)**: Swashbuckle is a library that integrates Swagger with ASP.NET Core applications

It generates Swagger documentation for APIs, allowing you to create a user-friendly API documentation and test interface

This package makes it easier to visualize and test APIs directly from a web UI

## 6. Add the data model 

![image](https://github.com/user-attachments/assets/5b48e348-76b7-459e-9f48-4b9baef82e09)

```csharp
namespace AspirePostgreSQL.Entities;

public class Article
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime? CreatedDate { get; set; }
}
```

## 7. Add the database DbContext

![image](https://github.com/user-attachments/assets/804d9619-3c11-4c0d-9b15-9e66188508f0)

```csharp
using Microsoft.EntityFrameworkCore;
using AspirePostgreSQL.Entities;

namespace AspirePostgreSQL.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("content");
    }

    public DbSet<Article> Articles { get; set; }
}
```

## 8. Define the database CRUD Service 

![image](https://github.com/user-attachments/assets/afe76a78-0227-45c2-841c-7f795877977c)

```csharp
using AspirePostgreSQL.Database;
using AspirePostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspirePostgreSQL.ApiService.Service
{
    public class ArticleService
    {
        private readonly ApplicationDbContext _context;

        public ArticleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Article>> GetAllAsync()
        {
            return await _context.Articles.ToListAsync();
        }

        public async Task<Article?> GetByIdAsync(Guid id)
        {
            return await _context.Articles.FindAsync(id);
        }

        public async Task<Article> CreateAsync(Article article)
        {
            article.Id = Guid.NewGuid();

            // Default to UTC now if CreatedDate is not provided
            article.CreatedDate ??= DateTime.UtcNow;

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return article;
        }

        public async Task<bool> UpdateAsync(Article updatedArticle)
        {
            var existingArticle = await _context.Articles.FindAsync(updatedArticle.Id);
            if (existingArticle == null)
            {
                return false;
            }

            existingArticle.Title = updatedArticle.Title;
            existingArticle.Content = updatedArticle.Content;
            existingArticle.CreatedDate = updatedArticle.CreatedDate;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return false;
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
```

## 9. Add the Web API Controller

![image](https://github.com/user-attachments/assets/0272ba33-15a4-40aa-aada-613ab54b0be0)

This code defines a RESTful API controller for managing "articles" in a PostgreSQL database using an ArticleService

It supports basic CRUD operations (Create, Read, Update, Delete) and handles responses and errors appropriately for each operation

```csharp
using AspirePostgreSQL.ApiService.Service;
using AspirePostgreSQL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AspirePostgreSQL.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly ArticleService _articleService;

        public ArticlesController(ArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var articles = await _articleService.GetAllAsync();
            return Ok(articles);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Article article)
        {
            if (article == null)
            {
                return BadRequest();
            }

            try
            {
                var createdArticle = await _articleService.CreateAsync(article);
                return CreatedAtAction(nameof(GetById), new { id = createdArticle.Id }, createdArticle);
            }
            catch (Exception ex)
            {
                // Log the exception or write to the console for debugging
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Article article)
        {
            if (article == null || article.Id != id)
            {
                return BadRequest();
            }

            var success = await _articleService.UpdateAsync(article);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _articleService.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
```

## 10. Define the middleware(Program.cs)

![image](https://github.com/user-attachments/assets/47978f44-704b-460d-8f62-cddfbca9e716)

We define the connection string for accessing the PostgreSQL database:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("databaseconnectionstring")));
```

We also register the Service with the CRUD operations

```csharp
builder.Services.AddScoped<ArticleService>();
```

We also define the code for the database initial migration

```csharp
app.ApplyMigrations();
```

We review the middleware(Program.cs):

```csharp
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
```

## 11. Define the database Migration definition

![image](https://github.com/user-attachments/assets/dda847d2-a9d0-4f5d-b544-3d2d17c2322d)

This code defines an extension method in C# that applies database migrations for an ASP.NET Core application using Entity Framework Core with a PostgreSQL database.

This method is typically called during application startup to ensure the database schema is up to date with the latest migrations, improving reliability by handling transient connectivity issues automatically

```csharp
using AspirePostgreSQL.Database;
using Microsoft.EntityFrameworkCore;

namespace AspirePostgreSQL.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        int retryCount = 5; // Number of times to retry
        int delayInMilliseconds = 2000; // Delay between retries in milliseconds

        for (int attempt = 0; attempt < retryCount; attempt++)
        {
            try
            {
                dbContext.Database.Migrate();
                break; // Exit loop if migration is successful
            }
            catch (Npgsql.NpgsqlException ex)
            {
                if (attempt == retryCount - 1) // Last attempt, rethrow the exception
                {
                    throw;
                }

                Console.WriteLine($"Migration attempt {attempt + 1} failed: {ex.Message}");
                Thread.Sleep(delayInMilliseconds); // Wait before retrying
            }
        }
    }
}
```

## 12. Add Migration for creating the database and tables

We first have to set the startup project for creating the migration

![image](https://github.com/user-attachments/assets/1eaac635-d7ea-43c9-8cf4-8f0a4c178b10)

We have to open the **Package Manager Console**

![image](https://github.com/user-attachments/assets/6928255e-2c8d-4e7e-9b0d-25bb23872168)

We run the following command to create the **Migration** folder and files for creating the database and tables

```
Add-Migration initialmigration
```

We have to select the project **AspirePostgreSQL.ApiService** where to create the migration files 

![image](https://github.com/user-attachments/assets/d6df95eb-76b4-4632-96b5-ffc5da7a3754)

We verfy the Migration was created

![image](https://github.com/user-attachments/assets/3bafd622-61a4-4a07-b758-98b26655869e)

This migration **creates an Articles table** in the content schema, with **columns for Id, Title, Content, and CreatedDate**

The **Up method builds the table**, and the **Down method deletes it**, allowing for easy application and rollback of this database change.

```csharp
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspirePostgreSQL.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "content");

            migrationBuilder.CreateTable(
                name: "Articles",
                schema: "content",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles",
                schema: "content");
        }
    }
}
```

This C# code defines a "model snapshot" for **Entity Framework Core**, specifically for the **ApplicationDbContext** class

A model snapshot is an autogenerated file that represents the current state of the **database schema** based on the Entity Framework Core models

It helps Entity Framework compare and generate future migrations by storing the current structure

This model snapshot represents the structure of the **Articles table** in the content schema, storing columns for Id, Content, Title, and CreatedDate

The snapshot captures the current **schema state**, enabling Entity Framework to detect and apply future changes to the database

```csharp
// <auto-generated />
using System;
using AspirePostgreSQL.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AspirePostgreSQL.ApiService.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("content")
                .HasAnnotation("ProductVersion", "9.0.0-rc.2.24474.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AspirePostgreSQL.Entities.Article", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Articles", "content");
                });
#pragma warning restore 612, 618
        }
    }
}
```




