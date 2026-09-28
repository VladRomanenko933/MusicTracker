using System.ComponentModel.DataAnnotations;
namespace MusicTracker.Models;

public class Album
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Будь ласка, вкажіть назву альбому.")]
    [StringLength(100, ErrorMessage = "Назва альбому не може перевищувати 100 символів.")]
    public string Title { get; set; } = "";

    public string? Review { get; set; }

    [Range(1, 1000, ErrorMessage = "Тривалість альбому має бути від 1 до 1000 хвилин.")]
    public int DurationMinutes { get; set; }

    [Required(ErrorMessage = "Вкажіть дату релізу.")]
    public DateOnly ReleaseDate { get; set; }

    public DateOnly? ListenedOn { get; set; }

    public bool IsFavorite { get; set; }

    [Required(ErrorMessage = "Оберіть формат.")]
    public AlbumFormat Format { get; set; }
}