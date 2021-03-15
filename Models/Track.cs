using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Musix4u_API.Models
{
    public class Track
    {
        public long Id { get; set; }

        public string Url { get; set; }

        public string CoverUrl { get; set; }

        public string Title { get; set; }

        public string Performers { get; set; }

        public string Album { get; set; }

        public uint? Year { get; set; }

        public long Duration { get; set; }

        public long? UploaderId { get; set; }

        public virtual User Uploader { get; set; }

        public bool IsPublic { get; set; }

        public virtual List<PlaylistTrack> PlaylistTracks { get; set; }

        public virtual List<FavoriteTrack> FavoriteTracks { get; set; }
    }
}