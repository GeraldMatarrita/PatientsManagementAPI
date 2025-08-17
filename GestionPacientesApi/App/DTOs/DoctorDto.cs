using Ganss.Xss;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GestionPacientesApi.App.DTOs
{
    // Data Transfer Object for representing doctor information
    public class DoctorDto
    {
        // Unique identifier for the doctor record
        public int Id { get; set; }

        // Static instance of HtmlSanitizer for sanitizing input to prevent XSS attacks
        private static readonly HtmlSanitizer _sanitizer = new();

        // Doctor's full name, required with a maximum length of 100 characters
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        // Doctor's license number, required with a maximum length of 50 characters
        [Required(ErrorMessage = "License Number is required.")]
        [StringLength(50, ErrorMessage = "License Number cannot exceed 50 characters.")]
        public string LicenseNumber { get; set; } = string.Empty;

        // Doctor's medical specialty, required with a maximum length of 100 characters
        [Required(ErrorMessage = "Specialty is required.")]
        [StringLength(100, ErrorMessage = "Specialty cannot exceed 100 characters.")]
        public string Specialty { get; set; } = string.Empty;

        // Doctor's email address, required and must be in a valid email format
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        // Sanitizes the Name, LicenseNumber, Specialty, and Email fields to prevent XSS attacks
        public void Sanitize()
        {
            // Sanitize Name by allowing only letters and spaces, then applying XSS protection
            Name = SanitizeInput(SanitizeLettersOnly(Name));
            // Sanitize LicenseNumber by allowing only alphanumeric characters, then applying XSS protection
            LicenseNumber = SanitizeInput(SanitizeLicenseNumbre(LicenseNumber));
            // Sanitize Specialty by applying XSS protection
            Specialty = SanitizeInput(Specialty);
            // Sanitize Email by applying XSS protection
            Email = SanitizeInput(Email);
        }

        // Helper method to apply XSS sanitization to input strings
        private static string SanitizeInput(string input)
        {
            // Return unchanged input if null or whitespace, otherwise sanitize using HtmlSanitizer
            return string.IsNullOrWhiteSpace(input)
                ? input
                : _sanitizer.Sanitize(input);
        }

        // Helper method to remove non-alphanumeric characters from the license number
        private static string SanitizeLicenseNumbre(string input)
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