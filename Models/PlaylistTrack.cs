using System;

namespace Musix4u_API.Models
{
    public class PlaylistTrack
    {
        public long Id { get; set; }

        public long PlaylistId { get; set; }

        public long TrackId { get; set; }

    }
}