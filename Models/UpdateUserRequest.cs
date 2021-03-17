using Microsoft.AspNetCore.Mvc;

namespace Musix4u_API.Models
{
    public class UpdateUserRequest
    {
        public string Name { get; set; }

        public bool? IsAdmin { get; set; }
    }
}