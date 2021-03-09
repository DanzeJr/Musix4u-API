using Microsoft.EntityFrameworkCore;
using Musix4u_API.Models;

namespace Musix4u_API.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Track> Track { get; set; }

        public DbSet<Playlist> Playlist { get; set; }

        public DbSet<PlaylistTrack> PlaylistTrack { get; set; }

        public DbSet<User> User { get; set; }
    }
}