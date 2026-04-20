Here's the improved `README.md` file, incorporating the new content while maintaining the existing structure and information:

# Quick-Gen

A sample learning platform built with ASP.NET Core and Entity Framework Core. This repository includes identity, course, lesson, and learning-path domain models, database migrations, and a database seeder for local development.

## Features

- Identity with roles and seeded users (Admin, Student)
- Course, Lesson, Learning Path, Enrollment, and Certificate domain models
- Database migrations with EF Core
- Local development seed data for quick evaluation

## Prerequisites

- .NET 10 SDK
- Visual Studio 2022 (recommended) or Visual Studio Code
- SQL Server (LocalDB/SQL Server) or another database supported by EF Core

## Quick start

1. Clone the repository:

   git clone https://github.com/3bdo-Yahya/Quick-Gen.git
   cd Quick-Gen

2. Restore and build in Visual Studio 2022 or using the CLI:

   dotnet restore
   dotnet build

3. Configure the connection string in `appsettings.Development.json` or user secrets. Example key:

   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=QuickGenDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }

4. (Optional) Configure seed admin credentials in `appsettings.Development.json`:

   "Seed": {
     "AdminEmail": "admin@quickgen.local",
     "AdminPassword": "Admin123!"
   }

If these keys are missing, the seeder uses sensible defaults.

5. Apply EF Core migrations and update the database:

   dotnet ef database update --project Quick_Gen

6. Run the application (the seeder runs on startup if wired in `Program.cs`):

   dotnet run --project Quick_Gen

If the seeder is not invoked automatically, you can call `Infrastructure.IdentitySeeder.SeedAsync` from `Program.cs` during startup to populate roles, users, and domain data.

## Database seeding

The project includes `Infrastructure/IdentitySeeder.cs` which:

- Creates required roles (`Admin`, `Student`)
- Creates seeded users and assigns roles
- Creates sample courses, lessons, learning paths, enrollments, and certificates

Use the `Seed:AdminEmail` and `Seed:AdminPassword` configuration keys to control the seeded admin account.

## Development notes

- This repository follows the code style and formatting defined in `.editorconfig`. Please follow those rules when contributing.
- See `CONTRIBUTING.md` for contribution guidelines, branch naming, and PR expectations.

## Tests

To run unit/integration tests, if present, use the following command (this project may include test projects in the solution):

dotnet test

## Contributing

1. Fork the repository and create a feature branch from `main`.
2. Follow the coding standards in `.editorconfig` and the guidelines in `CONTRIBUTING.md`.
3. Run and update tests.
4. Open a pull request with a clear description of changes.

## License

Specify the project license here (e.g., MIT). If no license file is included, add one before publishing.

## Contact

For questions, open an issue on the repository.

### Changes Made:
- Added code formatting for commands and JSON snippets for better readability.
- Ensured consistent use of bullet points and formatting throughout the document.
- Clarified the instructions in the "Tests" section for running tests.
- Maintained the overall structure and flow of the original document while enhancing clarity and usability.