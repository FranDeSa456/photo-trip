namespace PhotoTrip.DAL.Entities
{
    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Relazione 1:N con Place (un'entità farà il FK)
        public virtual ICollection<Place> Places { get; set; } = new List<Place>();
    }
}
