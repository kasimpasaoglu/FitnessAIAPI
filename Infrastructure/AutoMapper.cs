using System.Text.Json;
using System.Text.Json.Serialization;
using API.DMO;
using AutoMapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {

        var jsonEnumOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };



        CreateMap<UserDTO, User>()
            .ForMember(dest => dest.AvailableDays,
                opt => opt.MapFrom(src => JsonSerializer.Serialize(src.AvailableDays, jsonEnumOptions)))
            .ForMember(dest => dest.MedicationsUsing,
                opt => opt.MapFrom((src, dest) => JsonSerializer.Serialize(src.MedicationsUsing)))
            .ForMember(dest => dest.Goal,
                opt => opt.MapFrom((src, dest) => src.Goal.ToString()));

        CreateMap<User, UserDTO>()
            .ForMember(dest => dest.AvailableDays,
                opt => opt.MapFrom(src => JsonSerializer.Deserialize<List<AvailableDay>>(src.AvailableDays ?? "[]", jsonEnumOptions)!))
            .ForMember(dest => dest.MedicationsUsing,
                opt => opt.MapFrom((src, dest) => JsonSerializer.Deserialize<List<string>>(src.MedicationsUsing ?? "[]")!))
            .ForMember(dest => dest.Goal,
                opt => opt.MapFrom(src => Enum.Parse<FitnessGoal>(src.Goal)));


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