using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Api.Model;

internal class TaskDbContext(DbContextOptions<TaskDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<DbTask> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<DbTask>().HasOne(task => task.User).WithMany().IsRequired();
    }
}