using DigitalSignService.DAL.Entities;
using DigitalSignService.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace DigitalSignService.DAL
{
    public class DataBaseContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DataBaseContext()
        {
        }

        public DataBaseContext(DbContextOptions<DataBaseContext> option, IConfiguration configuration) : base(option)
        {
            _configuration = configuration;
        }

        public DbSet<Template> Template { get; set; }
        public DbSet<TemplatePaper> FileVersions { get; set; }
        public DbSet<TemplatePaperUserSign> TemplatePaperUserSigns { get; set; }
        public DbSet<PaperSize> PaperSizes { get; set; }
        public DbSet<HistorySign> HistorySigns { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //string connectionString = _configuration.GetConnectionString("postgres");
            //optionsBuilder.UseNpgsql("User ID =admin;Password=admin_password;Server=localhost;Port=5434;Database=dpu_digital_sign");
            //optionsBuilder.UseNpgsql(connectionString);
            //optionsBuilder.UseNpgsql("User ID =admin;Password=admin_password;Server=localhost;Port=5436;Database=dpu_webgis2;Pooling=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TemplatePaperUserSign>()
                .Property(s => s.UserSignPositions)
                .HasColumnType("jsonb")
                .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<UserSignPos[]>(v, (JsonSerializerOptions)null));

            modelBuilder.Entity<HistorySign>()
                .Property(s => s.UserSignPositions)
                .HasColumnType("jsonb")
                .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<UserSignPos[]>(v, (JsonSerializerOptions)null));

            base.OnModelCreating(modelBuilder);
        }
    }
}
