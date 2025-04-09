using System.Text.Json;
using Microsoft.JSInterop;
using TaskManager.Common;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Client.Services;

public partial class RepositoryService(IJSRuntime jsRuntime, ITaskService taskService) : IRepositoryService
{
    private const string Key = "LocalRepositoryService";

    private readonly List<LocalRepository> _repositories = [];

    private bool _loaded;

    public IList<LocalRepository> Repositories => _repositories;

    public async Task EnsureLoadedAsync()
    {
        if (_loaded)
        {
            return;
        }

        _loaded = true;

        if (await jsRuntime.InvokeAsync<string?>("window.localStorage.getItem", Key) is not { } storedValue)
        {
            return;
        }

        try
        {
            _repositories.Clear();
            _repositories.AddRange(JsonSerializer.Deserialize<IEnumerable<LocalRepository>>(storedValue) ?? []);
        }
        catch
        {
            // Suppressed in case the local storage entry is corrupted
        }

        _repositories.AddRange((await taskService.GetTasksAsync())
            .Select(task => task.Branches)
            .Where(branches => branches is not null)
            .Cast<ICollection<TemporaryBranch>>()
            .SelectMany(branches => branches)
            .Select(branch => branch.Repository)
            .Distinct(new RepositoryEqualityComparer())
            .Select(repository => new LocalRepository(repository.Name) { Id = repository.Id, Url = repository.Url }));
    }

    public async Task CommitChangesAsync()
    {
        await jsRuntime.InvokeVoidAsync("window.localStorage.setItem", Key, JsonSerializer.Serialize(Repositories));
    }
}