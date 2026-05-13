using System.ComponentModel.DataAnnotations;

namespace MythApi.Gods.Models;

public class GodInput {
    public int? Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [MinLength(1, ErrorMessage = "Name must not be empty.")]
    [MaxLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
    [RegularExpression(@"^[\p{L}\s'\-\.]+$", ErrorMessage = "Name may only contain letters, spaces, hyphens, apostrophes, and dots.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Description is required.")]
    [MinLength(1, ErrorMessage = "Description must not be empty.")]
    [MaxLength(500, ErrorMessage = "Description must not exceed 500 characters.")]
    public string Description { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "MythologyId must be a positive integer.")]
    public int MythologyId { get; set; }
}

