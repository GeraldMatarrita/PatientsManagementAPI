using AutoMapper;
using GestionPacientesApi.App.DTOs;
using GestionPacientesApi.App;
using GestionPacientesApi.Controllers;
using GestionPacientesApi.Domain.Entities;
using GestionPacientesApi.Infrastructure.Data;
using GestionPacientesApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GestionPacientesApi.Tests
{
    public class MedicalHistoriesControllerTests
    {
        // Attributes for the controller, unit of work, and mapper
        private readonly MedicalHistoriesController _controller;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        // Constructor to set up the in-memory database, AutoMapper, and controller instance
        public MedicalHistoriesControllerTests()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            // Configure AutoMapper for mapping between DTOs and domain entities
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _unitOfWork = new UnitOfWork(context);
            _controller = new MedicalHistoriesController(_unitOfWork, _mapper);
        }

        // Test method to verify that the GetAll method returns a list of medical histories
        [Fact]
        public async Task Create_InvalidPatientId_ReturnsBadRequest()
        {
            // Arrange
            var historyDto = new MedicalHistoryDto
            {
                PatientId = 999, // Non-existent
                DoctorId = 1,
                Date = DateTime.Now,
                Diagnosis = "Flu",
                Treatment = "Rest"
            };

            // Act
            var result = await _controller.Create(historyDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid PatientId.", badRequestResult.Value);
        }

        // Test method to verify that the Create method returns BadRequest for invalid DoctorId
        [Fact]
        public async Task Create_ValidIds_CreatesHistory()
        {
            // Arrange
            var patient = new Patient { Id = 2, Name = "John Doe", IdNumber = "123456", Email = "john@example.com", BirthDate = new DateTime(1990, 1, 1) };
            var doctor = new Doctor { Id = 4, Name = "Dr. Smith", LicenseNumber = "LIC123", Specialty = "Cardiology", Email = "smith@example.com" };
            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.Doctors.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            var historyDto = new MedicalHistoryDto
            {
                PatientId = 2,
                DoctorId = 4,
                Date = DateTime.Now,
                Diagnosis = "Flu",
                Treatment = "Rest"
            };

            // Act
            var result = await _controller.Create(historyDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var historyDtoResult = Assert.IsType<MedicalHistoryDto>(createdResult.Value);
            Assert.Equal("Flu", historyDtoResult.Diagnosis);
        }
    }
}