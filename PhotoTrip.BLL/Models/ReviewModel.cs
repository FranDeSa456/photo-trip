namespace PhotoTrip.BLL.Models
{
    public class ReviewModel
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime Date { get; set; }
    }
}