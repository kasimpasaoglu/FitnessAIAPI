using API.DMO;

public interface IDeepSeekService
{
    Task<string> RequestExerciseAsync(User user);
}