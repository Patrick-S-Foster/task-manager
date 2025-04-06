namespace TaskManager.Client.Services;

public class TryResultWhenFalse<T>(bool success)
{
    private T _result = default!;
    private bool _resultSet = false;

    public bool Success => success;

    public T Result
    {
        get => _resultSet ? _result : throw new InvalidOperationException();
        set
        {
            _resultSet = true;
            _result = value;
        }
    }
}