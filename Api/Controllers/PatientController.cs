using Api.Models.Requests;
using BusinessLogic.Services;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;

namespace Api.Controllers;

[ApiController]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;

    private readonly ILogger<PatientController> _logger;

    public PatientController(IPatientService patientService, ILogger<PatientController> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    /// <summary>
    /// Grants permission from one institution to another to access a patient's file, along with a referral letter.
    /// </summary>
    /// <param name="createReferralRequest"></param>
    /// <returns>201 status code, and returns the patientfile</returns>
    [HttpPost("patient/referral")]
    public IActionResult Post([FromBody] CreateReferralRequest createReferralRequest)
    {
        var referral = createReferralRequest.Adapt<Referral>();
        var patient = _patientService.GetById(createReferralRequest.PatientId);

        if (patient is null)
        {
            return NotFound("Patient with id:" + createReferralRequest.PatientId + " can not be found");
        }

        if (!patient.instutions.Any((instution ) => instution.Equals(referral.FromInstitution, StringComparison.OrdinalIgnoreCase)))
        {
            return Forbid("Institution not authorized");
        }

        patient = _patientService.Reffer(patient, referral);

        //TODO: Enter uri
        return Created("",patient);
    }
    
    /// <summary>
    /// Gets the medicine usage for a patient 
    /// </summary>
    /// <param name="patientId">the Id of the patient for which the data is requested</param>
    /// <returns>A list of medicines the patient uses, alongside the dosage</returns>
    [HttpGet("patient/medicine")]
    public IActionResult GetPatientMedicine(string patientId)
    {
        var patient = _patientService.GetById(patientId);
        
        if (patient is null)
        {
            return NotFound("Patient with id:" + patientId + " not found");
        }
        
        return Ok(patient.medicines);
    }
}