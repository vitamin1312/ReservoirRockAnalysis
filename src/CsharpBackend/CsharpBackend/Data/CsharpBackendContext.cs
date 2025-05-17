using Microsoft.EntityFrameworkCore;
using CsharpBackend.Models;
using CsharpBackend.Models.DTO;
using System.Reflection.Metadata;

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
        public DbSet<CsharpBackend.Models.PoreInfo> PoreInfo { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<AuthOptions>();
            modelBuilder.Ignore<ImageInfoDTO>();

            modelBuilder.Entity<Field>()
                .HasMany(f => f.FieldImages)
                .WithOne(i => i.Field)
                .HasForeignKey(i => i.FieldId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
