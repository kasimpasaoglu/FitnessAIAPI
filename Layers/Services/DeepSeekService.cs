
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

public class DeepSeekService : IDeepSeekService
{
  private readonly HttpClient _httpClient;
  private readonly string _apiKey;
  private readonly ILogService _logService;



  public DeepSeekService(HttpClient httpClient, ILogService logService)
  {
    _logService = logService;
    _httpClient = httpClient;
    _apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY") ?? throw new ArgumentNullException("DEEPSEEK_API_KEY not found");
  }




  public async Task<ExerciseJsonModel> RequestExerciseAsync(UserDTO user)
  {

    #region Mock
    var process = Environment.GetEnvironmentVariable("PROCESS");
    if (process == "DEV")
    {
      var mock = File.ReadAllText("Infrastructure/DeepSeekMock.json");
      var model = JsonSerializer.Deserialize<ExerciseJsonModel>(mock, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });
      System.Threading.Thread.Sleep(5000);
      return model;
    }
    #endregion


    var url = "https://api.deepseek.com/chat/completions";

    var promt = await GeneratePrompt(user);
    var requestBody = new
    {
      model = "deepseek-chat",
      messages = new[]
      {
        new {role = "system", content = "You are a helpful fitness coach."},
        new {role = "user", content = promt}
      }
    };
    var json = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

    var response = await _httpClient.PostAsync(url, content);

    if (!response.IsSuccessStatusCode)
    {
      var error = await response.Content.ReadAsStringAsync();
      throw new ExternalException($"DeepSeek API Error: {response.StatusCode}, {error}");
    }

    var resultJson = await response.Content.ReadAsStringAsync();
    Console.WriteLine("Response from DeepSeek:");
    Console.WriteLine(resultJson);

    using var doc = JsonDocument.Parse(resultJson);
    var root = doc.RootElement;

    var contentText = root
        .GetProperty("choices")[0]
        .GetProperty("message")
        .GetProperty("content")
        .GetString();

    Console.WriteLine("Content from DeepSeek:");
    Console.WriteLine(contentText);

    if (string.IsNullOrEmpty(contentText))
      throw new ExternalException("AI returned empty content.");

    var sanitized = SanitizeContentText(contentText);
    var exerciseJson = JsonSerializer.Deserialize<ExerciseJsonModel>(sanitized, new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    });



    return exerciseJson;
  }


  private async Task<string> GeneratePrompt(UserDTO user)
  {
    var example = new ExerciseJsonModel
    {
      ProgramDurationWeeks = 0,
      WeeklySchedule = new List<WorkoutDay>
      {
          new WorkoutDay
          {
              Day = "",
              Warmup = "",
              Focus = "",
              Exercises = new List<Exercise>
              {
                  new Exercise { Name = "", Sets = "", Reps = "", WeightRangeKg = "" },
                  new Exercise { Name = "", Sets = "", Reps = "", WeightRangeKg = "" },
                  new Exercise { Name = "", Sets = "", Reps = "", WeightRangeKg = "" }
              },
              Cooldown = ""
          }
      },
      ActiveRecoveryNote = "",
      PersonalNote = ""
    };

    var exampleJson = JsonSerializer.Serialize(example, new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });

    var goal = user.Goal.GetDisplayName();

    var availableDays = user.AvailableDays.Select(day => day.GetDisplayName()).ToList();


    var promptUserData = new
    {
      user.Name,
      user.Surname,
      user.Age,
      user.HeightCm,
      user.WeightKg,
      user.Gender,
      goal,
      availableDays,
      user.HasHealthIssues,
      user.MedicationsUsing
    };

    var userDataJson = JsonSerializer.Serialize(promptUserData, new JsonSerializerOptions
    {
      WriteIndented = true
    });

    var prompt = $@"Create a personalized fitness plan based on this user data:
      {userDataJson}
      Each workout day must include:
      - warm-up,
      - muscle group focus,
      - 3–5 exercises with sets, reps, and weight range in kg,
      - cooldown.
      For active recovery, write a single helpful note (e.g., yoga, walking) under 'activeRecoveryNote'.
      If the user has any health issues or takes medication (e.g., beta blockers), adapt the plan accordingly and add a health warning in the 'personalNote'.
      Also, add a motivational message, personal suggestions and tips in 'personalNote'.
      Respond STRICTLY in the following JSON format. NO CHANGES TO STRUCTURE:
      {exampleJson}";

    // log promt
    await _logService.LogSuccess("Prompt Sent", "DeepSeekService", new
    {
      userId = user.UserId.ToString(),
      prompt
    });

    return prompt;
  }

  private string SanitizeContentText(string content)
  {
    if (content.StartsWith("```"))
    {
      var start = content.IndexOf('\n');
      var end = content.LastIndexOf("```");

      if (start != -1 && end != -1 && end > start)
      {
        content = content.Substring(start + 1, end - start - 1).Trim();
      }
    }

    return content;
  }


}