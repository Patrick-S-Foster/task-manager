using Microsoft.AspNetCore.Identity;
using Task = TaskManager.Common.Task;

namespace TaskManager.Api.Model;

internal class DbTask : Task
{
    public required IdentityUser User { get; set; }
}