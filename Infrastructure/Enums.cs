using System.ComponentModel.DataAnnotations;

public enum FitnessGoal
{
    [Display(Name = "Gain Muscle Mass")]
    GAIN_MUSCLE_MASS,

    [Display(Name = "Lose Weight")]
    LOSE_WEIGHT,

    [Display(Name = "Burn Fat")]
    BURN_FAT,

    [Display(Name = "Improve Endurance")]
    IMPROVE_ENDURANCE,

    [Display(Name = "Build Strength")]
    BUILD_STRENGTH,

    [Display(Name = "Maintain Fitness")]
    MAINTAIN_FITNESS,

    [Display(Name = "Improve Flexibility")]
    IMPROVE_FLEXIBILITY,

    [Display(Name = "Improve Mobility")]
    IMPROVE_MOBILITY,

    [Display(Name = "Improve Cardiovascular Health")]
    IMPROVE_CARDIOVASCULAR_HEALTH,

    [Display(Name = "Rehabilitation Training")]
    REHABILITATION_TRAINING,

    [Display(Name = "Boost Mental Well-being")]
    BOOST_MENTAL_WELL_BEING,
}
public enum AvailableDay
{
    [Display(Name = "Monday")]
    MONDAY,

    [Display(Name = "Tuesday")]
    TUESDAY,

    [Display(Name = "Wednesday")]
    WEDNESDAY,

    [Display(Name = "Thursday")]
    THURSDAY,

    [Display(Name = "Friday")]
    FRIDAY,

    [Display(Name = "Saturday")]
    SATURDAY,

    [Display(Name = "Sunday")]
    SUNDAY
}

public enum ExperienceLevel
{
    [Display(Name = "Beginner")]
    BEGINNER,

    [Display(Name = "Intermediate")]
    INTERMEDIATE,

    [Display(Name = "Advanced")]
    ADVANCED
}

public enum Gender
{
    [Display(Name = "Male")]
    MALE,
    [Display(Name = "Female")]
    FEMALE
}