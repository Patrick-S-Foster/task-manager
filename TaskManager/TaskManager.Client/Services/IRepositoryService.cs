namespace TaskManager.Client.Services;

public interface IRepositoryService
{
    IList<LocalRepository> Repositories { get; }

    Task EnsureLoadedAsync();

    Task CommitChangesAsync();
}