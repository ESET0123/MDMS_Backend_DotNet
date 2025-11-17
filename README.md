# MDMS Backend (.NET)

This repository contains the backend service for a Meter Data Management System (MDMS), built with ASP.NET Core. It provides a comprehensive RESTful API for managing electrical grid components, consumers, meters, billing data, and user access. The system is designed to handle everything from infrastructure hierarchy (Zones, Substations, Feeders) to detailed daily meter readings and automated monthly bill generation.

## Core Features
*   **Authentication:** JWT-based authentication for both administrative users and consumers.
*   **Infrastructure Management:** Full CRUD operations for grid components including Zones, Substations, Feeders, and Distribution Transformers (DTRs).
*   **Consumer & Meter Management:** Manage consumer accounts and their associated meters. Includes bulk validation and creation of meters.
*   **Billing & Tariffs:** Sophisticated tariff management system with support for base rates, tariff slabs, and Time-of-Day (TOD) rules for dynamic pricing.
*   **Data Collection:** Endpoints for recording daily meter readings based on TOD rules, which automatically calculate consumption, effective rates, and costs.
*   **Automated Billing:** Functionality to generate monthly bills for consumers based on aggregated daily readings.
*   **User Administration:** Manage system users and their access roles.
*   **Dashboard Analytics:** An endpoint to provide key statistics about the system, such as total meters, active consumers, and more.

## Technology Stack
*   **Framework:** .NET 8 / ASP.NET Core Web API
*   **Database:** Microsoft SQL Server
*   **ORM:** Entity Framework Core 8
*   **Authentication:** JWT Bearer Tokens
*   **API Documentation:** Swashbuckle (Swagger)

## Getting Started

Follow these instructions to get a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites
*   .NET 8 SDK
*   A running instance of SQL Server (local, Docker, or Azure)
*   A code editor like Visual Studio or VS Code

### 1. Clone the Repository
```sh
git clone https://github.com/eset0123/mdms_backend_dotnet.git
cd mdms_backend_dotnet
```

### 2. Configure Database Connection
The application uses Entity Framework Core to connect to a SQL Server database. You need to configure the connection string.

Open `appsettings.json` and modify the `ConnectionStrings` section. For development, it is recommended to use User Secrets to store sensitive data.

To set the connection string via user secrets:
```sh
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:ConnectionDB" "Your_SQL_Server_Connection_String"
```

### 3. Configure JWT Settings
The JWT authentication settings are located in `appsettings.json`. You can modify the secret key and other parameters as needed.
```json
"Jwt": {
  "Key": "ThisIsASecretKeyForJWTAuthentication",
  "Issuer": "https://localhost:7272",
  "Audience": "https://localhost:7272",
  "DurationInMinutes": 60
}
```

### 4. Configure CORS
The allowed frontend URL for Cross-Origin Resource Sharing (CORS) is configured via an environment variable. For development, this is set in `Properties/launchSettings.json`.
```json
"environmentVariables": {
  "ASPNETCORE_ENVIRONMENT": "Development",
  "FRONTEND_URL": "https://localhost:7044"
}
```
Adjust the `FRONTEND_URL` to match your frontend application's address.

### 5. Install Dependencies
Restore the required NuGet packages.
```sh
dotnet restore
```

### 6. Run the Application
Execute the following command to start the application.
```sh
dotnet run
```
The API will be available at the URLs specified in `Properties/launchSettings.json` (e.g., `https://localhost:7272`). You can access the Swagger UI for interactive API documentation at `https://localhost:7272/swagger`.

## API Endpoints

The API provides a rich set of endpoints for managing the MDMS. Below is a summary grouped by controller.

*   **`api/Auth`**: Handles user and consumer login, returning a JWT.
*   **`api/Consumer`**: CRUD operations for consumer accounts.
*   **`api/Meter`**: CRUD for meters, including bulk validation and creation.
*   **`api/DailyMeterReading`**: Create and retrieve daily, time-slotted meter readings.
*   **`api/MonthlyBill`**: Generate, retrieve, and manage monthly bills.
*   **`api/Dashboard`**: Provides summary statistics for the system.
*   **`api/Zone`, `api/Substation`, `api/Feeder`, `api/Dtr`**: Manage the hierarchy of the electrical grid infrastructure.
*   **`api/Manufacturer`**: CRUD for meter manufacturers.
*   **`api/Tariff`, `api/TariffSlab`, `api/TodRule`**: Manage billing rules, rate slabs, and time-of-day pricing.
*   **`api/User`, `api/Role`**: Manage system administrators and their roles.
*   **`api/Status`**: Manage system-wide statuses (e.g., Active, Inactive).

## Project Structure
*   **`Controllers`**: Contains the API controllers that handle incoming HTTP requests and route them to the appropriate services.
*   **`Models`**: Defines the data structures (entities) and the `MdmsDbContext` for Entity Framework Core.
*   **`Repository`**: Implements the repository pattern for data access, abstracting database operations from the controllers. Each entity has a corresponding repository and interface.
*   **`Program.cs`**: The application's entry point, where services, dependency injection, middleware, and authentication are configured.
*   **`appsettings.json`**: Configuration file for the application, including database connection strings and JWT settings.
