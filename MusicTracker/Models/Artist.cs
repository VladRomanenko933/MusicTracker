using System.ComponentModel.DataAnnotations;

namespace MusicTracker.Models;

public class Artist
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Зазначте ім'я артиста")]
    public string Name { get; set; } = "";
}