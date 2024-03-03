using AddressBookWeb.Controllers;
using AddressBookWeb.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AddressBookWeb.Models.Data
{
    public static class MockDatabase
    {
        private static Dictionary<string, Entity> _entities = new Dictionary<string, Entity>();

        public static IActionResult CreateEntityWithRetry(Entity entity, ILogger<EntityController> logger, int permissibleAttempts)
        {
            int maxAttempts = 3; // Maximum number of retry attempts
            TimeSpan initialDelay = TimeSpan.FromSeconds(1); // Initial delay before the first retry
            TimeSpan maxDelay = TimeSpan.FromSeconds(10); // Maximum delay between retries
            double backoffMultiplier = 2.0; // Backoff multiplier for exponential backoff
            Random random = new Random();

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    if (permissibleAttempts == attempt)
                    {
                        if (_entities.ContainsKey(entity.Id))
                        {
                            return new BadRequestObjectResult("An entity with the same ID already exists.");
                        }
                        _entities.Add(entity.Id, entity);
                    }
                    else
                    {
                        throw new Exception("Database write operation failed.");
                    }

                    if (logger != null)
                    {
                        logger.LogInformation($"Entity created successfully on attempt {attempt}");
                    }

                    return new OkResult(); // Exit the loop if successful
                }
                catch (Exception ex)
                {
                    if (logger != null)
                    {
                        logger.LogInformation($"Error creating entity on attempt {attempt}: {ex.Message}");
                    }

                    if (attempt < maxAttempts)
                    {
                        // Calculate delay using exponential backoff strategy
                        TimeSpan delay = CalculateBackoffDelay(initialDelay, maxDelay, backoffMultiplier, attempt);

                        if (logger != null)
                        {
                            logger.LogInformation($"Retrying in {delay.TotalSeconds} seconds...");
                        }

                        Thread.Sleep(delay); // Wait before retrying
                    }
                }
            }

            if (logger != null)
            {
                logger.LogError("Maximum retry attempts reached. Failed to create entity.");
            }

            return new BadRequestObjectResult("Database is unavailable.");
        }

        public static IActionResult CreateEntityWithRetry(Entity entity, int permissibleAttempts)
        {
            return CreateEntityWithRetry(entity, null, permissibleAttempts);
        }

        public static IActionResult CreateEntity(Entity entity)
        {
            if (_entities.ContainsKey(entity.Id))
            {
                return new BadRequestObjectResult("An entity with the same ID already exists.");
            }
            _entities.Add(entity.Id, entity);
            return new OkResult();
        }

        public static Entity ReadEntity(string id)
        {
            return _entities.ContainsKey(id) ? _entities[id] : null;
        }

        public static void UpdateEntity(Entity entity)
        {
            if (_entities.ContainsKey(entity.Id))
            {
                _entities[entity.Id] = entity;
            }
        }

        public static void DeleteEntity(string id)
        {
            if (_entities.ContainsKey(id))
            {
                _entities.Remove(id);
            }
        }

        public static IEnumerable<Entity> GetAllEntities()
        {
            return _entities.Values;
        }

        private static TimeSpan CalculateBackoffDelay(TimeSpan initialDelay, TimeSpan maxDelay, double multiplier, int attempt)
        {
            // Calculate delay using exponential backoff strategy
            double delaySeconds = Math.Min(initialDelay.TotalSeconds * Math.Pow(multiplier, attempt - 1), maxDelay.TotalSeconds);
            return TimeSpan.FromSeconds(delaySeconds);
        }
    }
}
