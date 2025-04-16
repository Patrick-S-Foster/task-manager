using TaskManager.Common;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Client.Services;

public interface IRepositoryService
{
    IList<Repository> Repositories { get; }

    Task EnsureLoadedAsync();

    Task CommitChangesAsync();
}