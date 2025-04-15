public class UserDTO
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public double HeightCm { get; set; }
    public double WeightKg { get; set; }
    public string Gender { get; set; } = null!;
    public string Goal { get; set; } = null!;
    public string AvailableDays { get; set; } = null!;
    public bool HasHealthIssues { get; set; }
    public string? MedicationsUsing { get; set; }
}