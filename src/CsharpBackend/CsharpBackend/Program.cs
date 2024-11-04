using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CsharpBackend.Data;

namespace CsharpBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<CsharpBackendContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CsharpBackendContext") ?? throw new InvalidOperationException("Connection string 'CsharpBackendContext' not found.")));

            // Add services to the container.

            builder.Services.AddControllers();
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


            app.MapControllers();

            app.Run();
        }
    }
}
