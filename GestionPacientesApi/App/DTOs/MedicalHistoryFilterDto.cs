namespace GestionPacientesApi.App.DTOs
{
    // Data Transfer Object for filtering and paginating medical history records
    public class MedicalHistoryFilterDto
    {
        // Optional filter for the associated patient's ID
        public int? PatientId { get; set; }

        // Optional filter for the associated doctor's ID
        public int? DoctorId { get; set; }

        // Optional filter for the start date of the medical history records
        public DateTime? StartDate { get; set; }

        // Optional filter for the end date of the medical history records
        public DateTime? EndDate { get; set; }

        // Optional filter for the diagnosis description
        public string? Diagnosis { get; set; }

        // Page number for pagination, defaults to 1
        public int PageNumber { get; set; } = 1;

        // Number of records per page, defaults to 5
        public int PageSize { get; set; } = 5;

        // Field to sort the results by, defaults to "Date"
        public string? SortBy { get; set; } = "Date";

        // Indicates whether to sort in descending order, defaults to false (ascending)
        public bool SortDescending { get; set; } = false;
    }
}