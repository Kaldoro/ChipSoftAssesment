using Repository.Entities;
using Repository.Repositiories;

namespace BusinessLogic.Services;

public interface IPatientService
{
    Patient? GetById(string patientId);
    Patient Save(Patient patient);
    string Reffer(Patient patient, Referral referral);
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

    public string Reffer(Patient patient, Referral referral)
    {
        if (!patient.instutions.Contains(referral.ToInstitution))
        {
            patient.instutions.Add(referral.ToInstitution);
        }
        Save(patient);
        return  _patientRepository.AddReferral(patient, referral);
    }
}