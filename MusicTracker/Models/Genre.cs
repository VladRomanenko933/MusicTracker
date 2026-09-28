using System.ComponentModel.DataAnnotations;

namespace MusicTracker.Models;

public class Genre
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Зазначте назву жанру")]
    public string Name { get; set; } = "";
}