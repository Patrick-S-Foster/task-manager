using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Model;
using TaskManager.Common;
using Task = TaskManager.Common.Task;

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
        app.MapGet("/tasks",
                async (TaskDbContext db, UserManager<IdentityUser> userManager, ClaimsPrincipal claimsPrincipal) =>
                {
                    return await userManager.GetUserAsync(claimsPrincipal) is { } user
                        ? Results.Ok(await db.Tasks
                            .Include(task => task.Branches)
                            .ThenInclude(branch => branch.Repository)
                            .Include(task => task.Notes)
                            .Where(t => t.User == user)
                            .Select(t => t.WithoutIdentityUser())
                            .ToListAsync())
                        : Results.Unauthorized();
                })
            .RequireAuthorization();

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
                    State = TaskState.Created,
                    LastStart = null,
                    Duration = TimeSpan.Zero
                });
                await db.SaveChangesAsync();

                var task = entry.Entity.WithoutIdentityUser();

                return Results.Created($"{applicationUrl}/tasks/{task.Id}", task);
            }).RequireAuthorization();

        app.MapPut("/update/{id:int}",
            async (TaskDbContext db,
                UserManager<IdentityUser> userManager,
                ClaimsPrincipal claimsPrincipal,
                int id,
                [FromBody] Task task) =>
            {
                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                {
                    return Results.Unauthorized();
                }

                if (await db.Tasks
                        .Include(t => t.Branches)
                        .Include(t => t.Notes)
                        .SingleAsync(t => t.Id == id) is not { } existingTask ||
                    existingTask.User != user)
                {
                    return Results.NotFound();
                }

                if (existingTask.State is TaskState.Completed)
                {
                    return Results.UnprocessableEntity();
                }

                existingTask.Name = task.Name;
                existingTask.Description = task.Description;

                foreach (var note in existingTask.Notes
                             .Where(note => task.Notes.All(n => n.Id != note.Id))
                             .ToList())
                {
                    existingTask.Notes.Remove(note);
                }

                existingTask.Notes.AddRange(task.Notes.Where(note => note.Id is 0));

                foreach (var branch in existingTask.Branches
                             .Where(branch => task.Branches.All(b => b.Id != branch.Id))
                             .ToList())
                {
                    existingTask.Branches.Remove(branch);
                }

                existingTask.Branches.AddRange(task.Branches.Where(branch => branch.Id is 0));
                await db.SaveChangesAsync();

                return Results.NoContent();
            }).RequireAuthorization();

        app.MapPut("/start/{id:int}",
            async (TaskDbContext db,
                UserManager<IdentityUser> userManager,
                ClaimsPrincipal claimsPrincipal,
                int id) =>
            {
                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                {
                    return Results.Unauthorized();
                }

                if (await db.Tasks.SingleAsync(t => t.Id == id) is not { } existingTask || existingTask.User != user)
                {
                    return Results.NotFound();
                }

                if (existingTask.State is TaskState.Completed)
                {
                    return Results.UnprocessableEntity();
                }

                if (existingTask.State is TaskState.Created or TaskState.Paused)
                {
                    existingTask.LastStart = DateTime.Now;
                }

                existingTask.State = TaskState.Running;
                await db.SaveChangesAsync();

                return Results.Ok(existingTask.WithoutIdentityUser());
            }).RequireAuthorization();

        app.MapPut("/pause/{id:int}",
            async (TaskDbContext db,
                UserManager<IdentityUser> userManager,
                ClaimsPrincipal claimsPrincipal,
                int id) =>
            {
                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                {
                    return Results.Unauthorized();
                }

                if (await db.Tasks.SingleAsync(t => t.Id == id) is not { } existingTask || existingTask.User != user)
                {
                    return Results.NotFound();
                }

                if (existingTask.State is TaskState.Completed)
                {
                    return Results.UnprocessableEntity();
                }

                if (existingTask.State is TaskState.Running && existingTask.LastStart is { } lastStart)
                {
                    existingTask.Duration += DateTime.Now - lastStart;
                }

                existingTask.State = TaskState.Paused;
                existingTask.LastStart = null;
                await db.SaveChangesAsync();

                return Results.Ok(existingTask.WithoutIdentityUser());
            }).RequireAuthorization();

        app.MapPut("/complete/{id:int}",
            async (TaskDbContext db,
                UserManager<IdentityUser> userManager,
                ClaimsPrincipal claimsPrincipal,
                int id) =>
            {
                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                {
                    return Results.Unauthorized();
                }

                if (await db.Tasks.SingleAsync(t => t.Id == id) is not { } existingTask || existingTask.User != user)
                {
                    return Results.NotFound();
                }

                if (existingTask.State is TaskState.Completed)
                {
                    return Results.UnprocessableEntity();
                }

                if (existingTask.State is TaskState.Running && existingTask.LastStart is { } lastStart)
                {
                    existingTask.Duration += DateTime.Now - lastStart;
                }

                existingTask.State = TaskState.Completed;
                existingTask.LastStart = null;
                await db.SaveChangesAsync();

                return Results.Ok(existingTask.WithoutIdentityUser());
            }).RequireAuthorization();

        app.MapDelete("/delete/{id:int}",
            async (TaskDbContext db,
                UserManager<IdentityUser> userManager,
                ClaimsPrincipal claimsPrincipal,
                int id) =>
            {
                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                {
                    return Results.Unauthorized();
                }

                if (await db.Tasks
                        .Include(t => t.Branches)
                        .Include(t => t.Notes)
                        .SingleAsync(t => t.Id == id) is not { } existingTask ||
                    existingTask.User != user)
                {
                    return Results.NotFound();
                }

                db.Tasks.Remove(existingTask);
                await db.SaveChangesAsync();

                return Results.NoContent();
            }).RequireAuthorization();
    }
}