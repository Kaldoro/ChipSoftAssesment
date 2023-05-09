using Api.Models.Requests;
using BusinessLogic.Services;
using Serilog;
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
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
    }

    /// <summary>
    /// Grants permission from one institution to another to access a patient's file, along with a referral letter.
    /// </summary>
    /// <param name="patientId">the Id of the patient for which the data is requested</param>
    /// <param name="createReferralRequest"></param>
    /// <returns>201 status code, and returns the patient file</returns>
    [HttpPost("patient/{patientId}/referral")]
    public IActionResult PostReferPatient(string patientId, [FromForm] CreateReferralRequest createReferralRequest)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        Log.Information("{@correlationId}: Received patient referral request (POST) request_body: {@createReferralRequest}",correlationId, createReferralRequest);
        var referral = new Referral(createReferralRequest.ReferralLetter, createReferralRequest.FromInstitution,
            createReferralRequest.ToInstitution);
        var patient = _patientService.GetById(patientId);

        var contentType = referral.ReferralLetter.ContentType;
        if(contentType != "application/pdf")
        {
            Log.Warning("{@correlationId}: Failed, the given file by the request was incorrect", correlationId);
            return BadRequest("File type:" + contentType + " is not allowed please submit a pdf file");
        }
        
        if (patient is null)
        {
            Log.Warning("{@correlationId}: Failed, patient with id:{@patientId} could not be found", correlationId, patientId);
            return NotFound("Patient with id:" + patientId + " can not be found");
        }

        if (!patient.instutions.Any((instution ) => instution.Equals(referral.FromInstitution, StringComparison.OrdinalIgnoreCase)))
        {
            Log.Warning("{@correlationId}: Failed, Institution not authorized", correlationId);
            return Forbid("Institution not authorized");
        }
        
        var referralId = _patientService.Reffer(patient, referral);
        Log.Information("{@correlationId}: Successfully processed patient referral request (POST), response: {@referral}",correlationId, referral);
        return Created("patient/" + patient.patientId + "/referral/" + referralId, referral);
    }
    
    /// <summary>
    /// Get the specified referral for the specified patient
    /// </summary>
    /// <param name="patientId">the Id of the patient for which the data is requested</param>
    /// <param name="referralId">the Id of the requested referral</param>
    /// <returns>The requested referral</returns>
    [HttpGet("/patient/{patientId}/referral/{referralId}")]
    public IActionResult GetPatientRefferal(string patientId, string referralId)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        Log.Information("{@correlationId}: Received request to retrieve referral with id:{@referralId} from patientId: {@patientId} (GET)", correlationId,referralId, patientId );
        var patient = _patientService.GetById(patientId);
        
        if (patient is null)
        {
            Log.Warning("{@correlationId}: Failed, Patient with id:{@patientId} not found", correlationId, patientId);
            return NotFound("Patient with id:" + patientId + " not found");
        }

        if (!patient.referrals.ContainsKey(referralId))
        {
            Log.Warning("{@correlationId}: Failed, Referral with id:{referralId} not found", correlationId,referralId);
            return NotFound("Referral with id:" + referralId + " not found");
        }
        var referral = patient.referrals[referralId];
        Log.Information("{@correlationId}: Successfully processed request to retrieve referral with id:{@referralId} from patientId: {@patientId} (GET) result:{@referral}", correlationId,referralId, patientId, referral);
        return Ok(referral);
    }
    
    /// <summary>
    /// Gets the medicine usage for a patient 
    /// </summary>
    /// <param name="patientId">the Id of the patient for which the data is requested</param>
    /// <returns>A list of medicines the patient uses, alongside the dosage</returns>
    [HttpGet("patient/{patientId}/medicine")]
    public IActionResult GetPatientMedicine(string patientId)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        Log.Information("{@correlationId}: Received request to retrieve medicine usage from patientId: {@patientId} (GET)", correlationId, patientId );
        var patient = _patientService.GetById(patientId);
        
        if (patient is null)
        {
            Log.Warning("{@correlationId}: Failed, Patient with id:{@patientId} not found", correlationId, patientId);
            return NotFound("Patient with id:" + patientId + " not found");
        }

        var medicines = patient.medicines;
        Log.Information("{@correlationId}: Successfully processed request to retrieve medicine usage from patientId: {@patientId} (GET) result:{@medicines}", correlationId, patientId, medicines);
        return Ok(medicines);
    }
    
    /// <summary>
    /// Add allergy to specified patient file
    /// </summary>
    /// <param name="patientId">the Id of the patient for which the data is requested</param>
    /// <param name="createAllergyRequest"></param>
    /// <returns>All the allergies for the patient</returns>
    [HttpPost("patient/{patientId}/allergies")]
    public IActionResult PostPatientAllergy(string patientId, [FromBody] CreateAllergyRequest createAllergyRequest)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        Log.Information("{@correlationId}: Received request to add allergy to patientId: {@patientId} (POST) request_body:{@createAllergyRequest}", correlationId, patientId, createAllergyRequest);
        var allergy = new Allergy(createAllergyRequest.name, createAllergyRequest.severity);
        var patient = _patientService.GetById(patientId);
        
        if (patient is null)
        {
            Log.Warning("{@correlationId}: Failed, Patient with id:{@patientId} not found", correlationId, patientId);
            return NotFound("Patient with id:" + patientId + " not found");
        }

        patient.allergies.Add(allergy);
        _patientService.Save(patient);

        var allergies = patient.allergies;
        Log.Information("{@correlationId}: Successfully processed request to add allergy to patientId: {@patientId} (POST) response:{@allergies}", correlationId, patientId, allergies);
        //TODO in the future you could at endpoint to retrieve one allergy, at that point the URI should be added here.
        return Created("",allergies);
    }
}