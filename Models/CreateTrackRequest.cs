using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Musix4u_API.Models
{
    public class CreateTrackRequest
    {
        public IFormFile Song { get; set; }

        public IFormFile Cover { get; set; }

        public string Title { get; set; }

        public string Performers { get; set; }

        public string Album { get; set; }

        public uint? Year { get; set; }

        public long? PlaylistId { get; set; }

        public bool IsPublic { get; set; } = false;

        public bool IsFavorite { get; set; } = false;
    }
}