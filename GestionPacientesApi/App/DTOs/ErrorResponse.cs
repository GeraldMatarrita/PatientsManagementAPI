namespace GestionPacientesApi.Application.DTOs
{
    // Data Transfer Object for representing error responses in the API
    public class ErrorResponse
    {
        // HTTP status code associated with the error
        public int StatusCode { get; set; }

        // General error message describing the issue
        public string Message { get; set; } = string.Empty;

        // Optional dictionary containing field-specific error messages, where the key is the field name and the value is an array of error messages
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
