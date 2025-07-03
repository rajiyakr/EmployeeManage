## About the Project

`EmployeeManage` is a robust ASP.NET Core MVC application designed to streamline the management of employee information and tasks within an organization. It provides a centralized system for HR and team leads to keep track of employee details, assign and monitor tasks, and manage departments efficiently.

This project was developed with a focus on a clean, intuitive user interface and reliable backend functionality, utilizing modern web development practices.

## Features

* **Employee Management:**
    * Create, Read, Update, Delete (CRUD) employee records.
    * Store essential employee details like name, email, phone, department, position, hire date.
    * Optionally upload and display employee profile images.
    * Track employee activity status (IsActive).
* **Task Management:**
    * Assign and manage tasks for employees.
    * Track task details including title, due date, assigned employee, and status (Pending, Incomplete, Completed).
    * Visually highlight task rows based on their status (e.g., warning for pending, danger for incomplete, success for completed).
* **Department Management:**
    * Manage and categorize employees by department.
* **User Authentication & Authorization:**
    * Secure user login and registration powered by ASP.NET Core Identity.
    * Role-based authorization (e.g., Admin, Employee roles) to control access to specific functionalities.
* **Responsive Design:**
    * Built with Bootstrap, ensuring a responsive and consistent user experience across various devices (desktops, tablets, mobile phones).

## Technologies Used

* **Backend:**
    * [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0) 
    * ASP.NET Core MVC
    * Entity Framework Core (for database interaction)
    * ASP.NET Core Identity (for authentication and authorization)
* **Frontend:**
    * HTML5
    * CSS3
    * Bootstrap 5 (for responsive design and UI components)
    * Razor Views
* **Database:**
    * SQL Server 

## Getting Started

To get a local copy up and running, follow these simple steps.

### Prerequisites

* [.NET SDK](https://dotnet.microsoft.com/download) (ensure you have the version specified above)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or your chosen database server)
* [Visual Studio](https://visualstudio.microsoft.com/) (recommended IDE) or [Visual Studio Code](https://code.visualstudio.com/)

### Installation

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/YOUR_GITHUB_USERNAME/EmployeeManage.git](https://github.com/YOUR_GITHUB_USERNAME/EmployeeManage.git)
    cd EmployeeManage
    ```
2.  **Restore NuGet packages:**
    ```bash
    dotnet restore
    ```
3.  **Update `appsettings.json`:**
    * Open the `appsettings.json` file in the root of the project.
    * Configure your database connection string:
        ```json
        "ConnectionStrings": {
          "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=EmployeeManageDb;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;"
          // Replace YOUR_SERVER_NAME with your SQL Server instance name (e.g., DESKTOP-ABCDEF\SQLEXPRESS)
          // For production, consider using a more secure connection string.
        }
        ```

## Database Setup

This project uses Entity Framework Core Migrations to manage the database schema.

1.  **Apply Migrations:**
    Open the Package Manager Console in Visual Studio (or use the command line in the project root) and run:
    ```powershell
    Add-Migration InitialCreate # You might have already run this. If so, skip.
    Update-Database
    ```
    This will create the database and all necessary tables (including those for ASP.NET Core Identity).

2.  **Seed Initial Data (Optional but Recommended):**
    If you have any initial user roles or an admin user you'd like to create programmatically on startup, you might implement a database seeder. (If you don't have one, you'll need to register a user via the UI after running the app).

## Usage

After setting up the database and running the application, navigate to `https://localhost:PORT/` (the exact port will be displayed when you run the app).

* **Register a new user:** If this is your first time, you'll need to register an account.
* **Login:** Use your registered credentials to log in.
* **Navigate:** Use the sidebar menu to access "Employee Details," "Departments," and "Reports."
* **Manage Data:** Use the "Create New," "Edit," "Details," and "Delete" links within each section to manage your data.

## User Authentication

The application uses ASP.NET Core Identity for robust user management.

* **Registration:** New users can register via the `/Identity/Account/Register` route.
* **Login:** Users can log in via the `/Identity/Account/Login` route.
* **Roles:** (If you've implemented roles like 'Admin', 'Employee') Describe how roles are assigned or if there's an initial admin user. E.g., "Initially, you might need to manually assign the 'Admin' role to your first registered user via the database or a custom seeding mechanism if not automated."


