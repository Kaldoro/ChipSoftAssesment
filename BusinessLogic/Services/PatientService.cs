using Repository.Entities;
using Repository.Repositiories;

namespace BusinessLogic.Services;

public interface IPatientService
{
    Patient? GetById(string patientId);
    Patient Save(Patient patient);
    Patient Reffer(Patient patient, Referral referral);
}

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public Patient? GetById(string patientId)
    {
        return _patientRepository.GetById(patientId);
    }

    public Patient Save(Patient patient)
    {
        return _patientRepository.Save(patient);
    }

    public Patient Reffer(Patient patient, Referral referral)
    {
        patient.referrals.Add(referral);
        if (!patient.instutions.Contains(referral.ToInstitution))
        {
            patient.instutions.Add(referral.ToInstitution);
        }
        
        return Save(patient);
    }
}