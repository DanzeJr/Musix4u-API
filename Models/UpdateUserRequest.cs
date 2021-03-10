using Microsoft.AspNetCore.Mvc;

namespace Musix4u_API.Models
{
    public class UpdateUserRequest
    {
        [FromRoute(Name = "id")]
        public long Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool? IsAdmin { get; set; }
    }
}