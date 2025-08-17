using AutoMapper;
using GestionPacientesApi.App.DTOs;
using GestionPacientesApi.Domain.Entities;

namespace GestionPacientesApi.App
{
    // AutoMapper profile for defining mappings between domain entities and DTOs
    public class MappingProfile : Profile
    {
        // Constructor for initializing mapping configurations
        public MappingProfile()
        {
            // Mapping configuration between Patient entity and PatientDto
            CreateMap<Patient, PatientDto>()
                // Explicitly maps IdNumber from Patient entity to PatientDto
                .ForMember(dest => dest.IdNumber, opt => opt.MapFrom(src => src.IdNumber))
                // Enables reverse mapping from PatientDto back to Patient entity
                .ReverseMap()
                // Explicitly maps IdNumber from PatientDto back to Patient entity
                .ForMember(dest => dest.IdNumber, opt => opt.MapFrom(src => src.IdNumber));

            // Mapping configuration between Doctor entity and DoctorDto, with reverse mapping
            CreateMap<Doctor, DoctorDto>().ReverseMap();

            // Mapping configuration between MedicalHistory entity and MedicalHistoryDto, with reverse mapping
            CreateMap<MedicalHistory, MedicalHistoryDto>().ReverseMap();
        }
    }
}
