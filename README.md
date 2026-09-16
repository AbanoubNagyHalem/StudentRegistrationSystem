# 🎓 Student Registration System

A modern academic course registration portal for **Master's students**, built with **.NET 10**, **Blazor WebAssembly**, **MudBlazor**, **Entity Framework Core**, and **SQL Server**.

![.NET](https://img.shields.io/badge/.NET-10.0_LTS-512BD4)
![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-512BD4)
![EF Core](https://img.shields.io/badge/EF_Core-10.0-512BD4)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022+-CC2927)
![License](https://img.shields.io/badge/License-Academic-blue)

---

## 📖 Table of Contents

- [Project Overview](#-project-overview)
- [Features](#-features)
- [Architecture](#-architecture)
- [Technologies](#-technologies)
- [Prerequisites](#-prerequisites)
- [Installation](#-installation)
- [Database Setup](#-database-setup)
- [Running the Application](#-running-the-application)
- [Demo Accounts](#-demo-accounts)
- [Business Rules](#-business-rules)
- [API Documentation](#-api-documentation)
- [Testing](#-testing)
- [Project Structure](#-project-structure)
- [Troubleshooting](#-troubleshooting)
- [FAQ](#-faq)

---

## 📖 Project Overview

The **Student Registration System** is a full-stack web application that allows Master's students to:

- 🔐 Log in with registration number + PIN
- 📊 View a personalized dashboard with academic information
- 📚 Browse available courses filtered by their study plan
- ✅ Select course sections with **real-time validation** for:
  - Prerequisites
  - Capacity limits
  - Schedule conflicts
  - 18 credit-hour limit
- 📝 Confirm or delete semester registration
- 🖨️ Print a clean weekly schedule

An **Admin Dashboard** is included to manage colleges, departments, courses, sections, and registration periods.

---

## ✨ Features

### 🎓 Student Features

| Feature                 | Description                                               |
| ----------------------- | --------------------------------------------------------- |
| **Login**               | Registration number + PIN authentication with JWT         |
| **Dashboard**           | Student profile, current semester, quick actions          |
| **Online Registration** | Browse courses from your study plan                       |
| **Section Selection**   | Choose specific sections with real-time seat availability |
| **Prerequisites Check** | Automatic validation of prerequisite courses              |
| **18-Hour Limit**       | Cannot exceed 18 credit hours per semester                |
| **Conflict Detection**  | Automatic schedule conflict detection                     |
| **Capacity Check**      | Atomic seat reservation to prevent over-enrollment        |
| **Print Schedule**      | Clean print-friendly weekly schedule                      |
| **Delete Registration** | Remove all current semester registrations                 |

### 🔧 Admin Features

| Feature                  | Description                              |
| ------------------------ | ---------------------------------------- |
| **Dashboard**            | Quick stats and shortcuts                |
| **Registration Periods** | Open/close registration per department   |
| **Students Management**  | View students with college/level info    |
| **Courses Management**   | Browse all courses                       |
| **Sections Management**  | View sections with capacity and schedule |

### 🏛️ Academic Structure

- **2 Colleges** (Engineering & Management)
- **11 Departments** (6 Engineering + 5 Management)
- **4 Full Curriculums** (ECE, CPE, Business Administration, Accounting)
- **120 Courses** with prerequisites
- **12 Master's Students** (active/graduated/suspended/dismissed)
- **47 Sections** with lectures + labs

---

## 🏛️ Architecture

The solution follows **Clean Architecture** with clear separation of concerns:

```
StudentRegistrationSystem.sln
├── src/
│   ├── StudentRegistration.Domain/          → Entities, Enums (no dependencies)
│   ├── StudentRegistration.Application/     → DTOs, Services, Interfaces
│   ├── StudentRegistration.Infrastructure/  → EF Core, Migrations, Seed
│   ├── StudentRegistration.Api/             → REST API (ASP.NET Core)
│   └── StudentRegistration.Client/          → Blazor WASM UI
└── tests/
    ├── StudentRegistration.UnitTests/       → 23 unit tests
    └── StudentRegistration.IntegrationTests/ → 5 integration tests
```

### Data Flow

```
┌─────────────────┐
│  Blazor WASM    │  (Browser)
│    Client       │
└────────┬────────┘
         │ HTTP + JWT
         ▼
┌─────────────────┐
│   ASP.NET Core  │  (Web API)
│   Controllers   │
└────────┬────────┘
         │ DI
         ▼
┌─────────────────┐
│   Application   │  (Business Logic)
│    Services     │
└────────┬────────┘
         │ EF Core
         ▼
┌─────────────────┐
│   SQL Server    │  (Database)
└─────────────────┘
```

---

## 🧰 Technologies

| Layer          | Technology                     | Version  |
| -------------- | ------------------------------ | -------- |
| **Runtime**    | .NET                           | 10.0 LTS |
| **API**        | ASP.NET Core Web API           | 10       |
| **UI**         | Blazor WebAssembly             | 10       |
| **UI Library** | MudBlazor                      | 9.8.0    |
| **ORM**        | Entity Framework Core          | 10       |
| **Database**   | SQL Server Express             | 2022+    |
| **Auth**       | JWT Bearer                     | 10       |
| **Hashing**    | BCrypt.Net-Next                | 4.0.3    |
| **Storage**    | Blazored.LocalStorage          | 4.5.0    |
| **Tests**      | xUnit + FluentAssertions + Moq | Latest   |

---

## 📋 Prerequisites

### Required Software

1. **.NET 10 SDK**
   👉 [Download](https://dotnet.microsoft.com/download/dotnet/10.0)

2. **SQL Server Express** (any of these):
   - **LocalDB** (comes with Visual Studio) — RECOMMENDED for first-time users
   - **SQL Server Express** — [Download](https://www.microsoft.com/sql-server/sql-server-downloads)
   - **SQL Server Developer Edition**

3. **Git**
   👉 [Download](https://git-scm.com/downloads)

### Required VS Code Extensions (Recommended)

- C# Dev Kit (Microsoft)
- REST Client (Huachao Mao)
- SQL Server (Microsoft)

### Verify Installation

```bash
dotnet --version       # Should output: 10.0.x
dotnet ef --version    # Should output: Entity Framework Core CLI 10.x
```

If `dotnet-ef` is missing:

```bash
dotnet tool install --global dotnet-ef
```

---

## 🚀 Installation

### 1. Clone the Repository

```bash
git clone <repository-url>
cd StudentRegistrationSystem
```

### 2. Restore NuGet Packages

```bash
dotnet restore StudentRegistrationSystem.sln
```

### 3. Build the Solution

```bash
dotnet build StudentRegistrationSystem.sln
```

**Expected:** `Build succeeded with 0 errors`.

---

## 🗄️ Database Setup

### ⚠️ IMPORTANT: Choose Your SQL Server Type

The connection string is configured in:

`src/StudentRegistration.Api/appsettings.json`

### Option A: Using LocalDB (Easiest)

If you have **Visual Studio** installed, LocalDB is already available. Use this connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=StudentRegistrationDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### Option B: Using SQL Server Express (SQLEXPRESS)

If you have **SQL Server Express** installed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=StudentRegistrationDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### Option C: Using Full SQL Server

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=StudentRegistrationDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### Apply Migrations & Seed Data

**Option 1: Automatic (Recommended)**

Simply run the API. Migrations + seeding run automatically on startup in Development mode:

```bash
dotnet run --project src/StudentRegistration.Api
```

**Option 2: Manual**

```bash
dotnet ef database update --project src/StudentRegistration.Infrastructure --startup-project src/StudentRegistration.Api
```

### Verify Database

```bash
sqlcmd -S ".\SQLEXPRESS" -d StudentRegistrationDb -Q "SELECT COUNT(*) FROM Courses"
```

Expected: `120`

---

## 🏃 Running the Application

### 1. Run the API (Backend)

Open **Terminal 1**:

```bash
dotnet run --project src/StudentRegistration.Api
```

**Expected output:**

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5039
```

Keep this terminal open.

### 2. Run the Blazor Client (Frontend)

Open **Terminal 2**:

```bash
dotnet run --project src/StudentRegistration.Client
```

**Expected output:**

```
Now listening on: http://localhost:5174
```

### 3. Open in Browser

Navigate to: **http://localhost:5174**

---

## 🔑 Demo Accounts

> ⚠️ **These credentials are for local development ONLY.**
> **Never use them in production.**

### Students (Master's Program)

| Registration No. | PIN    | Name           | Status       | Department                     |
| ---------------- | ------ | -------------- | ------------ | ------------------------------ |
| `20260001`       | `1234` | Ahmed Mahmoud  | ✅ Active    | ECE (Term 3)                   |
| `20260002`       | `1234` | Sara Ali       | ✅ Active    | ECE (Term 5) — 27 CH available |
| `20260003`       | `1234` | Mohamed Khaled | ✅ Active    | CPE (Term 3)                   |
| `20260004`       | `1234` | Fatma Hassan   | ✅ Active    | CPE (Term 5)                   |
| `20260005`       | `1234` | Omar Ibrahim   | ✅ Active    | BUA (Term 2)                   |
| `20260006`       | `1234` | Layla Ahmed    | ✅ Active    | BUA (Term 4)                   |
| `20260007`       | `1234` | Youssef Tarek  | ✅ Active    | ACC (Term 2)                   |
| `20260008`       | `1234` | Nour Mostafa   | ✅ Active    | ACC (Term 4)                   |
| `20260009`       | `1234` | Ali Reda       | ✅ Active    | CPE (prereq fail test)         |
| `20260010`       | `1234` | Khaled Sami    | ⚠️ Graduated | ECE                            |
| `20260011`       | `1234` | Hana Yasser    | ⚠️ Suspended | ECE                            |
| `20260012`       | `1234` | Ziad Ayman     | ⚠️ Dismissed | CPE                            |

### Admin

| Username | Password    |
| -------- | ----------- |
| `admin`  | `Admin@123` |

---

## 📜 Business Rules

All rules are **enforced on the backend** — frontend validation is for UX only.

| Rule      | Description                                                                                                          |
| --------- | -------------------------------------------------------------------------------------------------------------------- |
| **BR-01** | Registration only available if `RegistrationPeriod.IsOpen == true` AND current date is within `[StartDate, EndDate]` |
| **BR-02** | Student status must be **Active** (not Graduated / Suspended / Dismissed)                                            |
| **BR-03** | Courses must belong to the student's study plan                                                                      |
| **BR-04** | All prerequisites must be completed                                                                                  |
| **BR-05** | Total credit hours ≤ **18** per semester                                                                             |
| **BR-06** | Section capacity: `enrolled < capacity` (atomic check under serializable transaction)                                |
| **BR-07** | No duplicate enrollment in the same section                                                                          |
| **BR-08** | No schedule conflicts (same day + overlapping times)                                                                 |
| **BR-09** | Registration blocked on ANY validation error                                                                         |
| **BR-10** | Delete registration only affects the **current semester**                                                            |

### Conflict Detection Algorithm

```
Two schedules conflict if:
    A.DayOfWeek == B.DayOfWeek
    AND A.StartTime < B.EndTime
    AND B.StartTime < A.EndTime
```

---

## 📚 API Documentation

### OpenAPI Specification

While the API is running in Development mode:

```
http://localhost:5039/openapi/v1.json
```

### Endpoint Groups

| Group        | Base Route            | Auth      |
| ------------ | --------------------- | --------- |
| Auth         | `/api/auth/*`         | Anonymous |
| Students     | `/api/students/*`     | Student   |
| Registration | `/api/registration/*` | Student   |
| Schedule     | `/api/schedule/*`     | Student   |
| Semesters    | `/api/semesters/*`    | Anonymous |
| Admin        | `/api/admin/*`        | Admin     |

### Test with `.http` Files

Open any file in `api-tests/` folder and click **"Send Request"**:

| File                   | Purpose                 |
| ---------------------- | ----------------------- |
| `01-auth.http`         | Login (student + admin) |
| `02-students.http`     | Profile + Dashboard     |
| `03-registration.http` | Full registration flow  |
| `04-admin.http`        | Admin operations        |

---

## 🧪 Testing

### Run All Tests

```bash
dotnet test StudentRegistrationSystem.sln
```

**Expected:** `Passed! - Failed: 0, Passed: 23, Skipped: 0`

### Unit Tests Breakdown

| Test Class                 | Count | Purpose                                                       |
| -------------------------- | ----- | ------------------------------------------------------------- |
| `AuthServiceTests`         | 6     | Login (student + admin)                                       |
| `RegistrationServiceTests` | 13    | Eligibility, prerequisites, capacity, credit limit, conflicts |
| `ConflictDetectionTests`   | 4     | Interval overlap algorithm                                    |

### Integration Tests

| Test Class      | Count | Purpose                              |
| --------------- | ----- | ------------------------------------ |
| `ApiSmokeTests` | 5     | End-to-end API (auth, authorization) |

---

## 📁 Project Structure

```
StudentRegistrationSystem/
├── README.md
├── StudentRegistrationSystem.sln
├── .gitignore
├── global.json
│
├── api-tests/                       # HTTP test files (REST Client)
│   ├── 01-auth.http
│   ├── 02-students.http
│   ├── 03-registration.http
│   └── 04-admin.http
│
├── docs/                            # Documentation
│   ├── 01-ERD.md
│   ├── 02-ClassDiagram.md
│   └── screenshots/
│
├── src/
│   ├── StudentRegistration.Domain/
│   │   ├── Common/BaseEntity.cs
│   │   ├── Entities/                # 17 entities
│   │   └── Enums/                   # 4 enums
│   │
│   ├── StudentRegistration.Application/
│   │   ├── Common/Exceptions/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   └── Services/                # 5 services
│   │
│   ├── StudentRegistration.Infrastructure/
│   │   ├── Data/
│   │   │   ├── Configurations/      # 16 EF configs
│   │   │   ├── Seed/                # Catalog + Seeder
│   │   │   └── AppDbContext.cs
│   │   └── Migrations/
│   │
│   ├── StudentRegistration.Api/
│   │   ├── Controllers/             # 6 controllers
│   │   ├── Middleware/
│   │   ├── Services/
│   │   ├── Settings/
│   │   └── Program.cs
│   │
│   └── StudentRegistration.Client/
│       ├── Auth/
│       ├── Components/              # 8 components
│       ├── Layout/
│       ├── Models/
│       ├── Pages/                   # 12 pages
│       ├── Services/                # 6 services
│       └── wwwroot/
│
└── tests/
    ├── StudentRegistration.UnitTests/
    └── StudentRegistration.IntegrationTests/
```

---

## 🐛 Troubleshooting

### ❌ `A network-related or instance-specific error occurred` (SQL Server)

**Cause:** Connection string mismatch with your SQL Server setup.

**Solution:**

1. Determine your SQL Server type:
   - **LocalDB** → use `(localdb)\\MSSQLLocalDB`
   - **SQLEXPRESS** → use `.\\SQLEXPRESS`
   - **Full SQL Server** → use `localhost` or `YOUR_MACHINE_NAME`
2. Update `src/StudentRegistration.Api/appsettings.json`
3. Restart the API.

### ❌ `Cannot open database "StudentRegistrationDb"`

**Cause:** Database doesn't exist yet.

**Solution:**

```bash
dotnet ef database update --project src/StudentRegistration.Infrastructure --startup-project src/StudentRegistration.Api
```

Or simply run the API — it auto-migrates.

### ❌ `Login failed for user 'DOMAIN\username'`

**Cause:** Windows Authentication not enabled.

**Solution:**

- Open **SQL Server Management Studio (SSMS)**
- Right-click Server → Properties → Security → **SQL Server and Windows Authentication mode**

### ❌ `CORS policy blocked`

**Cause:** Blazor client port not in CORS allowed list.

**Solution:**

1. Check the client port from `dotnet run` output.
2. Update `src/StudentRegistration.Api/Program.cs` CORS section:

```csharp
policy.WithOrigins(
    "http://localhost:5174",  // ← your client port
    ...
)
```

### ❌ `Self-signed certificate` warning

**Solution:**

```bash
dotnet dev-certs https --trust
```

### ❌ `dotnet: command not found`

**Solution:** Install .NET 10 SDK from https://dotnet.microsoft.com/download

### ❌ Seed data is empty after `dotnet run`

**Cause:** Database already has data (seeder is idempotent).

**Solution:**

```bash
dotnet ef database drop --project src/StudentRegistration.Infrastructure --startup-project src/StudentRegistration.Api --force
dotnet run --project src/StudentRegistration.Api
```

### ❌ `Address already in use` on port 5039 or 5174

**Solution:**

```bash
# Windows
netstat -ano | findstr :5039
taskkill /PID <pid> /F

# Or change the port in launchSettings.json
```

---

## ❓ FAQ

### Q: Do I need Visual Studio?

**A:** No. The project works perfectly with **VS Code + .NET CLI**.

### Q: Which SQL Server should I use?

**A:** **LocalDB** is easiest if you have Visual Studio. Otherwise, install **SQL Server Express**.

### Q: Can I use PostgreSQL or MySQL?

**A:** Not out of the box. You'd need to change the EF Core provider and migrations.

### Q: How do I reset the database?

```bash
dotnet ef database drop --project src/StudentRegistration.Infrastructure --startup-project src/StudentRegistration.Api --force
dotnet run --project src/StudentRegistration.Api
```

### Q: How do I add a new course?

Use the Admin Dashboard, or add it to `CourseCatalog.cs` and re-seed.

### Q: Why 18 credit hours?

It's an explicit requirement from the assignment. To change it, edit `MaxCreditHours` in `RegistrationService.cs`.

### Q: Why is my student not showing any available courses?

Possible reasons:

1. Registration period is closed for their department.
2. Student status is not Active.
3. Student has no study plan assigned.
4. All courses are already completed.

### Q: Where are the JWT secrets stored?

In `appsettings.json` for **development only**. For production, use User Secrets or Environment Variables.

---

## 🔐 Security Notes

- ✅ PINs hashed with **BCrypt**
- ✅ JWT tokens with 8-hour expiry
- ✅ Role-based authorization (Student / Admin)
- ✅ Backend validation (never trust the client)
- ✅ Parameterized queries via EF Core
- ❌ No secrets committed for production
- ❌ No database access from the browser

---

## 📜 License

Academic project — educational use only.

---

## 👨‍💻 Author

**Student Registration System** — Master's Program Academic Project.

---

## ✅ Assignment Checklist

- ✅ Story Board (Screens) — see `docs/`
- ✅ ERD — see `docs/01-ERD.md`
- ✅ Class Diagram — see `docs/02-ClassDiagram.md`
- ✅ .NET Core + Blazor WASM
- ✅ EF Core Code First + SQL Server
- ✅ Entity Framework + LINQ
- ✅ Login (Registration No + PIN)
- ✅ Menu / Dashboard
- ✅ Online Registration
- ✅ Prerequisites validation
- ✅ 18-hour limit
- ✅ Capacity check
- ✅ Instructor + TA display
- ✅ Schedule conflict detection
- ✅ Conflict visualization (red cells + X)
- ✅ Confirm registration
- ✅ Delete registration
- ✅ Print schedule
- ✅ Registration period validation
- ✅ Student status validation
- ✅ Clear error messages
