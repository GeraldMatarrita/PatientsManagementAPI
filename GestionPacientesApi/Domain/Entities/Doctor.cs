namespace GestionPacientesApi.Domain.Entities
{
    // Entity representing a doctor in the system
    public class Doctor
    {
        // Unique identifier for the doctor
        public int Id { get; set; }

        // Doctor's full name, initialized as an empty string
        public string Name { get; set; } = string.Empty;

        // Doctor's license number, initialized as an empty string
        public string LicenseNumber { get; set; } = string.Empty;

        // Doctor's medical specialty, initialized as an empty string
        public string Specialty { get; set; } = string.Empty;

        // Doctor's email address, initialized as an empty string
        public string Email { get; set; } = string.Empty;

        // Collection of medical history records associated with the doctor, initialized as an empty list
        public List<MedicalHistory> MedicalHistories { get; set; } = new();
    }
}