namespace GestionPacientesApi.Domain.Entities
{
    public class User
    {
        // Properties for User entity 
        // Unique identifier for the user
        public int Id { get; set; }
        // Username for the user, initialized as an empty string
        public string Username { get; set; } = string.Empty;
        // Password hash for the user, initialized as an empty string
        public string PasswordHash { get; set; } = string.Empty;
        // Role of the user, initialized as an empty string
        public string Role { get; set; } = string.Empty;
    }
}
