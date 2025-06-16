# Remote Learning Academy (RLA)

## 📖 Overview

The Remote Learning Academy (RLA) is a modern, secure .NET Core-based e-learning platform that supports online education for students, professors, and assistants.  
It includes robust features such as secure user authentication, course management, and student profile functionality — built with clean architecture principles.

---

## ✨ Key Features

### 🔐 Authentication & Security

- JWT-based authentication with refresh tokens  
- Role-based registration (Student, Professor, Assistant)  
- Two-Factor Authentication (TOTP via authenticator apps)  
- OAuth integration (e.g., Google)  
- Email confirmation and password reset  

### 🎓 Student Management

- View and update student profiles  
- Enroll in and view enrolled courses  

### 🏗️ Architecture

- Clean architecture with separation of concerns  
- Repository pattern (DbContext used only in repositories)  
- Service layer for business logic  
- DTOs for data transfer  

---

## 🛠️ Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)  
- [SQL Server](https://www.microsoft.com/en-us/sql-server)  
- SMTP Server (e.g., Gmail) for sending emails  
- Google OAuth credentials  
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)  

---

## 🚀 Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/RLA.git

```
```bash
cd RLA
```

---

### 2. Configure Environment

create .env file and put these information in it

```bash
# Database Connection
RLA_DEV_DATABASE=""

# JWT Authentication
RLA_DEV_JWT_SECRET=""
RLA_DEV_JWT_ISSUER=""
RLA_DEV_JWT_AUDIENCE=""
RLA_DEV_JWT_EXPIRATION_MINUTES=

# Email Settings (SMTP)
SMTP_SERVER=""
SMTP_PORT=587
SMTP_USERNAME=""
SMTP_PASSWORD=""
SMTP_EMAIL=""
SMTP_USESSL=""

# App Info.
AppUrl=""
AppName=""

# Admin Login Credentials
ADMIN_USERNAME=
ADMIN_EMAIL=
ADMIN_PASSWORD=""
```
---

### 3. Install Dependencies

Install NuGet packages:

```bash
dotnet restore
```

Required packages:
- Microsoft.AspNetCore.Identity.EntityFrameworkCore  
- Microsoft.EntityFrameworkCore.SqlServer  
- Microsoft.AspNetCore.Authentication.JwtBearer  
- Microsoft.AspNetCore.Authentication.Google  
- OtpNet  
- Microsoft.IdentityModel.Tokens  

---

### 4. Apply Database Migrations

Set up the database using Entity Framework Core:

```bash
dotnet ef migrations add InitialCreate --project RLA.Infrastructure
dotnet ef database update --project RLA.Infrastructure
```

---

### 5. Run the Application

```bash
dotnet run --project RLA.API
```

The API will be available at:  
**https://localhost:5001**

---

## 🧩 Architecture Details

- **Controllers**  
  Handle HTTP requests and delegate operations to the service layer.  
  _Examples_: `AuthController`, `StudentController`

- **Services**  
  Contain business logic and orchestrate operations between controllers and repositories.  
  _Examples_: `AuthService`, `StudentService`

- **Repositories**  
  Handle data access logic, interacting directly with the `ElearningPlatformDbContext`.  
  _Examples_: `StudentRepository`

- **Entities**  
  Represent the structure of database tables using domain models.  
  _Examples_: `ApplicationUser`, `Student`, `Course`

- **DTOs (Data Transfer Objects)**  
  Ensure clean and secure data transfer between layers and over the network.  
  _Examples_: `AuthResponseDto`, `CourseDto`

---

🤝 Contributing
Fork the repository.
Create a feature branch: git checkout -b feature/your-feature
Commit changes: git commit -m "Add your feature"
Push to the branch: git push origin feature/your-feature
Open a Pull Request.

---

📜 License
This project is licensed under the MIT License.

---

📧 Contact
For issues or questions, please open an issue on GitHub or contact the maintainers at [adhamhashem2025@gmail.com].
