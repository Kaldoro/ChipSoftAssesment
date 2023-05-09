namespace Repository.Entities;

public record Referral(string FromInstitution, string ToInstitution, Stream File, string Filetype, string Type);