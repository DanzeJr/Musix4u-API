namespace Musix4u_API.Models
{
    public class FavoriteTrack
    {
        public long Id { get; set; }

        public long TrackId { get; set; }

        public Track Track { get; set; }

        public long UserId { get; set; }

        public User User { get; set; }
    }
}