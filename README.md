# AI Image Generator Backend

This is a comprehensive backend built using ASP.NET Core with JWT authentication, PostgreSQL database integration, and RESTful API design.

## Requirements

- [.NET SDK](https://dotnet.microsoft.com/download)
- PostgreSQL
- Entity Framework Core

## Getting Started

Navigate to project directory and run the application:

```bash
dotnet run
```

The application will start on `https://localhost:7000` (default) and Swagger UI will be available at `https://localhost:7000/swagger`.

## API Documentation

The API follows RESTful conventions with versioned endpoints. All endpoints are documented in Swagger UI.

### Base URL
```
https://localhost:7000/api/v1
```

## Authentication Endpoints (`/api/v1/auth`)

### 1. User Registration
- **URL**: `/api/v1/auth/register`
- **Method**: `POST`
- **Headers**: `Content-Type: application/json`
- **Body**:
  ```json
  {
    "name": "John Doe",
    "email": "john@example.com",
    "password": "password123"
  }
  ```
- **Response** (201 Created):
  ```json
  {
    "message": "User registered successfully",
    "userId": 1,
    "email": "john@example.com",
    "name": "John Doe",
    "token": "jwt_token_here",
    "expiresIn": "1 hour"
  }
  ```

### 2. User Login
- **URL**: `/api/v1/auth/login`
- **Method**: `POST`
- **Headers**: `Content-Type: application/json`
- **Body**:
  ```json
  {
    "username": "john@example.com",
    "password": "password123"
  }
  ```
- **Response** (200 OK):
  ```json
  {
    "token": "jwt_token_here",
    "userId": 1,
    "username": "john@example.com",
    "name": "John Doe",
    "expiresIn": "1 hour"
  }
  ```

### 3. Refresh Token
- **URL**: `/api/v1/auth/refresh`
- **Method**: `POST`
- **Headers**: `Authorization: Bearer <jwt_token>`
- **Response** (200 OK):
  ```json
  {
    "token": "new_jwt_token_here",
    "expiresIn": "1 hour"
  }
  ```

### 4. Logout
- **URL**: `/api/v1/auth/logout`
- **Method**: `POST`
- **Headers**: `Authorization: Bearer <jwt_token>`
- **Response** (200 OK):
  ```json
  {
    "message": "Logged out successfully"
  }
  ```

## User Management Endpoints (`/api/v1/users`)

*All user management endpoints require JWT authentication*

### 5. Get All Users (Paginated)
- **URL**: `/api/v1/users`
- **Method**: `GET`
- **Headers**: `Authorization: Bearer <jwt_token>`
- **Query Parameters**:
  - `page`: Page number (default: 1)
  - `pageSize`: Items per page (default: 10)
- **Response** (200 OK):
  ```json
  {
    "users": [...],
    "pagination": {
      "currentPage": 1,
      "pageSize": 10,
      "totalCount": 25,
      "totalPages": 3
    }
  }
  ```

### 6. Get User by ID
- **URL**: `/api/v1/users/{id}`
- **Method**: `GET`
- **Headers**: `Authorization: Bearer <jwt_token>`
- **Response** (200 OK):
  ```json
  {
    "id": 1,
    "name": "John Doe",
    "email": "john@example.com",
    "createdAt": "2024-01-01T00:00:00Z"
  }
  ```

### 7. Search Users
- **URL**: `/api/v1/users/search`
- **Method**: `GET`
- **Headers**: `Authorization: Bearer <jwt_token>`
- **Query Parameters**:
  - `name`: Search term (required)
  - `page`: Page number (default: 1)
  - `pageSize`: Items per page (default: 10)
- **Response** (200 OK):
  ```json
  {
    "users": [...],
    "searchTerm": "john",
    "pagination": {
      "currentPage": 1,
      "pageSize": 10,
      "totalCount": 5,
      "totalPages": 1
    }
  }
  ```

### 8. Create User
- **URL**: `/api/v1/users`
- **Method**: `POST`
- **Headers**: 
  - `Authorization: Bearer <jwt_token>`
  - `Content-Type: application/json`
- **Body**:
  ```json
  {
    "name": "Jane Doe",
    "email": "jane@example.com",
    "password": "password123"
  }
  ```
- **Response** (201 Created):
  ```json
  {
    "message": "User created successfully",
    "data": {
      "id": 2,
      "name": "Jane Doe",
      "email": "jane@example.com",
      "createdAt": "2024-01-01T00:00:00Z"
    }
  }
  ```

### 9. Update User
- **URL**: `/api/v1/users/{id}`
- **Method**: `PUT`
- **Headers**: 
  - `Authorization: Bearer <jwt_token>`
  - `Content-Type: application/json`
- **Body**:
  ```json
  {
    "name": "John Updated",
    "email": "john.updated@example.com"
  }
  ```
- **Response** (200 OK):
  ```json
  {
    "message": "User 1 updated",
    "updatedData": {
      "id": 1,
      "name": "John Updated",
      "email": "john.updated@example.com"
    }
  }
  ```

### 10. Delete User
- **URL**: `/api/v1/users/{id}`
- **Method**: `DELETE`
- **Headers**: `Authorization: Bearer <jwt_token>`
- **Response** (200 OK):
  ```json
  {
    "message": "User 1 deleted"
  }
  ```

### 11. Get User Profile
- **URL**: `/api/v1/users/{id}/profile`
- **Method**: `GET`
- **Headers**: `Authorization: Bearer <jwt_token>`
- **Response** (200 OK):
  ```json
  {
    "id": 1,
    "name": "John Doe",
    "email": "john@example.com",
    "createdAt": "2024-01-01T00:00:00Z"
  }
  ```

## Health Check Endpoints (`/api/v1/health`)

### 12. Basic Health Check
- **URL**: `/api/v1/health`
- **Method**: `GET`
- **Response** (200 OK):
  ```json
  {
    "status": "healthy",
    "timestamp": "2024-01-01T00:00:00Z",
    "version": "1.0.0",
    "environment": "Development"
  }
  ```

### 13. Detailed Health Check
- **URL**: `/api/v1/health/detailed`
- **Method**: `GET`
- **Response** (200 OK):
  ```json
  {
    "status": "healthy",
    "timestamp": "2024-01-01T00:00:00Z",
    "version": "1.0.0",
    "environment": "Development",
    "services": {
      "database": "connected",
      "authentication": "enabled",
      "logging": "enabled"
    }
  }
  ```

## Configuration

### Database Configuration
Update connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=your_database;Username=your_username;Password=your_password"
  }
}
```

### JWT Configuration
Set JWT secrets using user secrets:
```bash
dotnet user-secrets set "Jwt:Key" "YourSecretKeyHere123456789"
dotnet user-secrets set "Jwt:Issuer" "AIImageGeneratorBackend"
dotnet user-secrets set "Jwt:Audience" "AIImageGeneratorUsers"
```

## API Features

### 🔄 RESTful Design
- **Resource-based routing** (`/api/v1/users`, `/api/v1/auth`)
- **HTTP method semantics** (GET, POST, PUT, DELETE)
- **Proper status codes** (200, 201, 400, 401, 404, 500)
- **Pagination support** for list endpoints

### 🔒 Security
- **JWT Authentication**: Secure token-based authentication
- **Password Hashing**: SHA256 hashing for password storage
- **Token Refresh**: Endpoint to refresh expired tokens
- **Authorization**: Role-based access control ready

### 📊 Response Format
- **Consistent JSON structure** across all endpoints
- **Error messages** with clear descriptions
- **Pagination metadata** for list responses
- **Timestamps** in ISO 8601 format

## Error Handling

The API returns appropriate HTTP status codes:
- `200 OK`: Successful request
- `201 Created`: Resource created successfully
- `400 Bad Request`: Invalid input data or validation errors
- `401 Unauthorized`: Authentication failed or missing token
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

## Swagger Documentation

Interactive API documentation is available at:
```
https://localhost:7000/swagger
```

Features:
- **Try it out** functionality
- **Request/response examples**
- **Authentication support**
- **Parameter validation**

## Development

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

### User Secrets Management
```bash
# List all user secrets
dotnet user-secrets list

# Set a secret
dotnet user-secrets set "SecretName" "SecretValue"

# Remove a secret
dotnet user-secrets remove "SecretName"
```

## API Usage Examples

### Register a new user
```bash
curl -X POST https://localhost:7000/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{"name":"John Doe","email":"john@example.com","password":"password123"}'
```

### Login with credentials
```bash
curl -X POST https://localhost:7000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"john@example.com","password":"password123"}'
```

### Get all users (with pagination)
```bash
curl -X GET "https://localhost:7000/api/v1/users?page=1&pageSize=5" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Search users
```bash
curl -X GET "https://localhost:7000/api/v1/users/search?name=john&page=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Technology Stack

- **.NET 10.0**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **PostgreSQL**
- **JWT Authentication**
- **Swagger/OpenAPI**
- **Docker Support**

## Docker Support

### Quick Start with Docker Compose

```bash
# Start all services (database + API)
docker-compose up --build

# Run in background
docker-compose up -d --build

# Stop services
docker-compose down

# View logs
docker-compose logs -f
```

### Services

- **PostgreSQL Database**: Port 5432
- **ASP.NET Core API**: Ports 7000 (HTTP) and 7001 (HTTPS)
- **Database**: `ai_image_generator`
- **Persistent data**: Stored in Docker volume

### Access Points

- **API**: `http://localhost:7000` or `https://localhost:7001`
- **Swagger**: `http://localhost:7000/swagger`
- **Database**: `localhost:5432` (for direct access)

### Environment Variables

The Docker Compose file includes:
- Database connection string
- JWT configuration
- Development environment settings

### Build and Run

```bash
# Build Docker image only
docker build -t ai-image-generator-backend .

# Run single container
docker run -p 7000:80 -p 7001:443 ai-image-generator-backend
```
