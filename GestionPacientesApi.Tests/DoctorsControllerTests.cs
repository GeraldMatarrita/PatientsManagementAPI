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
    public class DoctorsControllerTests
    {
        // Attributes for the test class, including the controller instance, unit of work, and mapper.
        private readonly DoctorsController _controller;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DoctorsControllerTests()
        {
            // Setting up an in-memory database for testing purposes
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            // Configuring AutoMapper for mapping between DTOs and domain entities
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            // Initializing the unit of work with the in-memory context
            _unitOfWork = new UnitOfWork(context);
            _controller = new DoctorsController(_unitOfWork, _mapper);
        }

        // Test method to verify that the GetAll method returns a list of doctors.
        [Fact]
        public async Task Create_DuplicateLicenseNumber_ReturnsBadRequest()
        {
            // Arrange
            var doctorDto = new DoctorDto
            {
                Name = "Dr. Smith",
                LicenseNumber = "LIC123",
                Specialty = "Cardiology",
                Email = "smith@example.com"
            };
            var doctor = _mapper.Map<Doctor>(doctorDto);
            await _unitOfWork.Doctors.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            // Act
            var result = await _controller.Create(doctorDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("LicenseNumber already exists.", badRequestResult.Value);
        }

        // Test method to verify the GetById method returns a doctor by ID.
        [Fact]
        public async Task GetById_ExistingId_ReturnsDoctor()
        {
            // Arrange
            var doctor = new Doctor
            {
                Id = 5,
                Name = "Dr. Jones",
                LicenseNumber = "LIC456",
                Specialty = "Neurology",
                Email = "jones@example.com"
            };
            await _unitOfWork.Doctors.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            // Act
            var result = await _controller.GetById(5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var doctorDto = Assert.IsType<DoctorDto>(okResult.Value);
            Assert.Equal("Dr. Jones", doctorDto.Name);
        }

    }
}