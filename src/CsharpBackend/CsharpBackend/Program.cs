using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CsharpBackend.Data;
using CsharpBackend.NeuralNetwork;
using CsharpBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CsharpBackend.Repository;
using Microsoft.Identity.Client;

namespace CsharpBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var myPolicy = "myPolicy";

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors();

            builder.Services.AddDbContext<CsharpBackendContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CsharpBackendContext") ?? throw new InvalidOperationException("Connection string 'CsharpBackendContext' not found.")));

            /*            builder.Services.AddCors(options =>
                        {
                            options.AddPolicy(name: myPolicy,
                                policy =>
                                {
                                    policy.AllowAnyHeader()
                                          .AllowAnyMethod()
                                            .AllowAnyOrigin();
                                        //.SetIsOriginAllowedToAllowWildcardSubdomains();
                                });
                        });*/



            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<INetworkManager, NetworkManager>();
            builder.Services.AddScoped<IImageRepository, ImageRepository>();
            builder.Services.Configure<AppConfig>(builder.Configuration.GetSection("AppConfig"));



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
