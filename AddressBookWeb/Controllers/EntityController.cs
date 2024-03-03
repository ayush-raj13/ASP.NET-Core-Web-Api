using AddressBookWeb.Models.Data;
using AddressBookWeb.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AddressBookWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntityController : ControllerBase
    {

        private readonly ILogger<EntityController> _logger;

        public EntityController(ILogger<EntityController> logger)
        {
            _logger = logger;
        }

        // GET: /api/Entity
        [HttpGet]
        public IActionResult Get(
            string? search = null,
            string? gender = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            [FromQuery] string[]? countries = null,
            int pageNumber = 1,
            int pageSize = 3,
            string? sortBy = null,
            string? sortOrder = "asc")
        {
            // Get all entities
            IEnumerable<Entity> entities = MockDatabase.GetAllEntities();

            // Apply filtering
            if (!string.IsNullOrEmpty(search))
            {
                entities = entities.Where(e =>
                    (e.Addresses?.Any(a => a?.Country?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ?? false) ||
                    (e.Addresses?.Any(a => a?.AddressLine?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ?? false) ||
                    (e.Names?.Any(n => n?.FirstName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ?? false) ||
                    (e.Names?.Any(n => n?.MiddleName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ?? false) ||
                    (e.Names?.Any(n => n?.Surname?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ?? false)
                );
            }
            if (!string.IsNullOrEmpty(gender))
            {
                entities = entities.Where(e => e.Gender?.Equals(gender, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            if (startDate != null)
            {
                entities = entities.Where(e => e.Dates?.Any(d => d.DateOfEvent >= startDate) ?? false);
            }
            if (endDate != null)
            {
                entities = entities.Where(e => e.Dates?.Any(d => d.DateOfEvent <= endDate) ?? false);
            }
            if (countries != null && countries.Any())
            {
                entities = entities.Where(e => e.Addresses?.Any(a => a?.Country != null && countries.Contains(a.Country, StringComparer.OrdinalIgnoreCase)) ?? false);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "createddate":
                        entities = sortOrder?.ToLower() == "desc" ? entities.OrderByDescending(e => e.CreatedDate) : entities.OrderBy(e => e.CreatedDate);
                        break;
                        // Add more cases for other sortable fields as needed
                }
            }

            // Paginate results
            int totalItems = entities.Count();
            entities = entities.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            // Construct pagination metadata
            var metadata = new
            {
                TotalItems = totalItems,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            };

            // Return paginated and sorted results
            return Ok(new { Metadata = metadata, Results = entities });
        }

        // GET: /api/Entity/{id}
        [HttpGet("{id}")]
        public ActionResult<Entity> Get(string id)
        {
            Entity entity = MockDatabase.ReadEntity(id);
            if (entity == null)
            {
                return NotFound(); // Entity not found
            }
            return Ok(entity);
        }

        // POST: /api/Entity
        [HttpPost]
        public IActionResult Post(Entity entity)
        {
            Random random = new Random();
            var result = MockDatabase.CreateEntityWithRetry(entity, _logger, random.Next(5));
            if (result is BadRequestObjectResult)
            {
                return result;
            }

            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
        }

        // PUT: /api/Entity/{id}
        [HttpPut("{id}")]
        public IActionResult Put(string id, Entity entity)
        {
            if (id != entity.Id)
            {
                return BadRequest(); // Entity ID mismatch
            }

            MockDatabase.UpdateEntity(entity);
            return NoContent();
        }

        // DELETE: /api/Entity/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            MockDatabase.DeleteEntity(id);
            return NoContent();
        }
    }
}
