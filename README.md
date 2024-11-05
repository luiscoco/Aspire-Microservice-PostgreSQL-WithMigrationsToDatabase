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

We right click on the project name and select the **Manage User secrets...** menu option

![image](https://github.com/user-attachments/assets/83e61803-5d35-437c-bc49-53367975abfe)

## 5. 

## 6. 

## 7.

## 8.

## 9.

## 10.

