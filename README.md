# AI Image Generator Backend

This is a simple backend built using ASP.NET Core Minimal APIs. It currently serves as a stub or test backend with basic user management and search functionalities.

## Requirements

- [.NET SDK](https://dotnet.microsoft.com/download)

## Getting Started

Navigate to the project directory and run the application using the .NET CLI:

```bash
dotnet run
```

The application will start and listen on the configured ports (typically `http://localhost:5000` or `https://localhost:5001`).

## API Endpoints

### 1. Root / Test
- **URL**: `/`
- **Method**: `GET`
- **Response**:
  ```json
  {
      "message": "test backend"
  }
  ```

### 2. Get User
- **URL**: `/user/{id}`
- **Method**: `GET`
- **Response**:
  ```json
  {
      "userId": 1,
      "name": "Vishal"
  }
  ```

### 3. Search
- **URL**: `/search?name={name}`
- **Method**: `GET`
- **Response**:
  ```json
  {
      "query": "search_term",
      "result": "Searching for search_term"
  }
  ```

### 4. Create User
- **URL**: `/user`
- **Method**: `POST`
- **Body** (JSON):
  ```json
  {
      "Id": 1,
      "Name": "John Doe",
      "Email": "john@example.com"
  }
  ```
- **Response**:
  ```json
  {
      "message": "User created successfully",
      "data": { ... }
  }
  ```

### 5. Update User
- **URL**: `/user/{id}`
- **Method**: `PUT`
- **Body** (JSON):
  ```json
  {
      "Id": 1,
      "Name": "John Doe Updated",
      "Email": "john.updated@example.com"
  }
  ```
- **Response**:
  ```json
  {
      "message": "User 1 updated",
      "updatedData": { ... }
  }
  ```

### 6. Delete User
- **URL**: `/user/{id}`
- **Method**: `DELETE`
- **Response**:
  ```json
  {
      "message": "User 1 deleted"
  }
  ```

## Models

### User
```csharp
public record User(int Id, string Name, string Email);
```
