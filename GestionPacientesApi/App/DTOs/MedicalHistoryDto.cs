using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Ganss.Xss;

namespace GestionPacientesApi.App.DTOs
{
    // Data Transfer Object for representing medical history information
    public class MedicalHistoryDto
    {
        // Static instance of HtmlSanitizer for sanitizing input to prevent XSS attacks
        private static readonly HtmlSanitizer _sanitizer = new();

        // Unique identifier for the medical history record
        public int Id { get; set; }

        // Identifier of the associated patient, required and must be positive
        [Required(ErrorMessage = "PatientId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "PatientId must be a positive number.")]
        public int PatientId { get; set; }

        // Identifier of the associated doctor, required and must be positive
        [Required(ErrorMessage = "DoctorId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "DoctorId must be a positive number.")]
        public int DoctorId { get; set; }

        // Date of the medical history record, required and must be in a valid date format
        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime Date { get; set; }

        // Diagnosis description, required with a maximum length of 500 characters
        [Required(ErrorMessage = "Diagnosis is required.")]
        [StringLength(500, ErrorMessage = "Diagnosis cannot exceed 500 characters.")]
        public string Diagnosis { get; set; } = string.Empty;

        // Treatment description, optional with a maximum length of 1000 characters
        [StringLength(1000, ErrorMessage = "Treatment cannot exceed 1000 characters.")]
        public string Treatment { get; set; } = string.Empty;

        // Sanitizes the Diagnosis and Treatment fields to prevent XSS attacks
        public void Sanitize()
        {
            // Sanitize Diagnosis by removing unsafe characters and applying XSS protection
            Diagnosis = SanitizeInput(SanitizeText(Diagnosis));
            // Sanitize Treatment by removing unsafe characters and applying XSS protection
            Treatment = SanitizeInput(SanitizeText(Treatment));
        }

        // Helper method to apply XSS sanitization to input strings
        private static string SanitizeInput(string input)
        {
            // Return unchanged input if null or whitespace, otherwise sanitize using HtmlSanitizer
            return string.IsNullOrWhiteSpace(input)
                ? input
                : _sanitizer.Sanitize(input);
        }

        // Helper method to remove unsafe characters from text, allowing letters, numbers, spaces, and common medical punctuation
        private static string SanitizeText(string input)
        {
            // Remove any characters not matching letters, numbers, spaces, or common punctuation (.,;:!?-)
            return Regex.Replace(input, @"[^\w\s.,;:!?-]", "");
        }
    }
}