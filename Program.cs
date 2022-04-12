using System.Configuration;
using Elsa;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.Sqlite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var elsaSection = builder.Configuration.GetSection("Elsa");

// Elsa services.
builder.Services
    .AddElsa(elsa => elsa
        .UseEntityFrameworkPersistence(ef => ef.UseSqlite())
        .AddConsoleActivities()
        .AddHttpActivities(elsaSection.GetSection("Server").Bind)
        .AddQuartzTemporalActivities()
        .AddWorkflowsFrom<Startup>()
    );

// Elsa API endpoints.
builder.Services.AddElsaApiEndpoints();
builder.Services.AddCors(cors => cors.AddDefaultPolicy(policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin()
    .WithExposedHeaders("Content-Disposition"))
);
// For Dashboard.
builder.Services.AddRazorPages();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app
    .UseStaticFiles() 
    .UseCors()
    .UseHttpActivities()
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
        // Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
        endpoints.MapControllers();

        // For Dashboard.
        endpoints.MapFallbackToPage("/_Host");
    });


app.Run();




