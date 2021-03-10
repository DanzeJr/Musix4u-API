using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Musix4u_API.Models
{
    public class UpdateTrackRequest
    {
        [FromRoute(Name = "id")]
        public long Id { get; set; }

        public IFormFile File { get; set; }

        public IFormFile Cover { get; set; }

        public string Title { get; set; }

        public List<string> Performers { get; set; }

        public string Album { get; set; }

        public uint? Year { get; set; }
    }
}