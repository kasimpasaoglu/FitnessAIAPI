using System;
using System.Collections.Generic;

namespace API.DMO;

public partial class WorkoutPlan
{
    public Guid PlanId { get; set; }

    public Guid UserId { get; set; }

    public string ExerciseJson { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<CompletedExercise> CompletedExercises { get; set; } = new List<CompletedExercise>();

    public virtual User User { get; set; } = null!;
}
