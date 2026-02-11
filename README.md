# INCC API

INCC API provides the Brazilian construction cost index (INCC) data and calculates accumulated variation between two dates. It exposes paged listings, date-range queries and an accumulated-variation endpoint.

## Technologies

- .NET 10 / C# 14
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL (Supabase)
- Azure App Service
- OpenAPI / Swagger

## Main Endpoints

| Route | Method | Description |
|---|---:|---|
| `/api/incc` | GET | Paged list of INCC entries (use query params for page/size). |
| `/api/incc/{year}/{month}` | GET | Retrieve a single INCC value for a specific year and month. |
| `/api/incc/range` | GET | Paged list of INCC entries within a date range (start/end query params). |
| `/api/incc/accumulated` | GET | Calculate accumulated variation between two dates (startYear/startMonth and endYear/endMonth query params). |

## How to use

Example request (accumulated variation):  
`curl "https://incc-api-ewdqbyfagqexeddc.brazilsouth-01.azurewebsites.net/api/incc/accumulated?amount=100000&startYear=2020&startMonth=1&endYear=2020&endMonth=12"`

Example successful JSON response:  
`{
  "accumulatedVariation": 8.3878,  
  "adjustedValue": 108387.8478,
  "startDate": "01/2020",
  "endDate": "12/2020"
}`

Example error response:  
`{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "00-f8935bb8459c9f3ac463fd72c43e18e6-d9160a097437ee87-00"
}`

> The `traceId` returned on errors is intended to correlate client-reported failures with server logs

## Deployment

The API is deployed to Azure App Service. OpenAPI/Swagger is available in development; logging integrates with Azure Web App diagnostics.

Live API URL: [https://incc-api.azurewebsites.net](https://incc-api-ewdqbyfagqexeddc.brazilsouth-01.azurewebsites.net/api/incc)

## Quick start (local)

Prerequisites:
- .NET 10 SDK
- PostgreSQL (or Supabase instance)

Steps:
1. Set the connection string to the `appsettings.json` file.
2. Run EF Core migrations:
   - `dotnet ef database update`
3. Start the app:
   - `dotnet run`
4. Open Swagger (when running in Development): `https://localhost:{port}/swagger`
