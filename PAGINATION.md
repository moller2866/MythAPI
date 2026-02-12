# Pagination Support

## Overview

The MythAPI now supports pagination for all list-returning endpoints. This improves performance and usability when dealing with large datasets.

## Usage

### Query Parameters

All list endpoints now accept optional pagination parameters:

- `page` - The page number (1-based). Default: returns all results if not specified
- `pageSize` - The number of items per page. Default: 10, Maximum: 100

### Examples

#### Without Pagination (Backward Compatible)
```bash
# Returns all gods
GET /api/v1/gods

# Returns all mythologies  
GET /api/v1/mythologies
```

#### With Pagination
```bash
# Returns first page with 10 items (default page size)
GET /api/v1/gods?page=1

# Returns second page with 5 items per page
GET /api/v1/gods?page=2&pageSize=5

# Returns first page of mythologies with 20 items
GET /api/v1/mythologies?page=1&pageSize=20
```

## Response Format

### Non-Paginated Response
When pagination parameters are not provided, the response is a simple list:
```json
[
  {
    "id": 1,
    "name": "Zeus",
    "description": "God of the sky",
    "mythologyId": 1,
    "aliases": []
  },
  ...
]
```

### Paginated Response
When pagination parameters are provided, the response includes metadata:
```json
{
  "items": [
    {
      "id": 1,
      "name": "Zeus",
      "description": "God of the sky",
      "mythologyId": 1,
      "aliases": []
    },
    ...
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 42,
  "totalPages": 5,
  "hasNext": true,
  "hasPrevious": false
}
```

### Response Fields

- `items` - Array of items for the current page
- `page` - Current page number (1-based)
- `pageSize` - Number of items per page
- `totalCount` - Total number of items across all pages
- `totalPages` - Total number of pages available
- `hasNext` - Boolean indicating if there's a next page
- `hasPrevious` - Boolean indicating if there's a previous page

## Validation

The API automatically validates and corrects invalid pagination parameters:

- `page` values less than 1 are automatically set to 1
- `pageSize` values greater than 100 are automatically capped at 100
- `pageSize` values less than 1 are automatically set to the default (10)

## Endpoints Supporting Pagination

The following endpoints support pagination:

1. **GET /api/v1/gods** - List all gods
2. **GET /api/v1/mythologies** - List all mythologies

## Implementation Notes

- Pagination is implemented at the database level for optimal performance
- Uses Entity Framework's `Skip()` and `Take()` methods
- Eager loading is used to avoid N+1 query problems
- Results are ordered by ID to ensure consistent pagination

## Backward Compatibility

Existing clients that don't provide pagination parameters will continue to work as before, receiving the complete list of items. This ensures no breaking changes for existing integrations.

## Best Practices

1. **Use appropriate page sizes**: Balance between reducing the number of requests and keeping response sizes manageable
2. **Handle edge cases**: Always check `hasNext` and `hasPrevious` before navigating
3. **Cache when possible**: If data doesn't change frequently, consider caching paginated results
4. **Consistent parameters**: Use the same page size across related requests for better caching
