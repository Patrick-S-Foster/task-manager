using Microsoft.AspNetCore.Identity;
using Task = TaskManager.Common.Task;

namespace TaskManager.Api.Model;

internal class DbTask : Task
{
    public required IdentityUser User { get; set; }

    public Task WithoutIdentityUser() => new()
    {
        Id = Id,
        Name = Name,
        Description = Description,
        Notes = Notes,
        Branches = Branches,
        State = State,
        LastStart = LastStart,
        Duration = Duration
    };
}