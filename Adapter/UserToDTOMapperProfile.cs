
using AutoMapper;
using Mapper.Models;
using Mapper.Models.DTOs;

namespace Mapper.Adapter
{
    public class UserToDTOMapperProfile : Profile
    {
        public UserToDTOMapperProfile()
        {
            CreateMap<User, UserDTO>();
        }


    }
}