
# .NET Core Web REST API

## Overview
This API provides endpoints to manage entities. It allows you to perform CRUD operations on entities such as creating, reading, updating, and deleting.

## Deployment link
[https://addressbookweb20240303145342.azurewebsites.net/](https://addressbookweb20240303145342.azurewebsites.net/)

## Endpoints

### Get all entities
- **Method:** GET
- **Endpoint:** /api/Entity
- **Query Parameters:**
  - search: Search query to filter entities
  - gender: Filter entities by gender
  - startDate: Filter entities with a start date
  - endDate: Filter entities with an end date
  - countries: Filter entities by countries (supports multiple values)
- **Example:** /api/Entity?search=bob&gender=male&startDate=2022-12-15&endDate=2023-12-15&countries=india&countries=pakistan

### Create an entity
- **Method:** POST
- **Endpoint:** /api/Entity
- **Body:**
  ```json
  {
    "Id": "1",
    "Gender": "Male",
    "Deceased": false,
    "Addresses": [
      {
        "AddressLine": "123 Main Street",
        "City": "Cityville",
        "Country": "India"
      },
      {
        "AddressLine": "456 Elm Street",
        "City": "Townsville",
        "Country": "Countryland"
      }
    ],
    "Names": [
      {
        "FirstName": "John",
        "MiddleName": "Robert",
        "Surname": "Doe"
      }
    ],
    "Dates": [
      {
        "DateType": "Birth",
        "DateOfEvent": "1980-05-15"
      },
      {
        "DateType": "Graduation",
        "DateOfEvent": "2000-06-30"
      }
    ]
  }
### Delete an entity

-   **Method:** DELETE
-   **Endpoint:** /api/Entity/{id}

### Update an entity

-   **Method:** PUT
-   **Endpoint:** /api/Entity/{id}

### Get an entity by ID

-   **Method:** GET
-   **Endpoint:** /api/Entity/{id}

## Database Implementation

For the database, an in-memory dictionary approach is used. The database's actual implementation deviates slightly from the provided template.
  ```
public static class MockDatabase
{
    private static Dictionary<string, Entity> _entities = new Dictionary<string, Entity>();

    public static void CreateEntity(Entity entity)
    {
        _entities.Add(entity.Id, entity);
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
}
```
## Advanced Features

### 1. Page-based Pagination and Sorting

Implemented page-based pagination and sorting features for better organization and retrieval of entities.
> Because sorting based on the provided fields was not practical, I introduced a new field named createdDate to store the date of creation. This allows entities to be sorted based on this new criterion.

-   **Endpoint:** `GET /api/Entity`
-   **Query Parameters:**
    -   pageNumber: Page number to retrieve
    -   pageSize: Number of entities per page
    -   sortBy: Field to sort by (supports sorting by `createdDate`)
    -   sortOrder: Sorting order (`asc` for ascending, `desc` for descending)

Example:
-   **Method:** GET
-   **Endpoint:** api/Entity?pageNumber=2&pageSize=3&sortBy=createdDate&sortOrder=asc
-   **Response:**
```json
{
    "metadata": {
        "totalItems": 12,
        "pageSize": 3,
        "currentPage": 2,
        "totalPages": 4
    },
    "results": [...]
}
```

### 2. Retry and Backoff Mechanism for Database Write Operations

Implemented a retry and backoff mechanism for database write operations to handle transient failures gracefully.


The `CreateEntityWithRetry` method in the `MockDatabase` class attempts to create an entity in the database, with support for retrying failed attempts. Here's a concise explanation of its functionality:

-   **Retry Mechanism**: The method iterates through a maximum of three attempts to create the entity in the database.
-   **Backoff Strategy**: It employs an exponential backoff strategy, increasing the delay between retry attempts. The delay is calculated as follows:
```
private static TimeSpan CalculateBackoffDelay(TimeSpan initialDelay, TimeSpan maxDelay, double multiplier, int attempt)
{
    // Calculate delay using exponential backoff strategy
    double delaySeconds = Math.Min(initialDelay.TotalSeconds * Math.Pow(multiplier, attempt - 1), maxDelay.TotalSeconds);
    return TimeSpan.FromSeconds(delaySeconds);
}
```
-   **Error Handling**: If the operation fails, it logs the error and waits for the calculated backoff period before retrying. The method's parameter, permissibleAttempts, indicates the number of attempts needed for the creation operation to succeed. For instance, if permissibleAttempts equals 3, storing the entity in the mock database will fail in the initial 2 attempts but will succeed on the 3rd attempt.
-   **Maximum Attempts**: It limits the number of retry attempts to three to prevent indefinite retrying.
-   **Result**: It returns an `OkResult` if the entity is successfully created, or a `BadRequestObjectResult` if the maximum retry attempts are exhausted or if the entity already exists in the database.

#### Code Snippet for Create Entity with Retry:
```
public static IActionResult CreateEntityWithRetry(Entity entity, int permissibleAttempts)
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

            return new OkResult(); // Exit the loop if successful
        }
        catch (Exception ex)
        {
            if (attempt < maxAttempts)
            {
                // Calculate delay using exponential backoff strategy
                TimeSpan delay = CalculateBackoffDelay(initialDelay, maxDelay, backoffMultiplier, attempt);

                Thread.Sleep(delay); // Wait before retrying
            }
        }
    }
    
    return new BadRequestObjectResult("Database is unavailable.");
}
```

### Test Cases

Implemented test cases to simulate various scenarios:

1.  Success in the 1st attempt: If an `OkResult()` response is received, the test passes; otherwise, it fails.
2.  Success in the 2nd attempt: If an `OkResult()` response is received, the test passes; otherwise, it fails.
3.  Success in the 3rd attempt: If an `OkResult()` response is received, the test passes; otherwise, it fails.
4.  Success in the 4th attempt: If a `BadRequestObjectResult()` response is received, the test passes; otherwise, it fails.

### Logging Details

Serilog is used for logging to both console and file. Logging includes details such as the number of attempts, the delay before each attempt, and the ultimate success or failure of the operation.

![Screenshot 2024-03-03 165309](https://github.com/ayush-raj13/ASP.NET-Core-Web-Api/assets/113297899/964fc6c7-0ed7-439c-9b50-6a5b70b35a28)
