public interface IWorkoutPlannerService
{
    Task<WorkoutPlanDTO> GetWorkoutByUserIdAsync(Guid id);
    // Task<WorkoutPlanDTO> GetWorkoutPlanByUserIdAsync(Guid id);
}