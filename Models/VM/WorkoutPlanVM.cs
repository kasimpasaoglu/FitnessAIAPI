public class WorkoutPlanVM
{
    public Guid PlanId { get; set; }

    public Guid UserId { get; set; }

    public ExerciseJsonModel ExerciseJson { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

}

public class ExerciseJsonModel
{
    public int ProgramDurationWeeks { get; set; }
    public List<WorkoutDay> WeeklySchedule { get; set; } = new();
    public string ActiveRecoveryNote { get; set; } = "";
    public string PersonalNote { get; set; } = "";
}

public class WorkoutDay
{
    public string Day { get; set; } = "";
    public string Warmup { get; set; } = "";
    public string Focus { get; set; } = "";
    public List<Exercise> Exercises { get; set; } = new();
    public string Cooldown { get; set; } = "";
}

public class Exercise
{
    public string Name { get; set; } = "";
    public string Sets { get; set; } = "";
    public string Reps { get; set; } = "";
    public string WeightRangeKg { get; set; } = "";
}
