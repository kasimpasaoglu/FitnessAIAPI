using FluentValidation;

public class UserVM
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public int Age { get; set; }
    public double HeightCm { get; set; }
    public double WeightKg { get; set; }
    public string Gender { get; set; } = null!;
    public string Goal { get; set; } = null!;
    public List<string> AvailableDays { get; set; } = null!;
    public bool HasHealthIssues { get; set; }
    public List<string>? MedicationsUsing { get; set; }
}