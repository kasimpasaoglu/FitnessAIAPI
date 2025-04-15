using System;
using System.Collections.Generic;

namespace API.DMO;

public partial class CompletedExercise
{
    public Guid CompletionId { get; set; }

    public Guid PlanId { get; set; }

    public string ExerciseName { get; set; } = null!;

    public DateTime CompletedAt { get; set; }

    public virtual WorkoutPlan Plan { get; set; } = null!;
}
