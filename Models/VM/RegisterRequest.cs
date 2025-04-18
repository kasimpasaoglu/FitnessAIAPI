using FluentValidation;

public class RegisterRequest
{
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public int Age { get; set; }
    public double HeightCm { get; set; }
    public double WeightKg { get; set; }
    public string Gender { get; set; } = null!;
    public FitnessGoal Goal { get; set; }
    public List<AvailableDay> AvailableDays { get; set; } = null!;
    public bool HasHealthIssues { get; set; }
    public List<string>? MedicationsUsing { get; set; }
}
public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    private readonly string[] allowedGenders = ["Male", "male", "MALE", "Female", "female", "FEMALE"];

    public RegisterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(3, 15).WithMessage("Name must be between 3 and 15 characters.")
            .Matches(@"^[a-zA-Z]+$").WithMessage("Name must contain only letters.");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Surname is required.")
            .Length(3, 15).WithMessage("Surname must be between 3 and 15 characters.")
            .Matches(@"^[a-zA-Z]+$").WithMessage("Surname must contain only letters.");

        RuleFor(x => x.Age)
            .InclusiveBetween(18, 100).WithMessage("Age must be between 18 and 100.");

        RuleFor(x => x.HeightCm)
            .InclusiveBetween(50, 210).WithMessage("Height must be between 50 and 210 cm.");

        RuleFor(x => x.WeightKg)
            .InclusiveBetween(30, 200).WithMessage("Weight must be between 30 and 200 kg.");

        RuleFor(x => x.Gender)
            .Must(g => allowedGenders.Contains(g))
            .WithMessage("Gender must be 'Male' or 'Female'.");

        RuleFor(x => x.Goal)
            .Must(g => g != FitnessGoal.Unknown).WithMessage("Fitness goal is required.")
            .IsInEnum().WithMessage("Invalid goal.");

        RuleFor(x => x.AvailableDays)
            .NotEmpty().WithMessage("Available days are required.");
        RuleForEach(x => x.AvailableDays)
            .IsInEnum().WithMessage("Invalid day of the week.");

        RuleFor(x => x.HasHealthIssues)
            .NotNull().WithMessage("Health issues status is required.");

        RuleFor(x => x.MedicationsUsing)
            .NotEmpty().WithMessage("If you have health issues, please list the medications.")
            .When(x => x.HasHealthIssues);
    }
}
