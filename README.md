# Folio-Backend
A robust, Onion Architecture backend for my fullstack project Folio, 
an URL saving and management app. Built with .NET 10, EF CORE, PostgreSQL.

# Project Structure

```text
Folio-Backend/
├── FolioWebAPI/           # Presentation Layer (API)
│   ├── Controllers/       # REST Endpoints
│   ├── Extensions/        # Extensions
│   └── Middlewares/       # Global Error Handling and custom request pipelines.
│   └── Services/          # HttpContext Services
│   └── appsettings.json   # Configuration
├── Folio.Core/          # Application Core
│   ├── Application/     # Application Services, DTOs, Mappers and Attributes 
│   ├── Domain/          # Business logic, entities, and domain-specific exceptions.
│   ├── Interfaces/      # Contracts for repositories and external services
├── Folio.Infrastructure/     # Infrastructure Layer
│   ├── Identity/             # Identity Services, DI, Mappers
│   ├── Migrations/           # EF Core Migrations
│   └── Persistence/          # DB Context
│   └── Repositories/         # Concrete Implementation of repositories
└── Folio-Backend-Integration-Tests/   # End-to-end testing of API endpoints and DB persistence.
└── Folio-Backend-Unit-Tests/          # Isolated logic testing for Domain and Application layers.
