using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Musix4u_API.Services;

namespace Musix4u_API.Infrastructures
{
    public static class HostExtension
    {
        public static async Task<IHost> SeedDataAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetService<AppDbContext>();
            var configuration = services.GetService<IConfiguration>();

            var initDb = configuration.GetSection("Initialize").Get<bool>();
            if (initDb)
            {
                context.Database.EnsureDeleted();
                context.Database.Migrate();
            }
            else
            {
                context.Database.Migrate();
            }

            return host;
        }
    }
}