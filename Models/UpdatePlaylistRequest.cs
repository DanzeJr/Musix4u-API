using Microsoft.AspNetCore.Mvc;

namespace Musix4u_API.Models
{
    public class UpdatePlaylistRequest
    {
        public string Name { get; set; }

        public bool? IsPublic { get; set; }
    }
}