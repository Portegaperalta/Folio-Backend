
using Folio.Core.Application.Services;
using FolioWebAPI.Mappers;
using Folio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FolioWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Services Area

            builder.Services.AddControllers();

            builder.Services.AddScoped<FolderMapper>();
            builder.Services.AddScoped<BookmarkMapper>();
            builder.Services.AddScoped<FolderService>();
            builder.Services.AddScoped<BookmarkService>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuthentication().AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = 
                     new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"]!)),
                     ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddOpenApi();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Middlewares Area

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseSwagger();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
