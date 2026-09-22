using Microsoft.EntityFrameworkCore;
using MusicTracker.Models;

namespace MusicTracker.Data;

public class MusicTrackerContext : DbContext
{
    public MusicTrackerContext(DbContextOptions<MusicTrackerContext> options)
        : base(options) { }

    public DbSet<Album> Albums => Set<Album>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Метод пустий, в моделі Album немає типу decimal
    }
}