using Microsoft.EntityFrameworkCore;
using CsharpBackend.Data;
using CsharpBackend.NeuralNetwork;
using CsharpBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CsharpBackend.Repository;
using CsharpBackend.Config;
using CsharpBackend.Utils;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;

namespace CsharpBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

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

            if (poreClasses == null || poreColors == null)
                throw new InvalidOperationException("Error while loading metadata (pore colors and pore classes)");

            DataConverter.Init(poreClasses, poreColors);
            PorosityAnalyzer.Init(poreClasses, poreColors);


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

            // Укажем путь к твоей папке dist
            var distPath = appConfig.PathToWebDist;

            // Отдаем index.html при заходе на корень
            app.UseDefaultFiles(new DefaultFilesOptions
            {
                DefaultFileNames = new List<string> { "index.html" },
                FileProvider = new PhysicalFileProvider(distPath)
            });

            // Отдаем все остальные файлы из dist (js, css и т.д.)
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(distPath),
                RequestPath = ""  // чтобы запросы без префикса шли сюда
            });

            app.UseCors("MyPolicy");

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
          
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapFallback(async context =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(Path.Combine(distPath, "index.html"));
            });

            app.Run();
        }

    }
}
