using TaskManager.Common;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Client.Services;

public interface ITaskService
{
    Task<IEnumerable<Common.Task>> GetTasksAsync();

    Task<Common.Task?> CreateTaskAsync(NewTask newTask);

    Task UpdateTaskAsync(Common.Task task);

    Task<Common.Task?> StartTaskAsync(Common.Task task);

    Task<Common.Task?> PauseTaskAsync(Common.Task task);

    Task<Common.Task?> CompleteTaskAsync(Common.Task task);

    Task DeleteTaskAsync(Common.Task task);
}