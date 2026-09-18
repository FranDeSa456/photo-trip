namespace PhotoTrip.DAL.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public Place Place { get; set; } = null!;
        public int PlaceId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int Score { get; set; }
        public DateTime Date { get; set; }
    }
}
