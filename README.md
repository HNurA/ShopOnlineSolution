# ShopOnline E-Commerce Solution

A modern e-commerce application built with .NET 8, implementing N-Layered Architecture with clean separation of concerns.

## 🏗️ Architecture

This project follows **N-Layered Architecture** principles:

- **Presentation Layer** - API controllers and Web UI
- **Business Layer** - Business logic and validation
- **Data Access Layer** - Repository pattern and Entity Framework
- **Cross-Cutting Concerns** - DTOs and shared models

## 🛠️ Tech Stack

- **.NET 8** - Framework
- **ASP.NET Core Web API** - REST API
- **Blazor Server** - Web UI
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **FluentValidation** - Input validation
- **Serilog** - Logging
- **Memory Cache** - Caching strategy
- **xUnit** - Unit testing

## 📦 Project Structure
├── 📱 ShopOnline.Api/ # Web API<br/>
├── 🌐 ShopOnline.Web/ # Blazor UI<br/>
├── 🧠 ShopOnline.Business/ # Business Logic<br/>
├── 💾 ShopOnline.DataAccess/ # Data Access<br/>
├── 📋 ShopOnline.Models/ # DTOs<br/>
└── 🧪 ShopOnline.Api.Tests/ # Tests<br/>

## 🚀 Quick Start

```bash
# Clone & restore
git clone https://github.com/yourusername/ShopOnlineSolution.git
cd ShopOnlineSolution
dotnet restore

# Update database
dotnet ef database update --project ShopOnline.DataAccess --startup-project ShopOnline.Api

# Run
dotnet run --project ShopOnline.Api
Access: https://localhost:7xxx/swagger

```

🎯 Features<br/>
✅ Product catalog | ✅ Shopping cart | ✅ Input validation<br/>
✅ Caching | ✅ Logging | ✅ Unit testing | ✅ Clean architecture<br/>

📚 API Endpoints<br/>
GET /api/Product - Get products<br/>
POST /api/ShoppingCart/AddItem - Add to cart<br/>
PUT /api/ShoppingCart/UpdateQty - Update quantity<br/>
DELETE /api/ShoppingCart/DeleteItem/{id} - Remove item<br/>

📄 License<br/>
This project is licensed under the MIT License - see the LICENSE file for details.<br/>

