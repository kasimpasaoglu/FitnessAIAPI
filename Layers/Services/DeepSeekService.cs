
using API.DMO;

public class DeepSeekService : IDeepSeekService
{
  public Task<string> RequestExerciseAsync(User user)
  {
    var process = Environment.GetEnvironmentVariable("PROCESS");
    if (process == "DEV")
    {
      var mock = File.ReadAllText("Infrastructure/DeepSeekMock.json");
      return Task.FromResult(mock);
    }
    else
      return Task.FromResult("");
  }
}