# API schemas and conventions

This document defines the request/response payload shapes used by the API.

## Auth
### Authorization header
Send the JWT access token in this header:
- `Authorization: Bearer <jwt_token>`

### Roles
- `Patient`
- `Doctor`
- `Manager`

### Token details
- Access tokens expire after 30 minutes.
- There is no refresh-token endpoint implemented.

## Data formats
### DateTime
All `DateTime` fields are JSON strings in ISO 8601 format (UTC recommended), for example:
- `2026-05-18T14:30:00Z`

### TimeSpan
`TimeSpan` fields are JSON strings in .NET “constant” format, for example:
- `"09:00:00"`

### IDs
All entity IDs in routes are integers unless stated otherwise.

## Enums (serialized as integers)
### Specialty
`Specialty` is sent/returned as an integer with this mapping:
- 0 = GeneralPractitioner
- 1 = Cardiologist
- 2 = Dermatologist
- 3 = Neurologist
- 4 = Pediatrician
- 5 = Psychiatrist
- 6 = Orthopedic
- 7 = Gynecologist
- 8 = Dentist
- 9 = Ophthalmologist
- 10 = Deleted

### AppointmentStatus
`AppointmentStatus` is sent/returned as an integer with this mapping:
- 0 = Pending
- 1 = Confirmed
- 2 = Cancelled
- 3 = Rejected
- 4 = Rescheduled

### DayOfWeekOption
`DayOfWeekOption` is sent/returned as an integer with this mapping:
- 0 = Monday
- 1 = Tuesday
- 2 = Wednesday
- 3 = Thursday
- 4 = Friday
- 5 = Saturday
- 6 = Sunday

## Standard error responses
### Current behavior
Error responses are returned as a plain-text message body.

Examples:
- `404 Not Found` body: `Doctor with ID 123 not found.`
- `409 Conflict` body: `Invalid state transition` (message varies)

## Request/response schemas
### Login
Request: `POST /api/auth/login`

```json
{
  "username": "string",
  "password": "string"
}
```

Response (200):

```json
{
  "token": "string"
}
```

### Register patient
Request: `POST /api/auth/register/patient`

```json
{
  "username": "string",
  "password": "string",
  "firstName": "string",
  "lastName": "string"
}
```

Response (200): empty body.

### Register doctor
Request: `POST /api/auth/register/doctor`

```json
{
  "username": "string",
  "password": "string",
  "firstName": "string",
  "lastName": "string",
  "specialty": 0,
  "officeName": "string",
  "officeAddress": "string",
  "officeCity": "string",
  "officePostalCode": "string",
  "latitude": 0.0,
  "longitude": 0.0
}
```

Response (200): empty body.

### DoctorDto
Returned by doctor endpoints.

```json
{
  "id": 0,
  "firstName": "string",
  "lastName": "string",
  "specialty": 0,
  "office": {
	"id": 0,
	"name": "string",
	"address": "string",
	"city": "string",
	"postalCode": "string",
	"latitude": 0.0,
	"longitude": 0.0
  }
}
```

### PatientDto
Returned by patient endpoints.

```json
{
  "id": 0,
  "firstName": "string",
  "lastName": "string"
}
```

### Create appointment
Request: `POST /api/appointment/request?doctorId={doctorId}`

```json
{
  "startingAt": "2026-05-18T14:30:00Z"
}
```

Response (201): empty body.

### AppointmentDto
Returned by appointment endpoints.

```json
{
  "id": 0,
  "patientId": 0,
  "doctorId": 0,
  "startingAt": "2026-05-18T14:30:00Z",
  "statusId": 0,
  "requestedAt": "2026-05-18T12:00:00Z",
  "doctorName": "string",
  "patientName": "string"
}
```

### Update appointment status
Request: `PATCH /api/appointment/{id}/status`

```json
{
  "statusId": 2
}
```

Response (200): `AppointmentDto`.

### Availability slot
Used in availability endpoints.

```json
{
  "dayOfWeek": 0,
  "shiftStartTime": "09:00:00",
  "shiftEndTime": "17:00:00"
}
```

`GET /api/availability/doctor/{doctorId}?date=...` returns a list of hour strings:

```json
["09:00", "10:00", "11:00"]
```

### Create review
Request: `POST /api/review/{doctorId}`

```json
{
  "rating": 5,
  "comments": "string"
}
```

Response (200): empty body.

### ReviewDto
Returned by `GET /api/review/{doctorId}`.

```json
{
  "patientFirstName": "string",
  "patientLastName": "string",
  "rating": 0,
  "comments": "string",
  "createdAt": "2026-05-18T14:30:00Z"
}
```

### AnalyticsSummaryDto
Returned by `GET /summary`.

```json
{
  "appointmentsByStatusCount": [
	{ "status": 0, "count": 0 }
  ],
  "demandBySpecialtyCount": [
	{ "specialty": 0, "count": 0 }
  ],
  "reviewsByRatingCount": [
	{
	  "doctorId": 0,
	  "doctorName": "string",
	  "specialty": 0,
	  "reviewCount": 0,
	  "averageRating": 0.0
	}
  ]
}
```
