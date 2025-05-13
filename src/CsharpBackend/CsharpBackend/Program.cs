using Microsoft.EntityFrameworkCore;
using CsharpBackend.Data;
using CsharpBackend.NeuralNetwork;
using CsharpBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CsharpBackend.Repository;
using CsharpBackend.Config;
using CsharpBackend.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CsharpBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors();

            builder.Services.AddDbContext<CsharpBackendContext>(options =>
                options.UseSqlServer(
                    builder.Configuration
                    .GetConnectionString("CsharpBackendContext") ??
                    throw new InvalidOperationException("Connection string 'CsharpBackendContext' not found.")
                    )
                );

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<INetworkManager, NetworkManager>();
            builder.Services.AddScoped<IImageRepository, ImageRepository>();
            builder.Services.Configure<AppConfig>(builder.Configuration.GetSection("AppConfig"));

            var appConfig = builder.Configuration.GetSection("AppConfig").Get<AppConfig>();
            if (appConfig == null)
                throw new InvalidOperationException("There are no app config");

            var poreClassesJson = File.ReadAllText(appConfig.PathToPoreClasses);
            var poreClasses = JsonConvert.DeserializeObject<List<PoreClasses>>(poreClassesJson)?.FirstOrDefault();
            var poreColorsJson = File.ReadAllText(appConfig.PathToPoreColors);
            var poreColors = JsonConvert.DeserializeObject<PoreColors>(poreColorsJson);


            DataConverter.Init(poreClasses, poreColors);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {

                            ValidateIssuer = true,
                            ValidIssuer = AuthOptions.Issuer,
                            ValidateAudience = true,
                            ValidAudience = AuthOptions.Audience,
                            ValidateLifetime = true,
                            IssuerSigningKey = AuthOptions.SigningKey,
                            ValidateIssuerSigningKey = true,
                        };
                    });




            var app = builder.Build();
            //app.UseHttpsRedirection();
            app.UseCors(
                    options => options
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                );

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            
          
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

    }
}
