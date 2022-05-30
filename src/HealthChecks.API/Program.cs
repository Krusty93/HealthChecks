using HealthChecks.API;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.EntityFrameworkCore;

const string ALIVE = "alive";
const string ALIVE_DBCONTEXT = "dbcontext";
const string ALIVE_COSMOS = "cosmos";

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<MyDbContext>(opt =>
    {
        var cs = builder.Configuration.GetConnectionString("SqlServer");
        opt.UseSqlServer(cs);
    });

builder.Services
.AddSingleton<Database>(sp =>
{
    var uri = builder.Configuration.GetConnectionString("Cosmos");
    var authKey = builder.Configuration["Cosmos:AuthKey"];
    var databaseName = builder.Configuration["Cosmos:Database"];

    var cosmosClient = new CosmosClientBuilder(uri, authKey)
            .WithConnectionModeDirect()
            .Build();
    return cosmosClient.GetDatabase(databaseName);
});

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<MyDbContext>(name: ALIVE_DBCONTEXT)
    .AddCheck<CosmosHealthChecker>(name: ALIVE_COSMOS);

var app = builder.Build();

app.MapHealthChecks(
    "/alive",
    new HealthCheckOptions { Predicate = (c) => c.Name == ALIVE });

app.MapHealthChecks(
    "/dbcontext",
    new HealthCheckOptions { Predicate = (c) => c.Name == ALIVE_DBCONTEXT });

app.MapHealthChecks(
    "/cosmos",
    new HealthCheckOptions { Predicate = (c) => c.Name == ALIVE_COSMOS });

app.Run();
