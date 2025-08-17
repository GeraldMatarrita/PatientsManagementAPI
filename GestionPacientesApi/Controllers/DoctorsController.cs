using AutoMapper;
using GestionPacientesApi.App.DTOs;
using GestionPacientesApi.Domain.Entities;
using GestionPacientesApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionPacientesApi.Controllers
{
    // API controller for managing doctor-related operations, requires authentication
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        // Unit of work for accessing repository methods
        private readonly IUnitOfWork _unitOfWork;
        // AutoMapper for mapping between entities and DTOs
        private readonly IMapper _mapper;

        // Constructor for dependency injection of unit of work and AutoMapper
        public DoctorsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Doctors
        // Retrieves a paginated and filtered list of doctors
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DoctorFilterDto filter)
        {
            // Build query to select DoctorDto objects from the Doctors repository
            var query = _unitOfWork.Doctors.Query()
                .Select(d => new DoctorDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    LicenseNumber = d.LicenseNumber,
                    Specialty = d.Specialty,
                    Email = d.Email
                });

            // Apply filtering based on provided filter parameters
            if (!string.IsNullOrEmpty(filter.Name))
                // Filter by name, case-insensitive
                query = query.Where(d => d.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(filter.LicenseNumber))
                // Filter by license number
                query = query.Where(d => d.LicenseNumber.Contains(filter.LicenseNumber));
            if (!string.IsNullOrEmpty(filter.Specialty))
                // Filter by specialty, case-insensitive
                query = query.Where(d => d.Specialty.Contains(filter.Specialty, StringComparison.OrdinalIgnoreCase));

            // Apply sorting based on SortBy parameter
            query = filter.SortBy?.ToLower() switch
            {
                "specialty" => filter.SortDescending
                    ? query.OrderByDescending(d => d.Specialty) // Sort by specialty in descending order
                    : query.OrderBy(d => d.Specialty),         // Sort by specialty in ascending order
                _ => filter.SortDescending
                    ? query.OrderByDescending(d => d.Name)    // Default: sort by name in descending order
                    : query.OrderBy(d => d.Name)              // Default: sort by name in ascending order
            };

            // Apply pagination
            var totalRecords = await query.CountAsync(); // Calculate total number of records
            var totalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize); // Calculate total pages
            query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize); // Apply skip and take for pagination

            // Execute query and retrieve results as a list
            var doctorsDto = await query.ToListAsync();

            // Return paginated response with metadata
            return Ok(new
            {
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Data = doctorsDto
            });
        }

        // GET: api/Doctors/5
        // Retrieves a specific doctor by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Retrieve doctor by ID from the repository
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
            // Return 404 if doctor is not found
            if (doctor == null) throw new KeyNotFoundException("Doctor not found.");
            // Map doctor entity to DTO
            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            // Return the doctor DTO
            return Ok(doctorDto);
        }

        // POST: api/Doctors
        // Creates a new doctor record
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DoctorDto doctorDto)
        {
            // Validate model state
            if (!ModelState.IsValid) throw new ArgumentException("Invalid input data.");

            // Check for unique LicenseNumber
            var existingDoctor = await _unitOfWork.Doctors.FindAsync(d => d.LicenseNumber == doctorDto.LicenseNumber);
            if (existingDoctor.Any()) throw new ArgumentException("LicenseNumber already exists.");

            // Map DTO to Doctor entity
            var doctor = _mapper.Map<Doctor>(doctorDto);
            // Add doctor to the repository
            await _unitOfWork.Doctors.AddAsync(doctor);
            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();

            // Map created doctor back to DTO for response
            var createdDto = _mapper.Map<DoctorDto>(doctor);
            // Return 201 Created with the new doctor's details
            return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, createdDto);
        }

        // PUT: api/Doctors/5
        // Updates an existing doctor record
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DoctorDto doctorDto)
        {
            // Validate model state
            if (!ModelState.IsValid) throw new ArgumentException("Invalid input data.");
            // Ensure ID in URL matches DTO ID
            if (id != doctorDto.Id) throw new ArgumentException("ID mismatch.");

            // Retrieve existing doctor by ID
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
            // Return 404 if doctor is not found
            if (doctor == null) throw new KeyNotFoundException("Doctor not found.");

            // Check for unique LicenseNumber, excluding the current doctor
            var existingDoctor = await _unitOfWork.Doctors.FindAsync(d => d.LicenseNumber == doctorDto.LicenseNumber && d.Id != id);
            if (existingDoctor.Any()) throw new ArgumentException("LicenseNumber already exists.");

            // Update doctor entity with DTO data
            _mapper.Map(doctorDto, doctor);
            // Mark doctor as updated in the repository
            _unitOfWork.Doctors.Update(doctor);
            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();

            // Return 204 No Content to indicate successful update
            return NoContent();
        }

        // DELETE: api/Doctors/5
        // Deletes a specific doctor by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieve doctor by ID
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
            // Return 404 if doctor is not found
            if (doctor == null) throw new KeyNotFoundException("Doctor not found.");

            // Mark doctor for deletion in the repository
            _unitOfWork.Doctors.Delete(doctor);
            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();

            // Return 204 No Content to indicate successful deletion
            return NoContent();
        }
    }
}