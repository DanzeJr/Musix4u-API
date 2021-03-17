namespace Musix4u_API.Models
{
    public class FilterTrackRequest
    {
        public string Title { get; set; }

        public bool? Favorite { get; set; }

        public long? PlaylistId { get; set; }
    }
}