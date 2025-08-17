using AutoMapper;
using GestionPacientesApi.App.DTOs;
using GestionPacientesApi.Domain.Entities;
using GestionPacientesApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionPacientesApi.Controllers
{
    // API controller for managing patient-related operations, requires authentication
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        // Unit of work for accessing repository methods
        private readonly IUnitOfWork _unitOfWork;
        // AutoMapper for mapping between entities and DTOs
        private readonly IMapper _mapper;

        // Constructor for dependency injection of unit of work and AutoMapper
        public PatientsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Patients
        // Retrieves a paginated and filtered list of patients
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PatientFilterDto filter)
        {
            // Build query to select PatientDto objects from the Patients repository
            var query = _unitOfWork.Patients.Query()
                .Select(p => new PatientDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    IdNumber = p.IdNumber,
                    Email = p.Email,
                    BirthDate = p.BirthDate
                });

            // Apply filtering based on provided filter parameters
            if (!string.IsNullOrEmpty(filter.Name))
                // Filter by name, case-insensitive
                query = query.Where(p => p.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(filter.IdNumber))
                // Filter by identification number
                query = query.Where(p => p.IdNumber.Contains(filter.IdNumber));

            // Apply sorting based on SortBy parameter
            query = filter.SortBy?.ToLower() switch
            {
                "birthdate" => filter.SortDescending
                    ? query.OrderByDescending(p => p.BirthDate) // Sort by birth date in descending order
                    : query.OrderBy(p => p.BirthDate),         // Sort by birth date in ascending order
                _ => filter.SortDescending
                    ? query.OrderByDescending(p => p.Name)    // Default: sort by name in descending order
                    : query.OrderBy(p => p.Name)              // Default: sort by name in ascending order
            };

            // Apply pagination
            var totalRecords = await query.CountAsync(); // Calculate total number of records
            var totalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize); // Calculate total pages
            query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize); // Apply skip and take for pagination

            // Execute query and retrieve results as a list
            var patientsDto = await query.ToListAsync();

            // Return paginated response with metadata
            return Ok(new
            {
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Data = patientsDto
            });
        }

        // GET: api/Patients/5
        // Retrieves a specific patient by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Retrieve patient by ID from the repository
            var patient = await _unitOfWork.Patients.GetByIdAsync(id) ?? throw new KeyNotFoundException("Patient not found.");
            // Map patient entity to DTO
            var patientDto = _mapper.Map<PatientDto>(patient);
            // Return the patient DTO
            return Ok(patientDto);
        }

        // POST: api/Patients
        // Creates a new patient record
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PatientDto patientDto)
        {
            // Validate model state
            if (!ModelState.IsValid) throw new ArgumentException("Invalid input data.");
            // Check for unique IdNumber
            var existingPatient = await _unitOfWork.Patients.FindAsync(p => p.IdNumber == patientDto.IdNumber);
            if (existingPatient.Any()) throw new ArgumentException("IdNumber already exists.");

            // Sanitize input data to prevent XSS attacks
            patientDto.Sanitize();
            // Map DTO to Patient entity
            var patient = _mapper.Map<Patient>(patientDto);
            // Add patient to the repository
            await _unitOfWork.Patients.AddAsync(patient);
            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();

            // Map created patient back to DTO for response
            var createdDto = _mapper.Map<PatientDto>(patient);
            // Return 201 Created with the new patient's details
            return CreatedAtAction(nameof(GetById), new { id = patient.Id }, createdDto);
        }

        // PUT: api/Patients/5
        // Updates an existing patient record
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PatientDto patientDto)
        {
            // Validate model state
            if (!ModelState.IsValid) throw new ArgumentException("Invalid input data.");
            // Ensure ID in URL matches DTO ID
            if (id != patientDto.Id) throw new ArgumentException("ID mismatch.");

            // Retrieve existing patient by ID
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);
            // Return 404 if patient is not found
            if (patient == null) throw new KeyNotFoundException("Patient not found.");

            // Sanitize input data to prevent XSS attacks
            patientDto.Sanitize();
            // Check for unique IdNumber, excluding the current patient
            var existingPatient = await _unitOfWork.Patients.FindAsync(p => p.IdNumber == patientDto.IdNumber && p.Id != id);
            if (existingPatient.Any()) throw new ArgumentException("IdNumber already exists.");

            // Update patient entity with DTO data
            _mapper.Map(patientDto, patient);
            // Mark patient as updated in the repository
            _unitOfWork.Patients.Update(patient);
            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();

            // Return 204 No Content to indicate successful update
            return NoContent();
        }

        // DELETE: api/Patients/5
        // Deletes a specific patient by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieve patient by ID
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);
            // Return 404 if patient is not found
            if (patient == null) throw new KeyNotFoundException("Patient not found.");

            // Mark patient for deletion in the repository
            _unitOfWork.Patients.Delete(patient);
            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();

            // Return 204 No Content to indicate successful deletion
            return NoContent();
        }
    }
}