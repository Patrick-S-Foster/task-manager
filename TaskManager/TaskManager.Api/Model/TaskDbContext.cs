using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Api.Model;

public class TaskDbContext(DbContextOptions<TaskDbContext> options) : IdentityDbContext<IdentityUser>(options);