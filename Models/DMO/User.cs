using System;
using System.Collections.Generic;

namespace API.DMO;

public partial class User
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public int Age { get; set; }

    public double HeightCm { get; set; }

    public double WeightKg { get; set; }

    public string Gender { get; set; } = null!;

    public string Goal { get; set; } = null!;

    public string AvailableDays { get; set; } = null!;

    public bool HasHealthIssues { get; set; }

    public string? MedicationsUsing { get; set; }

    public virtual ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();
}
