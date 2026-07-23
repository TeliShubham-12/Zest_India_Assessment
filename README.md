# Student Management System - Full Stack .NET 9 Web API

A production-ready **Student Management System** built with **ASP.NET Core Web API (.NET 9)**, **Entity Framework Core Code-First**, **Generic Repository & Unit of Work Architecture**, **SQL Server**, **JWT Authentication**, **Role-Based Authorization**, **Global Exception Handling Middleware**, **Serilog Logging**, and **Swagger Documentation**.

---

## 🏛️ Project Architecture

The solution follows a clean **Layered Architecture** adhering to SOLID principles:

```
Zest_India_Assessment/
│
├── StudentManagement.Domain/        # Domain Entities, BaseEntity, Repository Interfaces
├── StudentManagement.Application/   # Business Services, DTOs, Mapping, JWT Token Generator
├── StudentManagement.Infrastructure/# EF Core DbContext, Generic Repository, Unit of Work, Migrations
├── StudentManagement.API/           # Controllers, Middlewares, Swagger, Static Web UI
└── StudentManagement.UI/            # Angular Standalone SPA Frontend
```

---

## ✨ Key Features

1. **EF Core Code-First Architecture**:
   - Uses SQL Server database with Entity Framework Core migrations.
   - Includes automatic data seeding for default Admin user and initial sample students.

2. **Generic Repository & Unit of Work Pattern**:
   - **`IGenericRepository<T>`**: Reusable CRUD repository interface operating on all domain entities (`Student`, `User`).
   - **`IUnitOfWork`**: Coordinates generic repositories and ensures single atomic `SaveAsync()` transactions.

3. **JWT Authentication & Role-Based Authorization**:
   - Secure authentication endpoints (`POST /api/auth/register` and `POST /api/auth/login`).
   - Generates signed JWT Bearer Tokens with claims (`NameIdentifier`, `Name`, `Email`, `Role`).
   - **Role Restriction**: `DELETE /api/students/{id}` is restricted exclusively to **`Admin`** users via `[Authorize(Roles = "Admin")]`.
   - **Custom Unauthorized & Forbidden Responses**: Non-admin users attempting to delete receive an **HTTP 403 Forbidden** JSON response with a clear message.

4. **Global Exception Handling Middleware**:
   - `ExceptionHandlingMiddleware` catches all unhandled exceptions globally and formats them into a standardized `ApiResponse<T>` JSON format.

5. **Serilog Structured Logging**:
   - Configured to log output to both Console and rolling daily log files in `Logs/student_management_.log`.

6. **Interactive Web UI**:
   - Responsive web interface featuring authentication switching and a floating bottom-center notification banner.

---

## 🚀 Setup & Execution Guide

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express, Developer, or LocalDB)
- [EF Core CLI Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (`dotnet tool install --global dotnet-ef`)

---

### Step 1: Clone the Repository
```bash
git clone https://github.com/TeliShubham-12/Zest_India_Assessment.git
cd Zest_India_Assessment
```

---

### Step 2: Configure Database Connection String

Open `StudentManagement.API/appsettings.json` and set your SQL Server connection string under `DefaultConnection`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=StudentManagementDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;Encrypt=False;"
  }
}
```

*Example for SQL Server Express with SQL Authentication:*
```json
"DefaultConnection": "Server=SHUBHAM\\SQLEXPRESS;Database=StudentManagementDb;User Id=Sahil;Password=password;TrustServerCertificate=True;Encrypt=False;"
```

*Example for Windows Authentication / Trusted Connection:*
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudentManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

---

### Step 3: Run EF Core Migrations & Update Database

Apply the EF Core Code-First migration to create the `StudentManagementDb` database and tables:

```bash
dotnet ef database update --project StudentManagement.Infrastructure/StudentManagement.Infrastructure.csproj --startup-project StudentManagement.API/StudentManagement.API.csproj
```

---

### Step 4: Run the Application

Navigate to the API project and run the server:

```bash
dotnet run --project StudentManagement.API
```

Once running, access the services in your browser:
- **Web UI Application**: `http://localhost:5257`
- **Swagger Interactive API Documentation**: `http://localhost:5257/swagger`

---

## 🔑 Default Credentials & Role Authorization

### Default Admin Credentials (Seeded Automatically)
- **Username**: `admin`
- **Password**: `Admin@123`
- **Role**: `Admin` *(Has full access including Student Delete)*

### Regular User Registration
- Register a new account on the UI or via `POST /api/auth/register`.
- Regular accounts default to `Role: User`.
- **Note**: Calling `DELETE /api/students/{id}` with a regular user token returns **HTTP 403 Forbidden**:
  ```json
  {
    "success": false,
    "message": "Access Denied: You are not authorized to perform this operation. Only Admin users can delete student records.",
    "data": null,
    "errors": []
  }
  ```

---

## 📡 API Endpoints Reference

### Authentication Endpoints
| HTTP Method | Endpoint | Description | Auth Required |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/auth/register` | Register a new user account | No |
| `POST` | `/api/auth/login` | Authenticate user & issue JWT token | No |

### Student Management Endpoints
| HTTP Method | Endpoint | Description | Auth Required | Minimum Role |
| :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/students` | Get list of all students | Yes (Bearer JWT) | `User` / `Admin` |
| `GET` | `/api/students/{id}` | Get student by ID | Yes (Bearer JWT) | `User` / `Admin` |
| `POST` | `/api/students` | Add a new student record | Yes (Bearer JWT) | `User` / `Admin` |
| `PUT` | `/api/students/{id}` | Update existing student by ID | Yes (Bearer JWT) | `User` / `Admin` |
| `DELETE` | `/api/students/{id}` | Delete student by ID | Yes (Bearer JWT) | **`Admin` Only** |

---

## 🔒 Testing JWT Authentication in Swagger

1. Go to `http://localhost:5257/swagger`.
2. Execute `POST /api/auth/login` with credentials (`admin` / `Admin@123`).
3. Copy the returned token from the response body.
4. Click the **Authorize 🔓** button at the top right of Swagger.
5. Paste the token into the Value field: `Bearer <YOUR_JWT_TOKEN>` and click **Authorize**.
6. You can now execute all protected student endpoints.

## 📝 License
This project is developed as a technical assessment for Zest India IT Pvt Ltd.