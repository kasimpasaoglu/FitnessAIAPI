
public interface IDeepSeekService
{
    Task<ExerciseJsonModel> RequestExerciseAsync(UserDTO user);
}