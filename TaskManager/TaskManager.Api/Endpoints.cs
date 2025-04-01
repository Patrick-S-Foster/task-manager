using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Model;
using TaskManager.Common;

namespace TaskManager.Api;

internal static class Endpoints
{
    public static void MapLogoutEndpoint(this WebApplication app)
    {
        app.MapPost("/logout", async (SignInManager<IdentityUser> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.Ok();
        }).RequireAuthorization();
    }

    public static void MapCrudEndpoints(this WebApplication app, string applicationUrl)
    {
        app.MapGet("/tasks/{id:int}",
            async (TaskDbContext db, UserManager<IdentityUser> userManager, ClaimsPrincipal claimsPrincipal, int id) =>
            {
                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                {
                    return Results.Unauthorized();
                }

                if (db.Tasks.FirstOrDefault(t => t.Id == id && t.User == user) is { } task)
                {
                    return Results.Ok(task.WithoutIdentityUser());
                }

                return Results.NotFound();
            }).RequireAuthorization();

        app.MapPost("/create",
            async (TaskDbContext db,
                UserManager<IdentityUser> userManager,
                ClaimsPrincipal claimsPrincipal,
                [FromBody] NewTask newTask) =>
            {
                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                {
                    return Results.Unauthorized();
                }

                var entry = db.Tasks.Add(new DbTask
                {
                    User = user,
                    Name = newTask.Name,
                    Description = newTask.Description ?? string.Empty,
                    Notes = [],
                    Branches = [],
                    State = TaskState.Created,
                    LastStart = null,
                    Duration = TimeSpan.Zero
                });
                await db.SaveChangesAsync();

                var task = entry.Entity.WithoutIdentityUser();

                return Results.Created($"{applicationUrl}/tasks/{task.Id}", task);
            }).RequireAuthorization();
    }
}