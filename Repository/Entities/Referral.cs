using Microsoft.AspNetCore.Http;

namespace Repository.Entities;

public record Referral(IFormFile ReferralLetter, string FromInstitution, string ToInstitution);