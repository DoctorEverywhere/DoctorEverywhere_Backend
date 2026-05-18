
# API documentation (controllers)

Readable API endpoint map. For request/response payloads, enum mappings, and data formats, see:
- `api_schemas.md`

## HTTP Status Codes Summary
- 200 OK
- 201 Created
- 400 Bad Request (invalid input / request cannot be processed)
- 401 Unauthorized (missing/invalid/expired token)
- 403 Forbidden (authenticated but insufficient role)
- 404 Not Found
- 409 Conflict (business rule conflict)
- 500 Internal Server Error

## Conventions
- Base route: `/api/{controller}` via ASP.NET Core `[Route("api/[controller]")]`.
- Auth header: `Authorization: Bearer <jwt_token>`.
- Roles used: `Patient`, `Doctor`, `Manager`.
- Date/time: ISO 8601 strings (example: `2026-05-18T14:30:00Z`).


## Create account (patient)
- `POST /api/auth/register/patient`

	- Body: 
```json
{
  "username": "string",
  "password": "string",
  "firstName": "string",
  "lastName": "string"
}
```
  - Responses: 200 / 400 / 500

## Create account (doctor)
- `POST /api/auth/register/doctor`
	- Body:
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
  - Responses: 200 / 400 / 500

## Login
- `POST /api/auth/login`

	- Body: 
```json
{
  "username": "string",
  "password": "string"
}
```
  - Success response: 200 OK `{ "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6..." }`
  - Responses: 200 / 401 / 500

## DoctorController
- `GET /api/doctor/{id}`
	- Auth: Doctor or Patient
  - Success response: `DoctorDto` 
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
  - Responses: 200 / 404 / 500

- `GET /api/doctor/search?specialty={int?}`
	- Auth: Patient
  - Query:
	- `specialty`: integer `Specialty` enum value
  - Success response: `DoctorDto[]` 
  - Responses: 200 / 500

- `GET /api/doctor/me`
	- Auth: Doctor
  - Success response: `DoctorDto` 
  - Responses: 200 / 500

- `DELETE /api/doctor/delete`
	- Auth: Doctor
  - Responses: 200 / 500

## PatientController
- `GET /api/patient`
	- Auth: Patient
  - Success response: `PatientDto[]` 
 ```json
{
  "id": 0,
  "firstName": "string",
  "lastName": "string"
}
```
  - Responses: 200 / 500

- `GET /api/patient/{id}`
	- Auth: Patient
  - Success response: `PatientDto` 
  - Responses: 200 / 404 / 500

- `GET /api/patient/my`
	- Auth: Patient
  - Success response: `PatientDto` 
  - Responses: 200 / 404 / 500

- `DELETE /api/patient/delete`
	- Auth: Patient
  - Responses: 200 / 500

## AppointmentController
- `POST /api/appointment/request?doctorId={int}`
	- Auth: Patient
  - Query: `doctorId` (int)
	- Body:
```json
{
  "startingAt": "2026-05-18T14:30:00Z"
}
```
  - Responses: 201 / 404 / 500

- `GET /api/appointment/my`
	- Auth: Doctor or Patient
  - Success response:
	- Patient: `AppointmentDto[]` 
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
 
- Doctor: object containing `appointments: AppointmentDto[]` and `result` (notification payload)
``` json
"result": {
    "messageId": "60ee...",
    "createdAt": "2026-05-15T11:48:05.2620683Z",
    "appointmentId": 2004,
    "doctorId": 2,
    "patientId": 2,
    "startingAt": "2026-05-20T20:00:00"
  }
 ```
 or null if there is no notification to consume.
  - Responses: 200 / 404 / 500

- `GET /api/appointment/{id}`
	- Auth: Doctor or Patient
  - Success response: `AppointmentDto` 
  - Responses: 200 / 404 / 500

- `PATCH /api/appointment/{id}/status`
	- Auth: Doctor or Patient
  - Body:
```json
{
  "statusId": 2
}
```
  - Rules:
	- Patient can only set status to `Cancelled`
	- Doctor cannot set status to `Cancelled`
  - Responses:
	- 200 / 403 / 404 / 409 / 500

## AvailabilityController
- `POST /api/availability/slots`
	- Auth: Doctor
  - Body:
```json
{
  "dayOfWeek": 0,
  "shiftStartTime": "09:00:00",
  "shiftEndTime": "17:00:00"
}
```
  - Responses: 200 / 500

- `GET /api/availability/slots`
	- Auth: Doctor
  - Success response: `AvailabilityDto[]`
  - Responses: 200 / 500

- `GET /api/availability/doctor/{doctorId}?date={DateTime}`
	- Auth: Patient
  - Route: `doctorId` (int)
  - Query: `date` (DateTime)
	- Success response: `string[]` of hour slots `HH:mm` (see `api_schemas.md`)
  - Responses: 200 / 500

## ReviewController
- `POST /api/review/{doctorId}`
	- Auth: Patient
  - Route: `doctorId` (int)
	- Body:
 ```json
{
  "rating": 5,
  "comments": "string"
}
```
  - Responses: 200 / 500

- `GET /api/review/{doctorId}`
	- Auth: Doctor, Patient, or Manager
  - Route: `doctorId` (int)
	- Success response: `ReviewDto[]` 
  - Responses: 200 / 404 / 500

## AnalyticsController
- `GET /summary`
	- Auth: Manager
  - Success response: 
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

  - Notes: this endpoint is not prefixed by `/api/analytics`.
  - Responses: 200 / 500

## Notes
- The API currently does not implement pagination/query conventions beyond the parameters documented above.
- Standard error response envelope is not implemented, responses commonly return a plain-text message body.
