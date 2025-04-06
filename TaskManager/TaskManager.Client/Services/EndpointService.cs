namespace TaskManager.Client.Services;

public class EndpointService : IEndpointService
{
    public string Logout => "http://localhost:8080/logout";

    public string Register => "http://localhost:8080/register";

    public string Login => "http://localhost:8080/login";

    public string Refresh => "http://localhost:8080/refresh";

    public string Create => "http://localhost:8080/create";

    public string Get(int taskId) => $"http://localhost:8080/tasks/{taskId}";

    public string Update(int taskId) => $"http://localhost:8080/update/{taskId}";

    public string Start(int taskId) => $"http://localhost:8080/start/{taskId}";

    public string Pause(int taskId) => $"http://localhost:8080/pause/{taskId}";

    public string Complete(int taskId) => $"http://localhost:8080/complete/{taskId}";

    public string Delete(int taskId) => $"http://localhost:8080/delete/{taskId}";
}