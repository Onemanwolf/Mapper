
using System.Security.Cryptography;
using AutoMapper;
using Mapper.Models;
using Mapper.Models.DTOs;
using Mapper.Repository;

namespace Mapper.Services
{
    public class UserService
    {
        private readonly ICosmosRepository _cosmosRepository;
        private readonly IMapper _mapper;


        public UserService(ICosmosRepository cosmosRepository, IMapper mapper)
        {
            _cosmosRepository = cosmosRepository;



            _mapper = mapper;

        //     new MapperConfiguration(cfg =>
        //    {
        //        cfg.CreateMap<User, UserDTO>();
        //    }).CreateMapper();
        }
        public async Task<UserDTO> GetUser(string name)
        {
            //Given a user name,
            //When I get the user from the database map the user to a UserDTO
            //Then, return the UserDTO
            // TODO: Get user from database

            var randomNumber = new Random().Next();
            var userId = "custom_user2";
            User user = new User { Id = $"custom_user{randomNumber}", formId = $"custom_user{randomNumber}", Name = "John", Email = "john@example.com" };
            var dto = await MapUserToDTO(user);
            dto = await _cosmosRepository.GetUserAsync(userId);

            return dto;
        }

        public async Task<UserDTO> CreateUser(string name, string email)
        {
            //Given a user name and email,
            //When I create a new user and insert it into the database
            //Then, return the UserDTO
            var user = new User().FactorMethodUser(name, email);
            var dto = await MapUserToDTO(user);
            dto = await _cosmosRepository.InsertItemAsync(dto);
            return dto;
        }

        private async Task<UserDTO> MapUserToDTO(User user)
        {
            return await Task.Run(() => { UserDTO dto = _mapper.Map<UserDTO>(user); return dto; });
        }
    }
}