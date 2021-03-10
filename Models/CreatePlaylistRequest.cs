using Microsoft.AspNetCore.Mvc;

namespace Musix4u_API.Models
{
    public class CreatePlaylistRequest
    {
        public string Name { get; set; }

        public bool IsPublic { get; set; } = false;
    }
}