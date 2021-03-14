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

        public List<string> Performers { get; set; }

        public string Album { get; set; }

        public uint? Year { get; set; }

        public bool IsPublic { get; set; } = false;
    }
}