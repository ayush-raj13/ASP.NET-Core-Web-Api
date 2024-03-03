using AddressBookWeb.Controllers;
using AddressBookWeb.Models.Data;
using AddressBookWeb.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AddressBookUnitTest
{
    public class MockDatabaseTests
    {

        [Test]
        public void CreateEntityWithRetry_SuccessAfter1stAttempt()
        {
            // Arrange
            var entity = new Entity
            {
                Id = "1",
                Gender = "Male",
                Deceased = false,
                Addresses = new List<Address>
                {
                    new Address
                    {
                        AddressLine = "123 Main Street",
                        City = "Cityville",
                        Country = "Countryland"
                    },
                    new Address
                    {
                        AddressLine = "456 Elm Street",
                        City = "Townsville",
                        Country = "Countryland"
                    }
                },
                Names = new List<Name>
                {
                    new Name
                    {
                        FirstName = "John",
                        MiddleName = "Robert",
                        Surname = "Doe"
                    }
                },
                Dates = new List<Date>
                {
                    new Date
                    {
                        DateType = "Birth",
                        DateOfEvent = new DateTime(1980, 5, 15)
                    },
                    new Date
                    {
                        DateType = "Graduation",
                        DateOfEvent = new DateTime(2000, 6, 30)
                    }
                }
            };

            // Act
            var result = MockDatabase.CreateEntityWithRetry(entity, 1);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public void CreateEntityWithRetry_SuccessAfter2ndAttempt()
        {
            // Arrange
            var entity = new Entity
            {
                Id = "2",
                Gender = "Male",
                Deceased = false,
                Addresses = new List<Address>
                {
                    new Address
                    {
                        AddressLine = "123 Main Street",
                        City = "Cityville",
                        Country = "Countryland"
                    },
                    new Address
                    {
                        AddressLine = "456 Elm Street",
                        City = "Townsville",
                        Country = "Countryland"
                    }
                },
                Names = new List<Name>
                {
                    new Name
                    {
                        FirstName = "John",
                        MiddleName = "Robert",
                        Surname = "Doe"
                    }
                },
                Dates = new List<Date>
                {
                    new Date
                    {
                        DateType = "Birth",
                        DateOfEvent = new DateTime(1980, 5, 15)
                    },
                    new Date
                    {
                        DateType = "Graduation",
                        DateOfEvent = new DateTime(2000, 6, 30)
                    }
                }
            };

            // Act
            var result = MockDatabase.CreateEntityWithRetry(entity, 2);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public void CreateEntityWithRetry_SuccessAfter3rdAttempt()
        {
            // Arrange
            var entity = new Entity
            {
                Id = "3",
                Gender = "Male",
                Deceased = false,
                Addresses = new List<Address>
                {
                    new Address
                    {
                        AddressLine = "123 Main Street",
                        City = "Cityville",
                        Country = "Countryland"
                    },
                    new Address
                    {
                        AddressLine = "456 Elm Street",
                        City = "Townsville",
                        Country = "Countryland"
                    }
                },
                Names = new List<Name>
                {
                    new Name
                    {
                        FirstName = "John",
                        MiddleName = "Robert",
                        Surname = "Doe"
                    }
                },
                Dates = new List<Date>
                {
                    new Date
                    {
                        DateType = "Birth",
                        DateOfEvent = new DateTime(1980, 5, 15)
                    },
                    new Date
                    {
                        DateType = "Graduation",
                        DateOfEvent = new DateTime(2000, 6, 30)
                    }
                }
            };

            // Act
            var result = MockDatabase.CreateEntityWithRetry(entity, 3);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public void CreateEntityWithRetry_RetryLimitExceeded()
        {
            // Arrange
            var entity = new Entity
            {
                Id = "4",
                Gender = "Male",
                Deceased = false,
                Addresses = new List<Address>
                {
                    new Address
                    {
                        AddressLine = "123 Main Street",
                        City = "Cityville",
                        Country = "Countryland"
                    },
                    new Address
                    {
                        AddressLine = "456 Elm Street",
                        City = "Townsville",
                        Country = "Countryland"
                    }
                },
                Names = new List<Name>
                {
                    new Name
                    {
                        FirstName = "John",
                        MiddleName = "Robert",
                        Surname = "Doe"
                    }
                },
                Dates = new List<Date>
                {
                    new Date
                    {
                        DateType = "Birth",
                        DateOfEvent = new DateTime(1980, 5, 15)
                    },
                    new Date
                    {
                        DateType = "Graduation",
                        DateOfEvent = new DateTime(2000, 6, 30)
                    }
                }
            };

            // Act
            var result = MockDatabase.CreateEntityWithRetry(entity, 4);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}