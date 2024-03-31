using System.Net;
using AutoMapper;
using Mapper.Models;
using Mapper.Models.DTOs;
using Mapper.Repository;
using Mapper.Services;
using Microsoft.AspNetCore.Mvc;


namespace Mapper;

public class Program
{
    private static MapperConfiguration _mapperConfiguration = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<User, UserDTO>(); ;
                        });
    private static UserService _userService;
    private static IRepository _cosmosRepository;
    public async static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Add AutoMapper
        builder.Services.AddAutoMapper(typeof(Program).Assembly);
        // Add other services

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckl
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<IMapper>(sp =>
                    {
                        _mapperConfiguration = new MapperConfiguration(cfg =>
                       {
                           cfg.CreateMap<User, UserDTO>();
                       });

                        return _mapperConfiguration.CreateMapper();
                    });
        builder.Services.AddSingleton<IRepository, CosmosRepository>();
        var configuration = builder.Configuration.AddJsonFile("appsettings.Development.json")
        .AddEnvironmentVariables(Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING"))
        .Build();
        builder.Services.AddSingleton<IConfiguration>(configuration);
        _cosmosRepository = new CosmosRepository(configuration);
        _userService = new UserService(_cosmosRepository,
                                       _mapperConfiguration.CreateMapper());
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        //Endpoints

        app.MapGet("/user", (HttpContext httpContext, [FromQuery] string id) =>
        {
            var dto = _userService.GetUserById(id);
            return dto.Result;
        })
            .WithName("GetUserDTO")
            .WithOpenApi();


        app.MapPost("/user", (HttpContext httpContext, [FromBody] UserRequest user) =>
        {
            //fail fast validation
            //fail fast avoid allowing the request to go through the entire call cain
            if (user.NotValid())
            {
                return HttpStatusCode.BadRequest;
            }
            // if (String.IsNullOrEmpty(user.Name) || String.IsNullOrEmpty(user.Email))
            // {
            //     return HttpStatusCode.BadRequest;
            // }

            var dto = _userService.CreateUser(user.Name, user.Email);
            return HttpStatusCode.Created;
        }).WithName("CreateUserDTO").WithOpenApi();


        app.MapPut("/user", (HttpContext httpContext, [FromBody] UserUpdateRequest user) =>
        {
            if (String.IsNullOrEmpty(user.Id) || String.IsNullOrEmpty(user.Name) || String.IsNullOrEmpty(user.Email))
            {
                return HttpStatusCode.BadRequest;
            }
            var dto = _userService.UpdateUser(user.Id, user.Name, user.Email);
            return HttpStatusCode.Created;
        }).WithName("UpdateUserDTO").WithOpenApi();



        app.MapGet("/users", (HttpContext httpContext) =>
        {
            var dto = _userService.GetAllUsers();
            return dto.Result;
        }).WithName("GetAllUsersDTO").WithOpenApi();

        app.Run();

    }
}
