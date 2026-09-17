using System.ComponentModel.DataAnnotations;

namespace PhotoTrip.DAL.Entities
{
    public class Place
    {
        [Key]
        public int Id { get; set; }
    }
}