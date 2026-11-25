namespace MusicTracker.Api;

public record AlbumDto(
    int Id,
    string Title,
    string Artist,
    string Genre,
    int DurationMinutes,
    DateOnly ReleaseDate
);