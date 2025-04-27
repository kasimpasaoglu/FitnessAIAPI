using System.Text.Json;
using System.Text.Json.Serialization;
using API.DMO;
using AutoMapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        var relaxedJsonOptions = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter() }
        };

        var jsonEnumOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        CreateMap<User, UserDTO>()
            .ForMember(dest => dest.AvailableDays,
                opt => opt.MapFrom(src => JsonSerializer.Deserialize<List<AvailableDay>>(src.AvailableDays ?? "[]", jsonEnumOptions)!))
            .ForMember(dest => dest.MedicationsUsing,
                opt => opt.MapFrom((src, dest) => JsonSerializer.Deserialize<List<string>>(src.MedicationsUsing ?? "[]")!))
            .ForMember(dest => dest.Goal,
                opt => opt.MapFrom(src => Enum.Parse<FitnessGoal>(src.Goal)))
            .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => Enum.Parse<Gender>(src.Gender)))

            .ForMember(dest => dest.ExperienceLevel,
                opt => opt.MapFrom(src => Enum.Parse<ExperienceLevel>(src.ExperienceLevel)));

        CreateMap<UserDTO, User>()
            .ForMember(dest => dest.AvailableDays,
                opt => opt.MapFrom(src => JsonSerializer.Serialize(src.AvailableDays, relaxedJsonOptions)))
            .ForMember(dest => dest.MedicationsUsing,
                opt => opt.MapFrom((src, dest) => JsonSerializer.Serialize(src.MedicationsUsing, relaxedJsonOptions)))
            .ForMember(dest => dest.Goal,
                opt => opt.MapFrom((src, dest) => src.Goal.ToString()))
            .ForMember(dest => dest.Gender,
                opt => opt.MapFrom((src, dest) => src.Gender.ToString()))
            .ForMember(dest => dest.ExperienceLevel,
                opt => opt.MapFrom((src, dest) => src.ExperienceLevel.ToString()));

        CreateMap<UserVM, UserDTO>().ReverseMap();

        CreateMap<RegisterRequest, UserVM>().ReverseMap();
        CreateMap<RegisterRequest, UserDTO>().ReverseMap();



        CreateMap<WorkoutPlan, WorkoutPlanDTO>()
            .ForMember(dest => dest.ExerciseJson,
                opt => opt.MapFrom((src, dest) => JsonSerializer.Deserialize<ExerciseJsonModel>(src.ExerciseJson)));
        CreateMap<WorkoutPlanDTO, WorkoutPlan>()
            .ForMember(dest => dest.ExerciseJson,
                opt => opt.MapFrom((src, dest) => JsonSerializer.Serialize(src.ExerciseJson)));
        CreateMap<WorkoutPlanDTO, WorkoutPlanVM>().ReverseMap();
    }
}