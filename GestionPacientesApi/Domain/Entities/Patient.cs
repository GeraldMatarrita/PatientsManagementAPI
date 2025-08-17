using System;
using System.Collections.Generic;

namespace GestionPacientesApi.Domain.Entities
{
    // Entity representing a patient in the system
    public class Patient
    {
        // Unique identifier for the patient
        public int Id { get; set; }

        // Patient's full name, initialized as an empty string
        public string Name { get; set; } = string.Empty;

        // Patient's identification number, initialized as an empty string
        public string IdNumber { get; set; } = string.Empty;

        // Patient's email address, initialized as an empty string
        public string Email { get; set; } = string.Empty;

        // Patient's birth date
        public DateTime BirthDate { get; set; }

        // Collection of medical history records associated with the patient, initialized as an empty list
        public List<MedicalHistory> MedicalHistories { get; set; } = new();
    }
}