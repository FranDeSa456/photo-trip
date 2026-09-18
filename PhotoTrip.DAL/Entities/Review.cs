namespace PhotoTrip.DAL.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public Place Place { get; set; } = null!;
        public string AuthorName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime Date { get; set; }
    }
}
