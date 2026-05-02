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

### 1. Clone Repository

````
git clone <your-repo-url>
``` id="c6n9bw"

---

### 2. Open Solution

Open `ECommerceSolution.sln` in Visual Studio

---

### 3. Configure Database

Update connection string in:

````

ECommerce.API/appsettings.json

````id="4n39qr"

Example:

```json id="3y5x9t"
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=ECommerceDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
````

---

### 4. Apply Migrations

Open Package Manager Console:

````
Add-Migration InitialCreate
Update-Database
``` id="tfp8r9"

---

### 5. Run the Project

- Run **API Project**
- Run **MVC Project**

---

## 📚 Documentation

The project includes:

- 📄 SRS Document  
- 📊 UML Diagrams  
  - Use Case  
  - Class Diagram  
  - Sequence Diagram  
- 📘 API Documentation (Swagger)  

---

## 🧪 Testing

- Unit tests implemented using **xUnit**
- Focus on Application Layer
- Includes test cases documentation

---

## 🔄 Agile (Scrum Simulation)

The project simulates 4 sprints:

| Sprint | Description |
|------|------------|
| Sprint 1 | Requirements (SRS) |
| Sprint 2 | Design (UML + Architecture) |
| Sprint 3 | Implementation |
| Sprint 4 | Testing & Documentation |

---

## 🔧 Git Workflow

- `main` branch  
- Feature branches  
- Pull Requests (simulated)  

---

## 📊 Evaluation Coverage

This project satisfies:

- ✅ SRS & Requirements  
- ✅ UML Diagrams  
- ✅ Architecture & Design Patterns  
- ✅ Clean Code  
- ✅ Testing  
- ✅ Agile Process  
- ✅ Presentation  

---

## 💡 Key Highlights

- Clean separation of concerns  
- Real-world architecture  
- Scalable and maintainable structure  
- API-first design  
- MVC acts as a client  

---

## 📽️ Presentation

The final presentation includes:

- Problem Definition  
- Proposed Solution  
- System Architecture  
- UML Diagrams  
- Live Demo  

---

## 🏁 Conclusion

This project demonstrates how to build a **clean, maintainable, and scalable system** using modern software engineering practices.

---

## 👨‍💻 Author

- Your Name Here

---

## 📌 Notes

- This project focuses on **structure and practices**, not complexity  
- UI is intentionally simple  
- Architecture is the main highlight  

---

# ⭐ If you found this project useful, feel free to star it!
````
