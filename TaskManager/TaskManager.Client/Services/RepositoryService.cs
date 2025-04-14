using System.Text.Json;
using Microsoft.JSInterop;
using TaskManager.Common;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Client.Services;

public partial class RepositoryService(IJSRuntime jsRuntime, ITaskService taskService) : IRepositoryService
{
    private const string Key = "LocalRepositoryService";

    private readonly List<Repository> _repositories = [];

    private bool _loaded;

    public IList<Repository> Repositories => _repositories;

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
            _repositories.AddRange(JsonSerializer.Deserialize<IEnumerable<Repository>>(storedValue) ?? []);
        }
        catch
        {
            // Suppressed in case the local storage entry is corrupted
        }

        _repositories.AddRange((await taskService.GetTasksAsync())
            .SelectMany(task => task.Branches)
            .Select(branch => branch.Repository)
            .Distinct(new RepositoryEqualityComparer()));
    }

    public async Task CommitChangesAsync()
    {
        await jsRuntime.InvokeVoidAsync("window.localStorage.setItem", Key, JsonSerializer.Serialize(Repositories));
    }
}