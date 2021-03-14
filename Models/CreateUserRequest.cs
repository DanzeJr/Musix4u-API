namespace Musix4u_API.Models
{
    public class CreateUserRequest
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool IsAdmin { get; set; } = false;
    }
}