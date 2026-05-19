# DoctorEverywhere Backend

## Project overview
DoctorEverywhere Backend is an ASP.NET Core Web API backend for a location-aware doctor/patient appointment platform.
It exposes REST endpoints for authentication, doctor/patient profiles, availability, appointments, reviews, and analytics.

## Features
- Authentication
  - Register as Patient or Doctor
  - Login with JWT
- Doctor discovery
  - Search doctors by specialty
  - View doctor profile (including office/location details)
- Availability management
  - Doctors can create/update working schedule (availability slots)
  - Patients can view a doctor’s available time slots for a given date
- Appointments
  - Patients can request appointments with a doctor
  - Patients and doctors can view their appointments
  - Appointment status updates (patient cancellation; doctor confirm/reject)
- Reviews
  - Patients can leave a rating/comment review for a doctor
  - Doctors/patients/managers can view doctor reviews
- Analytics (Manager)
  - Summary reporting for appointments by status, demand by specialty, and reviews (backend implementation only)
- User deletion
   - Users (Patient/Doctors) can delete their account,
      - Patient names are marked as "Deleted Patient" in their reviews,and their appointments are auto-cancelled
      - Doctor appointments are auto-rejected

## Contributors
- Maria-Eleni Kosma
- Dimitrios Loukrezis 
- Periklis Tsaousis
- Marios Tzanos

## Tech stack (Backend and Databases)
- ASP.NET Core Web API + Controllers
- Entity Framework Core + SQL Server 
- ASP.NET Core Identity + Roles 
- JWT Bearer auth 
- RabbitMQ messaging (producer/consumer services) 
- Scalar UI for HTTP endpoint testing
- Docker Compose for local SQL Server and RabbitMQ 
  
## Key directories
- (Controllers) - HTTP API surface (routing, auth attributes, status code mapping)
- (Services) - business logic; called by controllers
- (Services/Interfaces) - service contracts registered in DI
- (Domain) - EF Core entities (Doctor, Patient, Appointment, etc.)
- (DTOs) - API request/response DTOs
- (Mappings) - mapping helpers from entities to DTOs (for Doctor)
- (Messaging) - RabbitMQ configuration, DTOs, interfaces, services
- (Migrations) - EF Core migrations

## Essential build/test commands
From the project root directory (where DoctorEverywhere.csproj is):
- Restore: `dotnet restore`
- Build: `dotnet build`
- Run API: `dotnet run`
- Apply migrations: `dotnet ef database update`

## Local infrastructure:
- Create env file and fill in the fields:
- `SQL_PASSWORD=` 
- `RABBIT_USER=`
- `RABBIT_PASS=`

- Start SQL Server + RabbitMQ in project root: `docker compose up -d` 

## Operational notes
- JWT settings and connection strings live in appsettings.json.
- Roles are seeded via EF model seeding in ApplicationDbContext.cs.
- A default Manager user,Doctors and Patients are created at startup by DbSeeder and FakeDataSeeder respectively.

## Additional documentation
- documentation/architectural_patterns.md - cross-cutting patterns and conventions observed in this codebase
- documentation/api_endpoints.md - controller-based API endpoint map (routes, auth roles, DTOs)
- documentation/api_schemas.md - consumer-facing request/response schemas, enums, and conventions
