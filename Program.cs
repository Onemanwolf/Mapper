using Mapper.Services;


namespace Mapper;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Add AutoMapper
        builder.Services.AddAutoMapper(typeof(Program).Assembly);
        // Add other services

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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
            var service = new UserService();
            var dto = service.GetUser("John"); return dto.Result;
        })
            .WithName("GetUserDTO")
            .WithOpenApi();
        app.Run();
    }
}
