
using FolioWebAPI.Mappers;
using Folio.Core.Application.Services;

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

            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Middlewares Area

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
