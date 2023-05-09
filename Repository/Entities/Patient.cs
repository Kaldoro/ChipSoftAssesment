namespace Repository.Entities;

public class Patient
{
    public string patientId;
    public string? firstname;
    public string? lastname;
    public List<Allergy> allergies = new List<Allergy>();
    public List<Medicine> medicines = new List<Medicine>();
    public Dictionary<string, Referral> referrals = new Dictionary<string, Referral>();
    public List<string> instutions = new List<string>();
}