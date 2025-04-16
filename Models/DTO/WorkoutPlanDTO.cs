public class WorkoutPlanDTO
{
    public Guid PlanId { get; set; }

    public Guid UserId { get; set; }

    public ExerciseJsonModel ExerciseJson { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

}