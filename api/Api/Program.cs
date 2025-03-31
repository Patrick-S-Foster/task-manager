using Api.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

// Read the password from the file and create the full connection string
var dbPassword = File.ReadAllText(dbPasswordFile);
var fullConnectionString = string.Format(connectionString, dbPassword);

builder.Services.AddDbContext<TaskDbContext>(options => options.UseMySQL(fullConnectionString));

builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", policyBuilder =>
        policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddEntityFrameworkStores<TaskDbContext>();

var app = builder.Build();

app.MapIdentityApi<IdentityUser>();

app.UseCors("AllowAll");
app.UseAuthorization();

app.Run();
