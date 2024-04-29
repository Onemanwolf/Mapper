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
using Microsoft.ApplicationInsights.Extensibility;




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

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddLogging(builder => builder.AddApplicationInsights());
        var configuration = builder.Configuration.AddJsonFile("appsettings.Development.json")
            .AddEnvironmentVariables(Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING"))
            .Build();

             builder.Services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = configuration.GetConnectionString("APPLICATIONINSIGHTS_CONNECTION_STRING");
        });

        TelemetryConfiguration  telemetryConfiguration = TelemetryConfiguration.CreateDefault();
        telemetryConfiguration.ConnectionString = configuration.GetConnectionString("APPLICATIONINSIGHTS_CONNECTION_STRING");
        var logger = LoggerFactory.Create(builder => builder.AddApplicationInsights(telemetryConfiguration => {telemetryConfiguration.ConnectionString})).CreateLogger<CosmosRepository>();
        builder.Services.AddSingleton<IConfiguration>(configuration);
        var cosmosDBClientFactory = new CosmosDBClientFactory();

        _cosmosRepository = new CosmosRepository(configuration, cosmosDBClientFactory, logger);

        _userService = new UserService(_cosmosRepository,
                                       _mapperConfiguration.CreateMapper());
        var app = builder.Build();

        // Configure the HTTP request pipeline.

          app.UseSwagger();
          app.UseSwaggerUI();

          app.UseHttpsRedirection();



        //Endpoints

        app.MapGet("/user/{id}", (HttpContext httpContext,  [FromQuery] string id) =>
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



        app.MapGet("/user",  (HttpContext httpContext, ILogger<Program> Logger) =>
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
