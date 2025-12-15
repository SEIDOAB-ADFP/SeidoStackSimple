namespace Services.Greetings;

public class GreetingOptions
{
    private readonly GreetingService _service;

    public GreetingOptions(GreetingService service)
    {
        _service = service;
    }

    public void AddGreeter<T>(Func<T, string> greeter)
        where T : class
    {
        _service._typeGreeters[typeof(T)] = 
            (entity) => greeter((T)entity);
    }
}
