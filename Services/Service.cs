
using AutoMapper;
using Mapper.Models;
using Mapper.Models.DTOs;

namespace Mapper.Services
{
    public class UserService
    {
        private readonly IMapper mapper;

        public UserService()
        {
            mapper = new MapperConfiguration(cfg =>
           {
               cfg.CreateMap<User, UserDTO>();
           }).CreateMapper();
        }
        public async Task<UserDTO> GetUser(string name)
        {
            //Given a user name,
            //When I get the user from the database map the user to a UserDTO
            //Then, return the UserDTO
            // TODO: Get user from database
            User user = new User { Name = "John", Email = "john@example.com" };
            var dto = await MapUserToDTO(user);
            return dto;
        }

        private async Task<UserDTO> MapUserToDTO(User user)
        {
            return await Task.Run(() => { UserDTO dto = mapper.Map<UserDTO>(user); return dto; });
        }
    }
}