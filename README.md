# 🛒 Amazon Clone - Full Stack E-Commerce Platform

A production-grade, full-stack e-commerce web application inspired by Amazon. Built with **ASP.NET Core Web API (.NET 9)** on the backend and **Angular 19** with **Signals** on the frontend. Designed with a modern, high-performance architecture, clean code principles, and rich, responsive aesthetics.

---

## 🚀 Key Features

### 🔐 Authentication & Account Management
* **JWT Authentication**: Secure token-based access with refresh token lifecycle.
* **Social Authentication**: Google OAuth 2.0 integration.
* **Email Verification**: Identity confirmation flows.
* **User Profile & Address Book**: Full CRUD operations for managing multiple delivery addresses with default selection.

### 🛍️ E-Commerce Catalog & PDP
* **High-Performance Search & Autocomplete**: Powered by SQL Server stored procedures and Dapper for ultra-fast query execution.
* **Product Detail Page (PDP)**: Dynamic variant selectors, image gallery, stock indicators, and specs.
* **Categories & Brands**: Multi-level hierarchical category browsing and brand filtering.

### 🛒 Shopping Cart & Wishlist
* **Reactive State Management**: Driven by Angular Signals (`signalStore` / reactive stores) for instantaneous UI updates without full page reloads.
* **Cart Persistence & Summary**: Real-time tax, shipping, and discount calculations.
* **Wishlist**: Quick toggle to save favorite items.

---

## 🛠️ Tech Stack & Architecture

### Backend (.NET 9 Web API)
* **Framework**: ASP.NET Core Web API (.NET 9)
* **Architecture**: Clean Architecture / Domain-Driven Layers (API, Application, Domain, Infrastructure)
* **Data Access**: 
  * Entity Framework Core (OR/M for identity & complex entities)
  * Dapper (Micro-ORM for high-performance search & procedure execution)
* **Database**: SQL Server
* **Caching & Performance**: Redis Caching
* **Logging & Observability**: Structured logging with Serilog & EF Core Audit Logging
* **Security & Resilience**: ASP.NET Core Identity, Rate Limiting Middleware, JWT validation

### Frontend (Angular 19)
* **Framework**: Angular 19 (Standalone Components, Signals API)
* **State Management**: Angular Signals & Custom Signal Stores
* **Styling**: Modern Custom SCSS (Design Tokens, Glassmorphism, Micro-animations, Dark/Light palettes)
* **HTTP & Routing**: Functional interceptors (JWT attach, error handling) & Protected Auth Guards

---

## 📁 Repository Structure

```
AmazonClone/
├── AmazonClone/                 # ASP.NET Core API Controllers & Middleware
├── AmazonClone.Application/     # Application Services, DTOs, & Interfaces
├── AmazonClone.Infrastructure/  # EF Core DbContext, Repositories, Services & Migrations
├── Class Library (.NET)/        # Core Domain Entities & Constants
├── AmazonClone.Frontend/        # Angular 19 SPA (Components, Signals, Stores, SCSS)
└── AmazonClone.slnx             # Solution File
```

---

## ⚙️ Getting Started

### Prerequisites
* [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* [Node.js](https://nodejs.org/) (v18 or higher)
* [SQL Server](https://www.microsoft.com/sql-server/) (LocalDB or Express)
* [Angular CLI](https://angular.dev/) (`npm install -g @angular/cli`)

---

### 1. Backend Setup

1. **Navigate to the API folder:**
   ```bash
   cd AmazonClone
   ```
2. **Update Connection String:**
   Ensure `appsettings.json` points to your local SQL Server instance.
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=AmazonCloneDB;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True"
   }
   ```
3. **Start the API server:**
   ```bash
   dotnet run
   ```
   *The API will start at `http://localhost:5000` (or configured HTTPS port).*

---

### 2. Frontend Setup

1. **Navigate to the Frontend directory:**
   ```bash
   cd AmazonClone.Frontend
   ```
2. **Install dependencies:**
   ```bash
   npm install
   ```
3. **Run the Angular Development Server:**
   ```bash
   ng serve
   ```
4. **Open Application:**
   Navigate to `http://localhost:4200` in your web browser.

---

## 📄 License
This project is open-source and available under the [MIT License](LICENSE).
