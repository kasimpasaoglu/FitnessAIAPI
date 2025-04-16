using FluentValidation;

public class RegisterRequest
{
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

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    private readonly string[] allowedGenders = ["Male", "male", "MALE", "Female", "female", "FEMALE"];
    public RegisterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(3, 15).WithMessage("Name must be between 3 and 15 characters long.")
            .Matches(@"^[a-zA-Z]+$").WithMessage("Name must contain only letters.");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Surname is required.")
            .Length(3, 15).WithMessage("Surname must be between 3 and 15 characters long.")
            .Matches(@"^[a-zA-Z]+$").WithMessage("Surname must contain only letters.");

        RuleFor(x => x.Age)
            .NotEmpty().WithMessage("Age is required.")
            .InclusiveBetween(18, 100).WithMessage("Age must be between 18 and 100 years.");

        RuleFor(x => x.HeightCm)
            .NotEmpty().WithMessage("Height is required.")
            .InclusiveBetween(50, 210).WithMessage("Height must be between 50 and 210 cm.");

        RuleFor(x => x.WeightKg)
            .NotEmpty().WithMessage("Weight is required.")
            .InclusiveBetween(30, 200).WithMessage("Weight must be between 30 and 200 kg.");

        RuleFor(x => x.Gender)
            .Must(g => allowedGenders.Contains(g)).WithMessage("Gender must be either 'Male' or 'Female'.");

        RuleFor(x => x.Goal)
            .NotEmpty().WithMessage("Goal is required.")
            .Must(g => Constants.Goals.Contains(g)).WithMessage($"Goal must be one of the following: {string.Join(", ", Constants.Goals)}.");

        RuleFor(x => x.AvailableDays)
            .NotEmpty().WithMessage("Available days are required.");
        RuleForEach(x => x.AvailableDays)
            .Must(Constants.Days.Contains)
            .WithMessage($"Each day must be one of the following: {string.Join(", ", Constants.Days)}.");

        RuleFor(x => x.HasHealthIssues)
            .NotNull().WithMessage("Health issues status is required.");

        RuleFor(x => x.MedicationsUsing)
            .NotEmpty().WithMessage("If you have health issues, please specify the medications you are using.").When(x => x.HasHealthIssues);
    }
}