namespace MusicTracker.Models;

public class Album
{
    public int Id { get; set; }
    public string Title { get; set; } = ""; 
    public string? Review { get; set; } 
    public int DurationMinutes { get; set; } 
    public DateOnly ReleaseDate { get; set; } 
    public DateOnly? ListenedOn { get; set; } 
    public bool IsFavorite { get; set; } 
    public AlbumFormat Format { get; set; } 
}