# DoctorEverywhere Backend

## Project overview
DoctorEverywhere Backend is an ASP.NET Core Web API backend for a location-aware doctor/patient appointment platform.
It exposes REST endpoints for authentication, doctor/patient profiles, availability, appointments, reviews, and analytics.

## Features
### Authentication & Identity
- Register as a **Patient** or **Doctor** in a single request (creates both Identity user + domain profile atomically)
- JWT Bearer tokens with role claims (`Patient`, `Doctor`, `Manager`)
- Tokens expire after **30 minutes**;

### Doctor Discovery
- Search doctors by **medical specialty** (10 specialties supported)
- View full doctor profiles including office location (name, address, city, coordinates)

### Availability Management
- Doctors define **weekly working schedules** (day-of-week + shift start/end times)
- Patients query available **hour slots** for any given doctor on a specific date

### Appointment Lifecycle
- Patients request appointments against a doctor's available slot
- Appointment status workflow: `Pending → Confirmed / Rejected / Cancelled `
- Role-enforced state transitions (patients can only cancel; doctors cannot cancel)
- Appointment creation **publishes a RabbitMQ message** to a doctor-specific queue
- Doctors receive the queued notification alongside their appointment list

### Reviews
- Patients leave a **rating + comment** for a doctor (one review per patient/doctor pair — enforced by unique DB index)
- Viewable by Doctors, Patients, and Managers

### Analytics (Manager - Backend Only)
- Summary report: appointments by status count, demand by specialty, doctor review statistics

### Soft-Delete & Cascade Logic
- Deleting a patient account: appointment records auto-cancelled, review names replaced with `"Deleted Patient"`
- Deleting a doctor account: their pending appointments auto-rejected
- Doctors and Patients use a **global query filter** on `IsActive` for soft-delete

## Contributors
| Name |
|------|
| Maria-Eleni Kosma |
| Dimitrios Loukrezis |
| Periklis Tsaousis |
| Marios Tzanos |

## Tech stack (Backend and Databases)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-10.0-blueviolet?logo=microsoft&logoColor=white)](https://learn.microsoft.com/en-us/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3.x-FF6600?logo=rabbitmq&logoColor=white)](https://www.rabbitmq.com/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)](https://docs.docker.com/compose/)
[![JWT](https://img.shields.io/badge/Auth-JWT%20Bearer-000000?logo=jsonwebtokens&logoColor=white)](https://jwt.io/)

| Layer | Technology |
|---|---|
| **Framework** | ASP.NET Core 10.0 Web API + Controllers |
| **ORM** | Entity Framework Core 10.0 (Code-First) |
| **Database** | Microsoft SQL Server 2022 |
| **Identity** | ASP.NET Core Identity + Roles |
| **Auth** | JWT Bearer (Microsoft.AspNetCore.Authentication.JwtBearer) |
| **Messaging** | RabbitMQ 3.x (via `RabbitMQ.Client` 7.x) |
| **API Docs** | Scalar UI (OpenAPI v3) |
| **Fake Data** | Bogus (for development seeding) |
| **Containers** | Docker Compose |
  
## Key directories
```
DoctorEverywhere_Backend/
├── .github/                        # GitHub Actions workflows
├── DoctorEverywhere/               # Main ASP.NET Core project
│   ├── Controllers/                # HTTP API surface — routing, auth attributes, status codes
│   │   ├── AuthController.cs       # Register patient/doctor, login
│   │   ├── DoctorController.cs     # Doctor profile & search
│   │   ├── PatientController.cs    # Patient profile management
│   │   ├── AppointmentController.cs# Full appointment lifecycle + RabbitMQ integration
│   │   ├── AvailabilityController.cs # Working schedule management
│   │   ├── ReviewController.cs     # Doctor reviews
│   │   └── AnalyticsController.cs  # Manager analytics summary
│   │
│   ├── Services/                   # Business logic layer
│   │   ├── Interfaces/             # Service contracts (registered in DI)
│   │   ├── AuthService.cs
│   │   ├── DoctorService.cs
│   │   ├── PatientService.cs
│   │   ├── AppointmentService.cs
│   │   ├── AvailabilityService.cs
│   │   ├── ReviewService.cs
│   │   └── AnalyticsService.cs
│   │
│   ├── Domain/                     # EF Core entity models
│   │   ├── ApplicationUser.cs      # ASP.NET Identity user (links to domain profiles)
│   │   ├── Doctor.cs
│   │   ├── Patient.cs
│   │   ├── Manager.cs
│   │   ├── Office.cs               # Doctor's clinic/office details + coordinates
│   │   ├── Appointment.cs
│   │   ├── Review.cs
│   │   ├── WorkingSchedule.cs      # Doctor's weekly shift schedule
│   │   └── Message.cs              # RabbitMQ-persisted notification messages
│   │
│   ├── DTOs/                       # API request/response data transfer objects
│   ├── Enums/                      # Specialty, AppointmentStatus, DayOfWeekOption
│   ├── Mappings/                   # Entity → DTO extension methods (e.g., Doctor)
│   ├── Messaging/                  # RabbitMQ infrastructure
│   │   ├── Configuration/          # RabbitMqSettings (bound from appsettings.json)
│   │   ├── DTOs/                   # Messaging-specific DTOs
│   │   ├── Interfaces/             # IRabbitMqProducerService, IRabbitMqConsumerService
│   │   └── Services/               # Producer & Consumer implementations (singletons)
│   ├── Migrations/                 # EF Core database migrations
│   ├── Exceptions/                 # Custom exception types (e.g., EntityNotFoundException)
│   ├── Documentation/              # Developer reference docs
│   │   ├── api_endpoints.md        # Full endpoint map with request/response examples
│   │   ├── api_schemas.md          # Request/response payload schemas & enum tables
│   │   └── architectural_patterns.md # Patterns and conventions used in this codebase
│   ├── ApplicationDbContext.cs     # EF Core DbContext with relationships & role seeding
│   ├── DbSeeder.cs                 # Seeds default Manager user on startup
│   ├── FakeDataSeeder.cs           # Seeds fake Doctors & Patients (dev only, uses Bogus)
│   ├── Program.cs                  # App entry point: DI, middleware, pipeline
│   ├── appsettings.json            # Connection strings, JWT config, RabbitMQ config
│   └── docker-compose.yml          # SQL Server + RabbitMQ local services
└── README.md
```

## 🚀 Getting Started

### Prerequisites

| Tool | Version | Notes |
|---|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | 10.0+ | Required to build and run the API |
| [Docker Desktop](https://www.docker.com/products/docker-desktop/) | Any recent | For SQL Server + RabbitMQ containers |
| [EF Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) | 10.0+ | For applying migrations (`dotnet tool install -g dotnet-ef`) |

---

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/DoctorEverywhere_Backend.git
cd DoctorEverywhere_Backend
```

---

### 2. Configure Environment

Create a `.env` file in the `DoctorEverywhere/` directory (next to `docker-compose.yml`) with the following variables:

```env
SQL_PASSWORD=YourStrong!Passw0rd
RABBIT_USER=admin
RABBIT_PASS=admin123
```

> [!IMPORTANT]
> The `SQL_PASSWORD` must meet SQL Server's password complexity requirements (uppercase, lowercase, digit, special character, min 8 chars).

Then verify or update the connection string in `appsettings.json` if needed:

```json
"ConnectionStrings": {
  "DoctorEverywhere": "Server=localhost,1433;Initial Catalog=DoctorEverywhere;User=sa;Password=YourStrong!Passw0rd;Trust Server Certificate=true;"
}
```

JWT and RabbitMQ settings are also configured in `appsettings.json`:

```json
"Jwt": {
  "Key": "<your-256-bit-secret>",
  "Issuer": "DoctorEverywhereAPI",
  "Audience": "DoctorEverywhereClients",
  "ExpiresInMinutes": 30
},
"RabbitMq": {
  "HostName": "localhost",
  "Port": 5672,
  "UserName": "admin",
  "Password": "admin123",
  "VirtualHost": "/",
  "QueueName": "appointments"
}
```

> [!WARNING]
> Never commit real secrets to version control. Use user secrets or environment variable overrides in production.

---

### 3. Start Infrastructure

From the `DoctorEverywhere/` directory (where `docker-compose.yml` lives):

```bash
docker compose up -d
```

This starts:
- **SQL Server 2022** → `localhost:1433`
- **RabbitMQ** → AMQP on `localhost:5672`, Management UI on `http://localhost:15672`

---

### 4. Run the API

From the `DoctorEverywhere/` directory:

```bash
# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update

# Start the API
dotnet run
```

The API will be available at `https://localhost:{port}`. In **Development** mode, the interactive **Scalar UI** is served at:

```
https://localhost:{port}/scalar/v1
```

> [!TIP]
> On first startup, `DbSeeder` and `FakeDataSeeder` automatically create a default **Manager** user and a set of sample **Doctors** and **Patients** so you can explore the API immediately.

---

### Build Reference

| Command | Description |
|---|---|
| `dotnet restore` | Restore NuGet packages |
| `dotnet build` | Compile the project |
| `dotnet run` | Start the development server |
| `dotnet ef database update` | Apply pending EF Core migrations |
| `dotnet ef migrations add <Name>` | Create a new migration |

---

## Additional documentation
- ![Architectural Patterns](DoctorEverywhere/Documentation/architectural_patterns.md) - patterns and conventions observed in this codebase
- ![API documentation](DoctorEverywhere/Documentation/api_endpoints.md) - controller-based API endpoint map (routes, auth roles, DTOs)
- ![API Schemas](DoctorEverywhere/Documentation/api_schemas.md) - consumer-facing request/response schemas, enums, and conventions
