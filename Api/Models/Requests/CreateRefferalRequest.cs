using System.ComponentModel.DataAnnotations;

namespace Api.Models.Requests;

public class CreateReferralRequest
{
    [Required]
    public IFormFile ReferralLetter { get; set; }
    [Required]
    public string FromInstitution { get; set; }
    [Required]
    public string ToInstitution { get; set; }
}