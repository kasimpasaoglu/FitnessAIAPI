using System.Text.Json;
using API.DMO;
using AutoMapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserDTO, User>()
            .ForMember(dest => dest.AvailableDays,
                opt => opt.MapFrom((src, dest) => JsonSerializer.Serialize(src.AvailableDays)))
            .ForMember(dest => dest.MedicationsUsing,
                opt => opt.MapFrom((src, dest) => JsonSerializer.Serialize(src.MedicationsUsing)));

        CreateMap<User, UserDTO>()
            .ForMember(dest => dest.AvailableDays,
                opt => opt.MapFrom((src, dest) => JsonSerializer.Deserialize<List<string>>(src.AvailableDays ?? "[]")!))
            .ForMember(dest => dest.MedicationsUsing,
                opt => opt.MapFrom((src, dest) => JsonSerializer.Deserialize<List<string>>(src.MedicationsUsing ?? "[]")!));


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