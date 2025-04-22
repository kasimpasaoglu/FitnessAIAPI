public class UserDTO
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public int Age { get; set; }
    public double HeightCm { get; set; }
    public double WeightKg { get; set; }
    public Gender Gender { get; set; }
    public FitnessGoal Goal { get; set; }
    public List<AvailableDay> AvailableDays { get; set; } = null!;
    public bool HasHealthIssues { get; set; }
    public List<string>? MedicationsUsing { get; set; }
    public ExperienceLevel ExperienceLevel { get; set; }
}