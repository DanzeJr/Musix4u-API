using Microsoft.AspNetCore.Mvc;

namespace Musix4u_API.Models
{
    public class UpdatePlaylistRequest
    {
        [FromRoute(Name = "id")]
        public long Id { get; set; }

        public string Name { get; set; }

        public bool? IsPublic { get; set; }
    }
}