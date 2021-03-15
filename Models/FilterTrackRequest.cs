namespace Musix4u_API.Models
{
    public class FilterTrackRequest
    {
        public bool? Favorite { get; set; }

        public long? PlaylistId { get; set; }
    }
}