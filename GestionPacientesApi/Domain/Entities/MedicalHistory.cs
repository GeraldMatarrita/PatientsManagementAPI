namespace GestionPacientesApi.Domain.Entities
{
    // Entity representing a medical history record in the system
    public class MedicalHistory
    {
        // Unique identifier for the medical history record
        public int Id { get; set; }

        // Foreign key referencing the associated patient's ID
        public int PatientId { get; set; }

        // Foreign key referencing the associated doctor's ID
        public int DoctorId { get; set; }

        // Date of the medical history record
        public DateTime Date { get; set; }

        // Diagnosis description, initialized as an empty string
        public string Diagnosis { get; set; } = string.Empty;

        // Treatment description, initialized as an empty string
        public string Treatment { get; set; } = string.Empty;

        // Navigation property to the associated Patient entity, non-nullable
        public Patient Patient { get; set; } = null!;

        // Navigation property to the associated Doctor entity, non-nullable
        public Doctor Doctor { get; set; } = null!;
    }
}