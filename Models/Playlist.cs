using System;
using System.Collections.Generic;

namespace Musix4u_API.Models
{
    public class Playlist
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long OwnerId { get; set; }

        public virtual User Owner { get; set; }

        public bool IsPublic { get; set; }

        public virtual List<PlaylistTrack> PlaylistTracks { get; set; }
    }
}