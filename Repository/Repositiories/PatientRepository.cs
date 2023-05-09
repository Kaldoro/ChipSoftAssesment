using Mapster;
using Repository.Entities;

namespace Repository.Repositiories;

public interface IPatientRepository
{
    Patient? GetById(string patientId);
    Patient Save(Patient patient);

    string AddReferral(Patient patient, Referral referral);
}
public class PatientRepository : IPatientRepository
{
    //In Memory Storage
    private readonly List<Stream> _referrals = new List<Stream>();
    private readonly Dictionary<string, Patient> _patients = new Dictionary<string, Patient>()
    {
        //Fake data so there is a patient file to Get();
        {"9b2d6c0a409946ffb03b8aadc883d5bf", new Patient()
        {
            patientId = "9b2d6c0a409946ffb03b8aadc883d5bf",
            firstname = "John",
            lastname = "Doe",
            medicines = new List<Medicine>() { new Medicine(0.5f, "Ibuprofen")},
            allergies = new List<Allergy>() { new Allergy("gluten", "severe")},
            referrals = new Dictionary<string, Referral>(),
            instutions =  new List<string>(){"Ziekenhuis A"}

        }}
    };

    public Patient? GetById(string patientId)
    {
        _patients.TryGetValue(patientId, out var patient);
        return patient;
    }
    
    public Patient Save(Patient patient)
    {
        if (_patients.TryGetValue(patient.patientId, out var _))
        {
            //Replace existing patient
            _patients[patient.patientId] = patient;
        }
        else
        {
            _patients.Add(patient.patientId, patient);
        }
        
        return patient;
    }

    public string AddReferral(Patient patient,Referral referral)
    {
        var referralId = Guid.NewGuid().ToString("N");
        patient.referrals.Add(referralId,referral);
        return referralId;
    }
}