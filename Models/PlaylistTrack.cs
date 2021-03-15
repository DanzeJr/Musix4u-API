using System;

namespace Musix4u_API.Models
{
    public class PlaylistTrack
    {
        public long Id { get; set; }

        public long PlaylistId { get; set; }

        public virtual Playlist Playlist { get; set; }

        public long TrackId { get; set; }

        public virtual Track Track { get; set; }

    }
}