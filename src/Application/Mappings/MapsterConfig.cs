using Application.DTOs.Class;
using Domain.Entities;
using Mapster;

namespace Application.Mappings
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Class, ClassInfoDto>
                .NewConfig()
                .Map(dest => dest.StudentCount, src => src.Enrollments.Count);
        }
    }
}
