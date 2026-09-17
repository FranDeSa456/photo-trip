namespace PhotoTrip.DAL.Entities
{
    public class Place
    {
        public int Id {  get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RegionId { get; set; }
        public Region Region { get; set; } = null!;
        public string ImageUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
    }
}
