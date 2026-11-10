
# RaceDay – Event Management System

## Project Overview

RaceDay is an event management system developed using ASP.NET Core Web API, ASP.NET Core MVC, C# and SQL Server.

The system is designed to help organisers manage sporting events, categories, participant registrations and race results.

Participants can use the system to view events, register for events and access their race information.

This project was developed as part of the RaceDay Portfolio of Evidence (POE).

## Technologies Used

- C#
- ASP.NET Core Web API
- ASP.NET Core MVC
- .NET 8
- Entity Framework Core
- Microsoft SQL Server
- HTML, CSS and Bootstrap
- Visual Studio 2022
- Git and GitHub
- GitHub Actions
- Docker

## Project Structure

The solution contains the following projects:

### RaceDay.API
The backend REST API responsible for handling requests, managing application data and communicating with SQL Server.

### Racedy.mvc
The ASP.NET Core MVC frontend responsible for displaying the website and communicating with the RaceDay API.

### RaceDay.API.Tests
Contains automated tests used to check the functionality of the application.

## Main Features

### Organiser Features

- Organiser login
- Organiser dashboard
- Create, view, update and delete events
- Manage event categories
- View event registrations
- Access race results management

### Participant Features

The participant functionality is designed to support:

- Viewing upcoming events
- Viewing event details
- Registering for event categories
- Viewing personal registrations
- Viewing race results
- Managing participant profiles

Some participant features may still require final implementation or testing.

## Database

RaceDay uses Microsoft SQL Server with Entity Framework Core.

The database stores information about:

- Organisers
- Participants
- Events
- Categories
- Registrations
- Race results

The database connection is configured in the API project's appsettings.json file.

## Running the Application Locally

### Requirements

- Visual Studio 2022
- .NET 8 SDK
- SQL Server
- SQL Server Management Studio (optional)

### Instructions

1. Clone or download the GitHub repository.
2. Open RaceDay.API.sln in Visual Studio 2022.
3. Restore the NuGet packages.
4. Configure the SQL Server connection string in the API.
5. Apply Entity Framework Core migrations if required.
6. Set RaceDay.API as the startup project.
7. Run the API.
8. Confirm that Swagger opens successfully.
9. Start the MVC project.
10. Open the MVC website in your browser.

The MVC application must be configured to communicate with the running API.

## Docker Setup

Docker is used to package the MVC application into a container.

### Requirements

- Docker Desktop
- Docker Engine running
- Linux containers enabled

### Build the Docker Image

Open a terminal in the MVC project directory containing the Dockerfile.

Run:

```bash
docker build -t raceday-mvc .
```

### Run the Docker Container

Start the RaceDay API first.

Then run:

```bash
docker run --rm -p 8080:8080 raceday-mvc
```

Open the application at:

http://localhost:8080

### Important Docker Configuration

When the MVC application runs inside Docker, localhost refers to the container itself.

If the API runs on the Windows host, the MVC application should use:

http://host.docker.internal:API_PORT

Replace API_PORT with the actual HTTP port of the RaceDay API.

The MVC API base URL must be configured correctly before the container can communicate with the API.

Docker functionality requires local build and runtime verification.

## GitHub Actions

The project uses GitHub Actions for continuous integration.

The workflow is configured to:

1. Download the repository source code.
2. Install the required .NET SDK.
3. Restore NuGet dependencies.
4. Build the solution.
5. Run automated tests.

The workflow runs when changes are pushed to the configured GitHub branches.

The latest workflow execution completed successfully.

## Azure Blob Storage

The project specification includes Azure Blob Storage for storing event banner images and participant profile pictures.

The intended design is for the MVC application to send image uploads to the API, which communicates with Azure Blob Storage.

Azure Blob Storage integration is not yet confirmed as fully implemented and tested.

## Security

- Session-based authentication
- Organiser and Participant roles
- Role-based access restrictions
- API validation
- Database access through Entity Framework Core

Sensitive information such as database passwords and Azure storage connection strings should not be committed to GitHub.

## Testing

The application includes an automated API test project.

Tests can be executed using:

```bash
dotnet test RaceDay.API.sln
```

GitHub Actions also runs the configured build and test steps.

## Known Limitations

- Participant functionality requires final verification.
- Azure Blob Storage integration is not yet verified.
- Docker container execution is not yet verified.
- Some results and registration workflows require end-to-end testing.

## Future Improvements

- Complete participant-facing features
- Finish Azure Blob Storage integration
- Improve event search and filtering
- Add more automated tests
- Improve mobile responsiveness
- Deploy the application to a hosting platform

## Conclusion

RaceDay demonstrates the development of an event management application using ASP.NET Core MVC, REST APIs, SQL Server, GitHub Actions and modern software development practices.

The application separates frontend and backend functionality and supports further improvements and deployment.
