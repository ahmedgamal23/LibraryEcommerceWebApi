### Project: LibraryEcommerceWeb

##### Description
A fully functional e-commerce web application designed for managing libraries and book purchases. 
Built using ASP.NET Core with a layered architecture, the project ensures scalability, maintainability, and clean separation of concerns.

------------- Architecture --------------------

      Controller 
         | |
          V
      Service
         | |
          V
      UnitOfWork
         | |
          V
      Repository
         | |
          V
  DbContext (Database) 


##### Key Responsibilities
Designed and implemented a 4-tier architecture: Presentation, Application, Domain, and Infrastructure layers, following SOLID principles.
Developed RESTful APIs using ASP.NET Core 8 to handle user authentication, product management, order processing, and reviews.
Created and managed the database using Entity Framework Core 8 with migrations, ensuring robust data modeling and relationships (e.g., One-to-Many and One-to-One).
Implemented Repository and Unit of Work Design Patterns to streamline database operations and enable dependency injection.
Integrated LINQ queries for efficient data retrieval and filtering.
Designed and implemented DTOs and AutoMapper for data abstraction between API and database models.
Wrote unit tests using NUnit, MOQ, and In-Memory Database, achieving 85%+ code coverage.
Developed a role-based authentication and authorization system with Identity to manage user roles (Admin, Vendor, Customer).

##### Technologies Used
Backend: ASP.NET Core API 8, C#, Entity Framework Core 8
Database: SQL Server
Frontend: REST API (JSON), Postman for testing APIs
Testing: NUnit, MOQ, In-Memory Database
Other Tools/Frameworks: Dependency Injection, LINQ, AutoMapper, Serilog (for logging), Swagger (API documentation)

##### Key Features Implemented
1 - E-commerce Features:
  User registration/login with email confirmation and password reset functionality.
  Role-based access for Admins, Vendors, and Customers.
  Order placement with support for payments and item reviews.
  Product and category management for Admins.

2 - Scalable Architecture:
  Layered structure to separate concerns, enabling maintainability and scalability.
  Repository Pattern for consistent database interactions.
  Unit of Work for transactional consistency across operations.
  
3 - Testing & Debugging:
  Validated models with DataAnnotations for ensuring robust input validation.
  Created mock services using MOQ for isolated unit testing.
  Tested relationships (e.g., cascading delete) using an In-Memory Database.


# Samples
![Screenshot (594)](https://github.com/user-attachments/assets/b39e9c2c-f03a-4d3b-89f9-cb58e9363c10)
![Screenshot (595)](https://github.com/user-attachments/assets/42ca7ad4-17d6-4fe1-813c-b5312dd31acb)
![Screenshot (596)](https://github.com/user-attachments/assets/bf73cf7c-cfb5-4bff-acf4-101dbd817f13)
![Screenshot (597)](https://github.com/user-attachments/assets/874ad7c8-51a1-4a2a-9d4c-552c6870d047)



