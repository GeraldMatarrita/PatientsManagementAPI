using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Ganss.Xss;

namespace GestionPacientesApi.App.DTOs
{
    // Data Transfer Object for representing patient information
    public class PatientDto
    {
        // Static instance of HtmlSanitizer for sanitizing input to prevent XSS attacks
        private static readonly HtmlSanitizer _sanitizer = new();

        // Unique identifier for the patient record
        public int Id { get; set; }

        // Patient's full name, required with a maximum length of 100 characters
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        // Patient's identification number, required with a maximum length of 50 characters
        [Required(ErrorMessage = "Identification is required.")]
        [StringLength(50, ErrorMessage = "Identification cannot exceed 50 characters.")]
        public string IdNumber { get; set; } = string.Empty;

        // Patient's email address, required and must be in a valid email format
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        // Patient's birth date, required and must be in a valid date format
        [Required(ErrorMessage = "BirthDate is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime BirthDate { get; set; }

        // Sanitizes the Name, IdNumber, and Email fields to prevent XSS attacks and normalize input
        public void Sanitize()
        {
            // Sanitize Name by allowing only letters and spaces, applying XSS protection, and trimming whitespace
            Name = SanitizeInput(SanitizeLettersOnly(Name)).Trim();
            // Sanitize IdNumber by allowing only alphanumeric characters, applying XSS protection, and trimming whitespace
            IdNumber = SanitizeInput(SanitizeIdNumber(IdNumber)).Trim();
            // Sanitize Email by applying XSS protection, trimming whitespace, and converting to lowercase
            Email = SanitizeInput(Email).Trim().ToLowerInvariant();
        }

        // Helper method to apply XSS sanitization to input strings
        private static string SanitizeInput(string input)
        {
            // Return unchanged input if null or whitespace, otherwise sanitize using HtmlSanitizer
            return string.IsNullOrWhiteSpace(input)
                ? input
                : _sanitizer.Sanitize(input);
        }

        // Helper method to remove non-alphanumeric characters from the identification number
        private static string SanitizeIdNumber(string input)
        {
            // Remove any characters that are not letters or numbers
            return Regex.Replace(input, @"[^a-zA-Z0-9]", "");
        }

        // Helper method to remove characters from the name, allowing only letters and accented letters
        private static string SanitizeLettersOnly(string input)
        {
            // Remove any characters that are not letters, accented letters, or spaces
            return Regex.Replace(input, @"[^a-zA-ZÀ-ÿ\s]", "");
        }
    }
}