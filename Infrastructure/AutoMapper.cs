using System.Text.Json;
using API.DMO;
using AutoMapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserVM, UserDTO>()
            .ForMember(dest => dest.AvailableDays,
                opt => opt.MapFrom((src, dest) => JsonSerializer.Serialize(src.AvailableDays)));

        CreateMap<UserDTO, UserVM>()
            .ForMember(dest => dest.AvailableDays,
                opt => opt.MapFrom((src, dest) => JsonSerializer.Deserialize<List<string>>(src.AvailableDays ?? "[]")!));

        CreateMap<User, UserDTO>().ReverseMap();
    }
}