public class WorkoutProgramVM
{
    public int ProgramDurationWeeks { get; set; }
    public List<WorkoutDayVM> WeeklySchedule { get; set; }
    public List<RecoveryDayVM> ActiveRecoveryDays { get; set; }
    public string PersonalNote { get; set; }
}

public class WorkoutDayVM
{
    public string Day { get; set; }
    public string Warmup { get; set; }
    public string Focus { get; set; }
    public List<ExerciseVM> Exercises { get; set; }
    public string Cooldown { get; set; }
}

public class ExerciseVM
{
    public string Name { get; set; }
    public int Sets { get; set; }
    public int Reps { get; set; }
    public string WeightRangeKg { get; set; }
}

public class RecoveryDayVM
{
    public string Day { get; set; }
    public string Activity { get; set; }
}
