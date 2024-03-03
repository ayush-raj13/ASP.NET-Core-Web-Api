using System.ComponentModel.DataAnnotations;
using AddressBookWeb.Models.Data;

namespace AddressBookWeb.Models.Entities
{
    public class Entity : IEntity
    {
        public Entity()
        {
            Id = Guid.NewGuid().ToString(); // Initialize Id with a non-null value
            Dates = new List<Date>(); // Initialize Dates with an empty list
            Names = new List<Name>(); // Initialize Names with an empty list
            CreatedDate = DateTime.Now;
        }

        public List<Address>? Addresses { get; set; }
        public List<Date> Dates { get; set; }
        public bool Deceased { get; set; }
        public string? Gender { get; set; }
        [Key]
        public string Id { get; set; }
        public List<Name> Names { get; set; }

        public DateTime CreatedDate { get; private set; }
    }
}
