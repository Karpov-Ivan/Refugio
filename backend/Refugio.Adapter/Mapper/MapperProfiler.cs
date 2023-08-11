using System;
using AutoMapper;
using Refugio.Models;

namespace Refugio.Adapter.Mapper
{
    public class MappingDatabaseProfile : Profile
    {
        public MappingDatabaseProfile()
        {
            CreateMap<User, DataBase.Models.Models.User>().ReverseMap();

            CreateMap<Group, DataBase.Models.Models.Group>().ReverseMap();
        }
    }
}