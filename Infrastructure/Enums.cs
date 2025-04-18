using System.ComponentModel.DataAnnotations;

public enum FitnessGoal
{
    Unknown = 0,
    [Display(Name = "Gain Muscle Mass")]
    GainMuscleMass,

    [Display(Name = "Lose Weight")]
    LoseWeight,

    [Display(Name = "Burn Fat")]
    BurnFat,

    [Display(Name = "Improve Endurance")]
    ImproveEndurance,

    [Display(Name = "Build Strength")]
    BuildStrength,

    [Display(Name = "Maintain Fitness")]
    MaintainFitness,

    [Display(Name = "Improve Flexibility")]
    ImproveFlexibility,

    [Display(Name = "Improve Mobility")]
    ImproveMobility,

    [Display(Name = "Improve Cardiovascular Health")]
    ImproveCardiovascularHealth,

    [Display(Name = "Rehabilitation Training")]
    RehabilitationTraining,

    [Display(Name = "Boost Mental Well-being")]
    BoostMentalWellBeing
}
public enum AvailableDay
{
    [Display(Name = "Monday")]
    Monday,

    [Display(Name = "Tuesday")]
    Tuesday,

    [Display(Name = "Wednesday")]
    Wednesday,

    [Display(Name = "Thursday")]
    Thursday,

    [Display(Name = "Friday")]
    Friday,

    [Display(Name = "Saturday")]
    Saturday,

    [Display(Name = "Sunday")]
    Sunday
}