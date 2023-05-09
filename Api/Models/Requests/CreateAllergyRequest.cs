using System.ComponentModel.DataAnnotations;

namespace Api.Models.Requests;

public class CreateAllergyRequest
{
    [Required]
    public string name { get; set; }
    [Required]
    public string severity { get; set; }
}