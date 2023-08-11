using System;
using AutoMapper;
using Refugio.Dto;
using Refugio.Models;

namespace Refugio.Mapper
{
    public class MapperDto : Profile
    {
        public MapperDto()
        {
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<Group, GroupDto>().ReverseMap();

            CreateMap<PointDto, PointDto>().ReverseMap();
        }
    }
}