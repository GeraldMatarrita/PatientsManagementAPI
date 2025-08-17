using AutoMapper;
using GestionPacientesApi.App.DTOs;
using GestionPacientesApi.Domain.Entities;
using GestionPacientesApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionPacientesApi.Controllers
{
    // API controller for managing medical history-related operations, requires authentication
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalHistoriesController : ControllerBase
    {
        // Unit of work for accessing repository methods
        private readonly IUnitOfWork _unitOfWork;
        // AutoMapper for mapping between entities and DTOs
        private readonly IMapper _mapper;

        // Constructor for dependency injection of unit of work and AutoMapper
        public MedicalHistoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/MedicalHistories
        // Retrieves a paginated and filtered list of medical histories
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] MedicalHistoryFilterDto filter)
        {
            // Build query to select MedicalHistoryDto objects from the MedicalHistories repository
            var query = _unitOfWork.MedicalHistories.Query()
                .Select(h => new MedicalHistoryDto
                {
                    Id = h.Id,
                    PatientId = h.PatientId,
                    DoctorId = h.DoctorId,
                    Date = h.Date,
                    Diagnosis = h.Diagnosis,
                    Treatment = h.Treatment
                });

            // Apply filtering based on provided filter parameters
            if (filter.PatientId.HasValue)
                // Filter by patient ID
                query = query.Where(h => h.PatientId == filter.PatientId.Value);
            if (filter.DoctorId.HasValue)
                // Filter by doctor ID
                query = query.Where(h => h.DoctorId == filter.DoctorId.Value);
            if (filter.StartDate.HasValue)
                // Filter by start date (inclusive)
                query = query.Where(h => h.Date >= filter.StartDate.Value);
            if (filter.EndDate.HasValue)
                // Filter by end date (inclusive)
                query = query.Where(h => h.Date <= filter.EndDate.Value);
            if (!string.IsNullOrEmpty(filter.Diagnosis))
                // Filter by diagnosis, case-insensitive
                query = query.Where(h => h.Diagnosis.Contains(filter.Diagnosis, StringComparison.OrdinalIgnoreCase));

            // Apply sorting based on SortBy parameter
            query = filter.SortBy?.ToLower() switch
            {
                "diagnosis" => filter.SortDescending
                    ? query.OrderByDescending(h => h.Diagnosis) // Sort by diagnosis in descending order
                    : query.OrderBy(h => h.Diagnosis),         // Sort by diagnosis in ascending order
                _ => filter.SortDescending
                    ? query.OrderByDescending(h => h.Date)    // Default: sort by date in descending order
                    : query.OrderBy(h => h.Date)              // Default: sort by date in ascending order
            };

            // Apply pagination
            var totalRecords = await query.CountAsync(); // Calculate total number of records
            var totalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize); // Calculate total pages
            query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize); // Apply skip and take for pagination

            // Execute query and retrieve results as a list
            var historiesDto = await query.ToListAsync();

            // Return paginated response with metadata
            return Ok(new
            {
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Data = historiesDto
            });
        }

        // GET: api/MedicalHistories/5
        // Retrieves a specific medical history by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Retrieve medical history by ID from the repository
            var history = await _unitOfWork.MedicalHistories.GetByIdAsync(id);
            // Return 404 if medical history is not found
            if (history == null) throw new KeyNotFoundException("Medical history not found.");
            // Map medical history entity to DTO
            var historyDto = _mapper.Map<MedicalHistoryDto>(history);
            // Return the medical history DTO
            return Ok(historyDto);
        }

        // POST: api/MedicalHistories
        // Creates a new medical history record
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MedicalHistoryDto historyDto)
        {
            // Validate model state
            if (!ModelState.IsValid) throw new ArgumentException("Invalid input data.");

            // Validate that PatientId exists
            var patient = await _unitOfWork.Patients.GetByIdAsync(historyDto.PatientId);
            if (patient == null) throw new ArgumentException("Invalid PatientId.");
            // Validate that DoctorId exists
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(historyDto.DoctorId);
            if (doctor == null) throw new ArgumentException("Invalid DoctorId.");

            // Map DTO to MedicalHistory entity
            var history = _mapper.Map<MedicalHistory>(historyDto);
            // Add medical history to the repository
            await _unitOfWork.MedicalHistories.AddAsync(history);
            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();

            // Map created medical history back to DTO for response
            var createdDto = _mapper.Map<MedicalHistoryDto>(history);
            // Return 201 Created with the new medical history's details
            return CreatedAtAction(nameof(GetById), new { id = history.Id }, createdDto);
        }

        // PUT: api/MedicalHistories/5
        // Updates an existing medical history record
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MedicalHistoryDto historyDto)
        {
            // Validate model state
            if (!ModelState.IsValid) throw new ArgumentException("Invalid input data.");
            // Ensure ID in URL matches DTO ID
            if (id != historyDto.Id) throw new ArgumentException("ID mismatch.");

            // Retrieve existing medical history by ID
            var history = await _unitOfWork.MedicalHistories.GetByIdAsync(id);
            // Return 404 if medical history is not found
            if (history == null) throw new KeyNotFoundException("Medical history not found.");

            // Validate that PatientId exists
            var patient = await _unitOfWork.Patients.GetByIdAsync(historyDto.PatientId);
            if (patient == null) throw new ArgumentException("Invalid PatientId.");
            // Validate that DoctorId exists
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(historyDto.DoctorId);
            if (doctor == null) throw new ArgumentException("Invalid DoctorId.");

            // Update medical history entity with DTO data
            _mapper.Map(historyDto, history);
            // Mark medical history as updated in the repository
            _unitOfWork.MedicalHistories.Update(history);
            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();

            // Return 204 No Content to indicate successful update
            return NoContent();
        }

        // DELETE: api/MedicalHistories/5
        // Deletes a specific medical history by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieve medical history by ID
            var history = await _unitOfWork.MedicalHistories.GetByIdAsync(id);
            // Return 404 if medical history is not found
            if (history == null) throw new KeyNotFoundException("Medical history not found.");

            // Mark medical history for deletion in the repository
            _unitOfWork.MedicalHistories.Delete(history);
            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();

            // Return 204 No Content to indicate successful deletion
            return NoContent();
        }
    }
}