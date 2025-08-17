using AutoMapper;
using GestionPacientesApi.App;
using GestionPacientesApi.App.DTOs;
using GestionPacientesApi.Controllers;
using GestionPacientesApi.Domain.Entities;
using GestionPacientesApi.Infrastructure.Data;
using GestionPacientesApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionPacientesApi.Tests
{
    public class PatientsControllerTests
    {
        // Unit tests for PatientsController using in-memory database and AutoMapper
        private readonly PatientsController _controller;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        // Constructor to set up the in-memory database, AutoMapper, and controller instance
        public PatientsControllerTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            // Setup AutoMapper
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            // Setup UnitOfWork
            _unitOfWork = new UnitOfWork(context);
            _controller = new PatientsController(_unitOfWork, _mapper);
        }

        // Test method to verify that the create method returns BadRequest for duplicate IdNumber
        [Fact]
        public async Task Create_DuplicateIdNumber_ReturnsBadRequest()
        {
            // Arrange
            var patientDto = new PatientDto
            {
                Name = "John Doe",
                IdNumber = "123456",
                Email = "john@example.com",
                BirthDate = new DateTime(1990, 1, 1)
            };
            var patient = _mapper.Map<Patient>(patientDto);
            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.SaveChangesAsync();

            // Act
            var result = await _controller.Create(patientDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("IdNumber already exists.", badRequestResult.Value);
        }

        // Test method to verify that the getbyid method returns a patient for an existing ID
        [Fact]
        public async Task GetById_ExistingId_ReturnsPatient()
        {
            // Arrange
            var patient = new Patient
            {
                Id = 1,
                Name = "Jane Doe",
                IdNumber = "789012",
                Email = "jane@example.com",
                BirthDate = new DateTime(1985, 5, 5)
            };
            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.SaveChangesAsync();

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var patientDto = Assert.IsType<PatientDto>(okResult.Value);
            Assert.Equal("Jane Doe", patientDto.Name);
        }
    }
}
