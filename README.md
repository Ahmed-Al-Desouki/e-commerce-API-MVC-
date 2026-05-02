# 🛒 E-Commerce System

## Clean Onion Architecture (.NET + Web API + MVC + SQL Server)

---

## 📌 Project Overview

This project is a **simple E-commerce system** built to demonstrate **real-world Software Engineering practices** rather than complexity.

It applies:

* Clean Onion Architecture
* SOLID Principles
* Design Patterns
* Clean Code Practices
* Agile (Scrum Simulation)
* Entity Framework Core (Code First)
* SQL Server
* API + MVC (Separated Architecture)

---

## 🎯 Project Objectives

The main goal of this project is to simulate a **real software development lifecycle (SDLC)** and apply:

* Requirements Engineering (SRS)
* System Design (UML Diagrams)
* Architecture & Design Patterns
* Implementation using Clean Architecture
* Testing & Quality Assurance
* Version Control (Git)
* Final Presentation

---

## 🧱 System Architecture

The system follows **Clean Onion Architecture** with clear separation of concerns:

```
ECommerceSolution
│
├── ECommerce.Domain          → Core entities & interfaces
├── ECommerce.Application     → Business logic & use cases
├── ECommerce.Infrastructure  → Database & external services
├── ECommerce.API             → Backend (REST API)
└── ECommerce.Web             → Frontend (MVC)
```

---

## 🔄 System Flow

```
User (Browser)
   ↓
MVC (Frontend)
   ↓ HTTP Requests
API (Backend)
   ↓
Application Layer
   ↓
Infrastructure Layer
   ↓
SQL Server Database
```

---

## ⚠️ Architecture Rules

* Domain has **no dependencies**
* MVC communicates with API via **HTTP only**
* No direct DB access outside Infrastructure
* No business logic inside Controllers

---

## 🚀 Features

### 👤 User

* Register & Login
* View Products
* Add to Cart
* Checkout

### 🛠️ Admin

* Add Product
* Delete Product

---

## 🧠 Design Patterns Used

* **Repository Pattern** → Data access abstraction
* **Strategy Pattern** → Payment methods
* **Factory Pattern** → Object creation

---

## 🧩 Technologies Used

* ASP.NET Core Web API
* ASP.NET Core MVC
* Entity Framework Core (Code First)
* SQL Server
* Swagger (API Documentation)
* xUnit (Testing)

---

## 🗄️ Database

* Built using **Code First approach**
* Managed via **EF Core Migrations**
* Includes entities:

  * User
  * Product
  * Cart
  * Order

---

## ⚙️ Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/Ahmed-Al-Desouki/e-commerce-API-MVC-.git
cd e-commerce-API-MVC-
```

---

### 2. Open the Solution

Open the solution file in Visual Studio:

```
ECommerceSolution.sln
```

---

### 3. Configure the Database

Update the connection string in:

```
ECommerce.API/appsettings.json
```

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=ECommerceDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

### 4. Apply Migrations (Code First)

Open **Package Manager Console** and run:

```powershell
Add-Migration InitialCreate
Update-Database
```

---

### 5. Run the Application

* Set **ECommerce.API** as Startup Project and run it
* Set **ECommerce.Web** as Startup Project and run it

> Make sure both projects are running to allow MVC to communicate with the API.

---

## 📚 Documentation

This project includes:

* 📄 Software Requirements Specification (SRS)
* 📊 UML Diagrams

  * Use Case Diagram
  * Class Diagram
  * Sequence Diagram
* 📘 API Documentation via Swagger

---

## 🧪 Testing

* Unit tests implemented using **xUnit**
* Focus on testing the **Application Layer**
* Includes structured test cases documentation

---

## 🔄 Agile (Scrum Simulation)

This project simulates an Agile workflow using 4 sprints:

| Sprint   | Description                        |
| -------- | ---------------------------------- |
| Sprint 1 | Requirements Gathering (SRS)       |
| Sprint 2 | System Design (UML + Architecture) |
| Sprint 3 | Implementation                     |
| Sprint 4 | Testing & Documentation            |

---

## 🔧 Git Workflow

The project follows a simplified Git workflow:

* `main` branch for stable code
* Feature branches for development
* Pull Requests (simulated) for integration

---

## 📊 Evaluation Coverage

This project satisfies all required evaluation criteria:

* ✅ SRS & Requirements
* ✅ UML Diagrams
* ✅ Architecture & Design Patterns
* ✅ Clean Code Practices
* ✅ Testing & QA
* ✅ Agile Process
* ✅ Final Presentation

---

## 💡 Key Highlights

* Clear separation of concerns using Clean Architecture
* Real-world layered design
* Scalable and maintainable structure
* API-first approach
* MVC acts as a client consuming the API via HTTP

---

## 📽️ Presentation

The final presentation includes:

* Problem Definition
* Proposed Solution
* System Architecture
* UML Diagrams
* Live System Demo

---

## 🏁 Conclusion

This project demonstrates how to build a **clean, scalable, and maintainable system** using modern software engineering principles and best practices.

---

## 👨‍💻 Author

* Ahmed Al-Desouki

---

## 📌 Notes

* The focus is on **architecture and best practices**, not feature complexity
* The UI is intentionally simple
* The system is designed to be easily extendable

---

## ⭐ Support

If you found this project useful, feel free to ⭐ star the repository:
👉 https://github.com/Ahmed-Al-Desouki/e-commerce-API-MVC-.git

---

# ⭐ If you found this project useful, feel free to star it!
````
