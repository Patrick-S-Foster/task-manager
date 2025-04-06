namespace TaskManager.Client.Services;

public interface IEndpointService
{
    string Logout { get; }

    string Register { get; }

    string Login { get; }

    string Refresh { get; }

    string Create { get; }

    string Get(int taskId);

    string Update(int taskId);

    string Start(int taskId);

    string Pause(int taskId);

    string Complete(int taskId);

    string Delete(int taskId);
}