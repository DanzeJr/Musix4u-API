using System;
using System.Collections.Generic;

namespace Musix4u_API.Models
{
    public class User
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool IsAdmin { get; set; }

        public virtual List<Playlist> Playlists { get; set; }

        public virtual List<Track> Tracks { get; set; }

        public virtual List<FavoriteTrack> FavoriteTracks { get; set; }
    }
}