using System.Net;
using AutoMapper;
using Mapper.Models;
using Mapper.Models.DTOs;
using Mapper.Repository;
using Mapper.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.DependencyInjection;




namespace Mapper;

public class Program
{

    private static MapperConfiguration _mapperConfiguration = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<User, UserDTO>(); ;
                        });
    private static UserService _userService;
    private static IRepository _cosmosRepository;
    private static ILogger<Program> _logger;

    public async static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
       // Add this line

        // ...

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();



        // ...

        builder.Services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = "InstrumentationKey=ae4da2fa-70de-466f-a152-0524d48bcba9;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/;ApplicationId=475cb098-c206-4648-9716-bdcc8271f85b";
        });

        builder.Services.AddLogging(builder => builder.AddApplicationInsights());

        // ...
        var configuration = builder.Configuration.AddJsonFile("appsettings.Development.json")
            .AddEnvironmentVariables(Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING"))
            .Build();

        builder.Services.AddSingleton<IConfiguration>(configuration);
        var cosmosDBClientFactory = new CosmosDBClientFactory();
        _cosmosRepository = new CosmosRepository(configuration, cosmosDBClientFactory);

        _userService = new UserService(_cosmosRepository,
                                       _mapperConfiguration.CreateMapper());
        var app = builder.Build();

        // Configure the HTTP request pipeline.

            app.UseSwagger();
          app.UseSwaggerUI();

        app.UseHttpsRedirection();



        //Endpoints

        app.MapGet("/user", (HttpContext httpContext,  [FromQuery] string id) =>
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



        app.MapGet("/users",  (HttpContext httpContext, ILogger<Program> Logger) =>
        {
           _logger = Logger;

            var dto = _userService.GetAllUsers();
            _logger.LogError("Get all users error");
            _logger.LogInformation("Get all users information");

            return dto.Result;
        }).WithName("GetAllUsersDTO").WithOpenApi();

        app.Run();

    }
}
