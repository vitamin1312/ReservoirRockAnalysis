using Microsoft.EntityFrameworkCore;
using CsharpBackend.Models;
using CsharpBackend.Models.DTO;

namespace CsharpBackend.Data
{
    public class CsharpBackendContext : DbContext
    {
        public CsharpBackendContext (DbContextOptions<CsharpBackendContext> options)
            : base(options)
        {
        }

        public DbSet<CsharpBackend.Models.CoreSampleImage> CoreSampleImage { get; set; } = default!;
        public DbSet<CsharpBackend.Models.ImageInfo> ImageInfo { get; set; } = default!;
        public DbSet<CsharpBackend.Models.Field> Field { get; set; } = default!;
        public DbSet<CsharpBackend.Models.User> User { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<AuthOptions>();
            modelBuilder.Ignore<ImageInfoDTO>();
        }
    }
}
