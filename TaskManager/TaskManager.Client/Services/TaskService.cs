using System.Net.Http.Json;
using System.Text.Json;
using TaskManager.Common;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Client.Services;

public class TaskService(
    IHttpClientFactory httpClientFactory,
    IEndpointService endpointService,
    IAuthenticationService authenticationService,
    IGitService gitService) : ITaskService
{
    public async Task<IEnumerable<Common.Task>> GetTasksAsync()
    {
        using var httpClient = httpClientFactory.CreateClient();
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, endpointService.Get);

        if (!await authenticationService.TryAddAuthorizationHeaderAsync(requestMessage))
        {
            return [];
        }

        using var response = await httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        return JsonSerializer.Deserialize<IEnumerable<Common.Task>>(await response.Content.ReadAsStreamAsync()) ?? [];
    }

    public async Task<Common.Task?> CreateTaskAsync(NewTask newTask)
    {
        using var httpClient = httpClientFactory.CreateClient();
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpointService.Create);
        requestMessage.Content = JsonContent.Create(newTask);

        if (!await authenticationService.TryAddAuthorizationHeaderAsync(requestMessage))
        {
            return null;
        }

        using var response = await httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return JsonSerializer.Deserialize<Common.Task>(await response.Content.ReadAsStreamAsync());
    }

    public async Task UpdateTaskAsync(Common.Task task)
    {
        using var httpClient = httpClientFactory.CreateClient();
        using var requestMessage = new HttpRequestMessage(HttpMethod.Put, endpointService.Update(task.Id));
        requestMessage.Content = JsonContent.Create(task);

        if (await authenticationService.TryAddAuthorizationHeaderAsync(requestMessage))
        {
            using var _ = await httpClient.SendAsync(requestMessage);
        }
    }

    public async Task<Common.Task?> StartTaskAsync(Common.Task task)
    {
        if (task.State is TaskState.Paused)
        {
            foreach (var branch in task.Branches)
            {
                gitService.Restore(branch);
            }
        }

        using var httpClient = httpClientFactory.CreateClient();
        using var requestMessage = new HttpRequestMessage(HttpMethod.Put, endpointService.Start(task.Id));

        if (!await authenticationService.TryAddAuthorizationHeaderAsync(requestMessage))
        {
            return null;
        }

        using var response = await httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return JsonSerializer.Deserialize<Common.Task>(await response.Content.ReadAsStreamAsync());
    }

    public async Task<Common.Task?> PauseTaskAsync(Common.Task task)
    {
        if (task.State is not TaskState.Paused and not TaskState.Completed)
        {
            foreach (var branch in task.Branches)
            {
                gitService.CreateTemporaryBranch(branch.Repository);
            }
        }

        using var httpClient = httpClientFactory.CreateClient();
        using var requestMessage = new HttpRequestMessage(HttpMethod.Put, endpointService.Pause(task.Id));

        if (!await authenticationService.TryAddAuthorizationHeaderAsync(requestMessage))
        {
            return null;
        }

        using var response = await httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return JsonSerializer.Deserialize<Common.Task>(await response.Content.ReadAsStreamAsync());
    }

    public async Task<Common.Task?> CompleteTaskAsync(Common.Task task)
    {
        using var httpClient = httpClientFactory.CreateClient();
        using var requestMessage = new HttpRequestMessage(HttpMethod.Put, endpointService.Complete(task.Id));

        if (!await authenticationService.TryAddAuthorizationHeaderAsync(requestMessage))
        {
            return null;
        }

        using var response = await httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return JsonSerializer.Deserialize<Common.Task>(await response.Content.ReadAsStreamAsync());
    }

    public async Task DeleteTaskAsync(Common.Task task)
    {
        using var httpClient = httpClientFactory.CreateClient();
        using var requestMessage = new HttpRequestMessage(HttpMethod.Delete, endpointService.Delete(task.Id));

        if (!await authenticationService.TryAddAuthorizationHeaderAsync(requestMessage))
        {
            return;
        }

        using var _ = await httpClient.SendAsync(requestMessage);
    }

    public async Task<Common.Task?> GetRunningTaskAsync() =>
        (await GetTasksAsync()).FirstOrDefault(task => task.State is TaskState.Running);
}