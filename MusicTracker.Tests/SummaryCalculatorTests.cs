using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MusicTracker.Data;
using MusicTracker.Models;
using MusicTracker.Services;

namespace MusicTracker.Tests;

public class SummaryCalculatorTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly MusicTrackerContext _db;

    public SummaryCalculatorTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<MusicTrackerContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new MusicTrackerContext(options);
        _db.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    private static Album CreateAlbum(
        string title,
        Artist artist,
        Genre genre,
        int durationMinutes,
        DateOnly releaseDate) =>
        new()
        {
            Title = title,
            TitleSearch = title.ToLowerInvariant(),
            Artist = artist,
            Genre = genre,
            DurationMinutes = durationMinutes,
            ReleaseDate = releaseDate,
            ListenedOn = releaseDate,
            Format = AlbumFormat.Digital,
            Review = "test"
        };

    [Fact]
    public async Task ByArtistForEmptySetReturnsNoRows()
    {
        var result = await SummaryCalculator.ByArtistAsync(_db.Albums);

        Assert.Empty(result);
    }

    [Fact]
    public async Task BooksOfOneAuthorMergeIntoSingleRowAndSumMinutes()
    {
        var artist = new Artist { Name = "Playboi Carti" };
        var genre = new Genre { Name = "Rage" };
        _db.Artists.Add(artist);
        _db.Genres.Add(genre);

        _db.Albums.AddRange(
            CreateAlbum("Die Lit", artist, genre, 57, new DateOnly(2018, 5, 11)),
            CreateAlbum("Whole Lotta Red", artist, genre, 63, new DateOnly(2020, 12, 25)));
        await _db.SaveChangesAsync();

        var result = await SummaryCalculator.ByArtistAsync(_db.Albums);

        var row = Assert.Single(result);
        Assert.Equal("Playboi Carti", row.Name);
        Assert.Equal(2, row.Count);
        Assert.Equal(120, row.TotalMinutes);
    }

    [Fact]
    public async Task ByMonthSeparatesDifferentMonthsIntoDistinctRows()
    {
        var artist = new Artist { Name = "Artist A" };
        var genre = new Genre { Name = "Pop" };
        _db.Artists.Add(artist);
        _db.Genres.Add(genre);

        _db.Albums.AddRange(
            CreateAlbum("Album Jan", artist, genre, 40, new DateOnly(2024, 1, 15)),
            CreateAlbum("Album Mar", artist, genre, 50, new DateOnly(2024, 3, 20)));
        await _db.SaveChangesAsync();

        var result = await SummaryCalculator.ByMonthAsync(_db.Albums);

        Assert.Equal(2, result.Count);
        Assert.Equal("01.2024", result[0].Name);
        Assert.Equal(40, result[0].TotalMinutes);
        Assert.Equal("03.2024", result[1].Name);
        Assert.Equal(50, result[1].TotalMinutes);
    }

    [Fact]
    public async Task SingleAlbumProducesSingleRowWithCountOne()
    {
        var artist = new Artist { Name = "Solo Artist" };
        var genre = new Genre { Name = "Rock" };
        _db.Artists.Add(artist);
        _db.Genres.Add(genre);

        _db.Albums.Add(CreateAlbum("Single Album", artist, genre, 45, new DateOnly(2025, 6, 1)));
        await _db.SaveChangesAsync();

        var result = await SummaryCalculator.ByArtistAsync(_db.Albums);

        var row = Assert.Single(result);
        Assert.Equal(1, row.Count);
        Assert.Equal(45, row.TotalMinutes);
    }

    [Fact]
    public async Task TotalAsyncReturnsOverallCountAndMinutesSum()
    {
        var a1 = new Artist { Name = "Artist 1" };
        var a2 = new Artist { Name = "Artist 2" };
        var genre = new Genre { Name = "Electronic" };
        _db.AddRange(a1, a2, genre);

        _db.Albums.AddRange(
            CreateAlbum("One", a1, genre, 30, new DateOnly(2024, 2, 1)),
            CreateAlbum("Two", a2, genre, 70, new DateOnly(2024, 2, 10)));
        await _db.SaveChangesAsync();

        var total = await SummaryCalculator.TotalAsync(_db.Albums);

        Assert.Equal("Усього", total.Name);
        Assert.Equal(2, total.Count);
        Assert.Equal(100, total.TotalMinutes);
    }
}