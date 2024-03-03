using AddressBookWeb.Models.Entities;

namespace AddressBookWeb.Models.Data
{
    public static class DataGenerator
    {
        public static Entity GenerateRandomEntity()
        {
            Random random = new Random();
            Entity entity = new Entity
            {
                Id = Guid.NewGuid().ToString(),
                Gender = random.Next(2) == 0 ? "Male" : "Female",
                Deceased = random.Next(2) == 0,
                Addresses = GenerateRandomAddresses(random.Next(1, 4)),
                Dates = GenerateRandomDates(random.Next(1, 4)),
                Names = GenerateRandomNames(random.Next(1, 4))
            };
            return entity;
        }

        private static List<Address> GenerateRandomAddresses(int count)
        {
            List<Address> addresses = new List<Address>();
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                addresses.Add(new Address
                {
                    AddressLine = "Address " + (i + 1),
                    City = "City " + (i + 1),
                    Country = "Country " + (i + 1)
                });
            }
            return addresses;
        }

        private static List<Date> GenerateRandomDates(int count)
        {
            List<Date> dates = new List<Date>();
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                dates.Add(new Date
                {
                    DateType = "DateType " + (i + 1),
                    DateOfEvent = DateTime.Now.AddDays(random.Next(-365, 365))
                });
            }
            return dates;
        }

        private static List<Name> GenerateRandomNames(int count)
        {
            List<Name> names = new List<Name>();
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                names.Add(new Name
                {
                    FirstName = "FirstName " + (i + 1),
                    MiddleName = "MiddleName " + (i + 1),
                    Surname = "Surname " + (i + 1)
                });
            }
            return names;
        }
    }
}
