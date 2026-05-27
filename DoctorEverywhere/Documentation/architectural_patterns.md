# Architectural patterns and conventions

```
┌──────────────────────────────────────────────────────┐
│                    Angular Frontend                   │
│               (http://localhost:4200)                 │
└──────────────────┬───────────────────────────────────┘
                   │ HTTP + JWT Bearer
┌──────────────────▼───────────────────────────────────┐
│              ASP.NET Core Web API                     │
│                                                       │
│  Controllers  →  Services  →  EF Core DbContext       │
│                    ↕                                  │
│            RabbitMQ Producer/Consumer                 │
└──────────┬───────────────────────┬────────────────────┘
           │                       │
┌──────────▼──────┐    ┌───────────▼──────────┐
│   SQL Server    │    │      RabbitMQ         │
│  (port 1433)    │    │  (ports 5672/15672)   │
└─────────────────┘    └──────────────────────┘
```

# Layering conventions:

## Dependency Injection (DI)
- Services are registered with ASP.NET Core DI in Program.cs.
  - Messaging services are singletons: `IRabbitMqProducerService`, `IRabbitMqConsumerService`.
  - Domain services are scoped: doctor/patient/appointment/etc. 
- Controllers depend on service interfaces rather than concrete implementations.

## Layering: Controllers -> Services -> EF Core
- Controllers primarily:
  - read auth context (`ClaimTypes.NameIdentifier`) and route/query/body params
  - call services
  - translate exceptions into HTTP status codes
  - example:

```json
catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
```

## Authentication/Authorization
- JWT Bearer authentication is used and configured in Program.cs with Identity integration.
- Role-based authorization is applied via `[Authorize(Roles = "...")]` on endpoints.
- Token generation is handled in the auth service using Identity roles as `ClaimTypes.Role`.

## Identity + domain profile separation
- Identity user is `ApplicationUser`, and domain profiles (Doctor/Patient/Manager) link 1:1 via `ApplicationUserId`.
  - Relationship is configured in ApplicationDbContext.
  - Registration creates both Identity user and domain profile in one operation (examples: [Services/AuthService.cs:44-93](../../Services/AuthService.cs)).

## EF Core model conventions
- Role seeding is done via `OnModelCreating` with `IdentityRole` HasData.
- Relationships are configured explicitly with delete behaviors.
- Several enums are stored as integers via conversions.
- Soft-delete pattern via `IsActive` with global query filters for Doctors/Patients.

## DTO usage and mapping
- Services typically return DTOs to controllers, not EF entities.
- Some entity-to-DTO mapping is extracted into extension methods, like in Doctor.

## Messaging pattern (RabbitMQ)
- Appointment creation publishes a message to a doctor-specific queue name `appointment-doctor-{DoctorId}` within AppointmentController.
- Doctor appointment retrieval consumes at most one message per request via BasicGet (polling) and returns it alongside appointment list,also in AppointmentController.

## Startup seeding
- Startup creates a scope and runs a seeder to ensure a Manager role/user/profile exists.
