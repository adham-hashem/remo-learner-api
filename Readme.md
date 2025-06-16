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

---

## 👨‍🏫 Professor Features

### 📘 Course Management
- View all courses assigned to the professor  
- View course details including:
  - Day of the week
  - Time
  - Location
  - Number of materials, quizzes, and discussions  

### 🎓 Student Management
- View enrolled students in a specific course  
- See each student’s name and final grade for that course  

### 📚 Material Management
- Upload lecture materials to a course  
- Retrieve a specific material by ID  
- List all materials for a course with:
  - Week number  
  - Lecture title and description  
  - File path  

### 📝 Quiz Management
- Create new quizzes for a course  
- Retrieve quiz details by ID  
- List all quizzes in a course with:
  - Title  
  - Max score  
  - Term  
  - Question count  

### 💬 Discussion Management
- Start new discussions in a course  
- Retrieve a specific discussion by ID  
- List all discussions for a course with:
  - Professor’s name  
  - Message  
  - Post date  
  - Author role (professor or not)  

### 🔒 Authorization & Access Control
- All operations require authenticated professor access (via JWT)  
- Identity verified through token `sub` claim  
- Fine-grained access control:
  - Only professors assigned to a course can access or modify its data  

---

## 🏗️ Architecture

### ⚙️ Technical Design
- Clean architecture with separation of concerns  
- Repository pattern: `DbContext` usage restricted to repositories  
- Service layer encapsulates business logic  
- DTOs used for clean and secure data transfer  


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
