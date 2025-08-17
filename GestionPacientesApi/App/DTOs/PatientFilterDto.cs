namespace GestionPacientesApi.App.DTOs
{
    // Data Transfer Object for filtering and paginating patient records
    public class PatientFilterDto
    {
        // Optional filter for patient's name
        public string? Name { get; set; }

        // Optional filter for patient's identification number
        public string? IdNumber { get; set; }

        // Page number for pagination, defaults to 1
        public int PageNumber { get; set; } = 1;

        // Number of records per page, defaults to 5
        public int PageSize { get; set; } = 5;

        // Field to sort the results by, defaults to "Name"
        public string? SortBy { get; set; } = "Name";

        // Indicates whether to sort in descending order, defaults to false (ascending)
        public bool SortDescending { get; set; } = false;
    }
}