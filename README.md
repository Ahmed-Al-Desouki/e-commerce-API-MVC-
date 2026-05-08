<div align="center">

# 🛒 E-Commerce System

### Clean Onion Architecture · .NET Web API + MVC + SQL Server

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-Code%20First-purple?style=flat-square)](https://docs.microsoft.com/en-us/ef/core/)
[![xUnit](https://img.shields.io/badge/Tests-68%20Passing-brightgreen?style=flat-square)](https://xunit.net/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Onion-blue?style=flat-square)]()

</div>

---

## 📌 Project Overview

A **simple E-Commerce system** built to demonstrate real-world **Software Engineering practices** — not just feature complexity.

The project covers the full **Software Development Lifecycle (SDLC)**:

- ✅ Requirements Engineering (SRS)
- ✅ System Design (UML Diagrams)
- ✅ Clean Architecture & Design Patterns
- ✅ Implementation with SOLID Principles
- ✅ Unit Testing & Quality Assurance
- ✅ Agile Scrum Simulation
- ✅ Version Control (Git)

---

## 🧱 System Architecture

The system follows **Clean Onion Architecture** with strict separation of concerns:

```
ECommerceSolution
│
├── ECommerce.Domain          → Core entities & domain interfaces (no dependencies)
├── ECommerce.Application     → Business logic, use cases & service interfaces
├── ECommerce.Infrastructure  → Database, repositories & external services
├── ECommerce.API             → REST API Backend (Controllers)
├── ECommerce.Web             → MVC Frontend (consumes API via HTTP)
└── ECommerce.Tests           → Unit & Integration Tests (Services, Controllers, Helpers)
```

### System Flow

```
User (Browser)
      ↓
  MVC Frontend
      ↓  HTTP Requests
  REST API
      ↓
  Application Layer (Services)
      ↓
  Infrastructure Layer (Repositories)
      ↓
  SQL Server Database
```

### Architecture Rules

| Rule | Details |
|------|---------|
| Domain | No external dependencies |
| MVC ↔ API | HTTP only — no direct service calls |
| Database access | Infrastructure layer only |
| Business logic | Services only — never in Controllers |

---

## 🚀 Features

### 👤 User
- Register & Login (JWT Authentication)
- Browse Products
- Add to Cart
- Checkout with Payment

### 🛠️ Admin
- Add / Delete Products

---

## 🧠 Design Patterns

| Pattern | Usage |
|---------|-------|
| **Repository Pattern** | Abstracts data access from business logic |
| **Strategy Pattern** | Interchangeable payment method implementations |
| **Factory Pattern** | Centralized object creation |

---

## 🧩 Technologies

| Area | Technology |
|------|-----------|
| Backend API | ASP.NET Core Web API |
| Frontend | ASP.NET Core MVC |
| ORM | Entity Framework Core (Code First) |
| Database | SQL Server |
| API Docs | Swagger / OpenAPI |
| Testing | xUnit · Moq · FluentAssertions |

---

## 🗄️ Database Entities

Built using **EF Core Code First** migrations:

- `User` — authentication & profile
- `Product` — catalog
- `Cart` / `CartItem` — shopping cart
- `Order` / `OrderItem` — checkout & payment
- `Payment` — payment status tracking

---

## ⚙️ Setup Instructions

### 1. Clone

```bash
git clone https://github.com/Ahmed-Al-Desouki/e-commerce-API-MVC-.git
cd e-commerce-API-MVC-
```

### 2. Open Solution

```
ECommerceSolution.sln  →  Visual Studio
```

### 3. Configure Database

`ECommerce.API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=ECommerceDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 4. Apply Migrations

```powershell
Add-Migration InitialCreate
Update-Database
```

### 5. Run

> Start **both** projects (API + Web) to allow MVC to communicate with the API.

```
ECommerce.API   →  https://localhost:{port}/swagger
ECommerce.Web   →  https://localhost:{port}
```

---

## 🧪 Testing

The project includes a comprehensive unit test suite under **`ECommerce.Tests`**.

### Tools

| Tool | Role |
|------|------|
| **xUnit** | Test framework — discovery & execution |
| **Moq** | Mocking library — isolates all external dependencies |
| **FluentAssertions** | Readable, expressive assertion syntax |

### Coverage Summary

| Test Suite | Cases |
|------------|------:|
| ProductServiceTests | 13 |
| AuthServiceTests | 5 |
| CartServiceTests | 9 |
| ProductImageStoreTests | 4 |
| OrderServiceTests | 13 |
| CartControllerTests | 5 |
| ProductsControllerTests | 8 |
| AuthControllerTests | 4 |
| OrderControllerTests | 5 |
| ClaimsHelperTests | 2 |
| **Total** | **68** |

### Approach

- **AAA pattern** (Arrange-Act-Assert) used consistently
- All dependencies mocked — tests are fully **isolated**
- **Theory tests** with `InlineData` for boundary & edge cases
- Exception scenarios validated with correct exception types
- HTTP response codes verified in Controller tests
- `Moq.Verify()` used to assert repository interaction counts

### What is Tested

- Happy paths — correct data returns expected results
- Null / missing inputs — proper exceptions thrown
- Invalid values — negative price/stock, empty name, id ≤ 0
- Business rules — stock validation, duplicate email, expired payment
- HTTP responses — 200, 201, 400, 401, 404, 409

---

## 🔄 Agile Process (Scrum Simulation)

| Sprint | Goal | Deliverable |
|--------|------|-------------|
| Sprint 1 | Requirements Gathering | SRS Document |
| Sprint 2 | System Design | UML Diagrams + Architecture |
| Sprint 3 | Implementation | Working Codebase |
| Sprint 4 | Testing & Documentation | Test Cases + README |

---

## 🔧 Git Workflow

- `main` — stable, production-ready code
- Feature branches for each module
- Pull Requests (simulated) for code integration

---

## 📚 Documentation

| Document | Description |
|----------|-------------|
| SRS | Software Requirements Specification |
| UML – Use Case | System actors & interactions |
| UML – Class Diagram | Domain model & relationships |
| UML – Sequence | Request flows across layers |
| Swagger | Live API documentation at `/swagger` |
| Test Cases PDF | Structured unit test documentation |

---

## 💡 Key Highlights

- Clean, layered architecture that is easy to extend
- API-first design — MVC is purely a consumer
- Real-world patterns applied consistently
- Full test suite with 68 passing unit tests
- Simulated complete SDLC from requirements to delivery

---

## 👨‍💻 Author

**Ahmed Al-Desouki**
🔗 [GitHub](https://github.com/Ahmed-Al-Desouki/e-commerce-API-MVC-)

---

<div align="center">

⭐ **If you found this project useful, please star the repository!**

</div>
