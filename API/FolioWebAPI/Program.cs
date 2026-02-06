
using Folio.Core.Application.Services;
using Folio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;
using Folio.Core.Interfaces;
using FolioWebAPI.Services;
using Folio.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Folio.Infrastructure.Repositories;
using FolioWebAPI.Middlewares;
using Folio.Core.Application.Mappers;

namespace FolioWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // SERVICES AREA

            builder.Services.AddDataProtection();

            // controllers services
            builder.Services.AddControllers();
            
            // services, repositories and mapper services
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<FolderMapper>();
            builder.Services.AddScoped<BookmarkMapper>();
            builder.Services.AddScoped<FolderService>();
            builder.Services.AddScoped<BookmarkService>();
            builder.Services.AddScoped<IFolderRepository, FolderRepository>();
            builder.Services.AddScoped<IBookmarkRepository, BookmarkRepository>();
            builder.Services.AddTransient<ITokenGenerator, TokenGenerator>();

            // db context services
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // auth and identity services
            builder.Services.AddIdentityCore<ApplicationUser>()
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddDefaultTokenProviders();

            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<UserManager<ApplicationUser>>();
            builder.Services.AddScoped<SignInManager<ApplicationUser>>();
            builder.Services.AddHttpContextAccessor();

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
                     new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtKey"]!)),
                     ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddOpenApi();

            //swagger services
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (dbContext.Database.IsRelational())
                {
                    dbContext.Database.Migrate();
                }
            }

            // MIDDLEWARES AREA

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseSwagger();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
