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