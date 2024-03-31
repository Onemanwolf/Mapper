using System.Net;
using Mapper.Models;
using Mapper.Repository;
using Mapper.Services;
using Microsoft.AspNetCore.Mvc;


namespace Mapper;

public class Program
{

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
        builder.Services.AddSingleton<ICosmosRepository, CosmosRepository>();
        var configuration = builder.Configuration.AddJsonFile("appsettings.Development.json").AddEnvironmentVariables(Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING") ).Build();
        builder.Services.AddSingleton<IConfiguration>(configuration);
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapGet("/user", (HttpContext httpContext) =>
        {
            var service = new UserService(new CosmosRepository(configuration));
            var dto = service.GetUser("John");
            return dto.Result;
        })
            .WithName("GetUserDTO")
            .WithOpenApi();


        app.MapPost("/user", (HttpContext httpContext, [FromBody] UserRequest user) =>
        {
            var service = new UserService(new CosmosRepository(configuration));
            var dto = service.CreateUser(user.Name, user.Email);
            return HttpStatusCode.Created;}).WithName("CreateUserDTO").WithOpenApi();
        app.Run();
    }
}
