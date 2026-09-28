using Microsoft.EntityFrameworkCore;
using MusicTracker.Models;

namespace MusicTracker.Data;

public class MusicTrackerContext : DbContext
{
    public MusicTrackerContext(DbContextOptions<MusicTrackerContext> options)
        : base(options) { }

    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Genre> Genres => Set<Genre>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Album>()
            .HasIndex(x => new { x.Title, x.ReleaseDate })
            .IsUnique();

        b.Entity<Album>()
            .HasOne(x => x.Artist)
            .WithMany()
            .HasForeignKey(x => x.ArtistId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Album>()
            .HasOne(x => x.Genre)
            .WithMany()
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}