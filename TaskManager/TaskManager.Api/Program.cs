using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api;
using TaskManager.Api.Model;

var builder = WebApplication.CreateBuilder(args);

// Guarantee that the connection string is set
if (builder.Configuration["DB_CONNECTION_STRING"] is not { } connectionString)
{
    throw new Exception("Connection string must be set in the docker compose file.");
}

// Guarantee that the password file is set
if (builder.Configuration["DB_PASSWORD_FILE"] is not { } dbPasswordFile)
{
    throw new Exception("Password file must be set via docker secrets.");
}

if (builder.Configuration["APPLICATION_URL"] is not { } applicationUrl)
{
    throw new Exception("Application URL must be set.");
}

// Read the password from the file and create the full connection string
var dbPassword = File.ReadAllText(dbPasswordFile);
var fullConnectionString = string.Format(connectionString, dbPassword);

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseMySql(fullConnectionString, ServerVersion.AutoDetect(fullConnectionString)));

builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", policyBuilder =>
        policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddEntityFrameworkStores<TaskDbContext>();

var app = builder.Build();

app.MapIdentityApi<IdentityUser>();
app.MapLogoutEndpoint();
app.MapCrudEndpoints(applicationUrl);

app.UseCors("AllowAll");
app.UseAuthorization();

app.Run();