DotNet Ecommerce API
====================

Overview
--------

The DotNet Ecommerce API is a RESTful web service built using ASP.NET Core, Entity Framework Core, and Microsoft Identity. It serves as the backend for an ecommerce application, providing endpoints for managing users, products, orders, and more.

Features
--------

-   User Management: Allows registration, login, and role-based authentication for users.
-   Product Management: Enables CRUD operations for managing products in the ecommerce platform.
-   Order Management: Facilitates order placement, retrieval, and status updates for customers and administrators.
-   Role-Based Authorization: Admins have access to additional functionality compared to regular customers.
-   Swagger Documentation: Includes Swagger UI for easy API exploration and testing.

Technologies Used
-----------------

-   ASP.NET Core: Framework for building cross-platform web applications and services.
-   Entity Framework Core: Object-relational mapping framework for interacting with databases.
-   Microsoft Identity: Provides user authentication and authorization functionality.
-   Swagger: Used for API documentation and interactive testing.
-   SQL Server: Backend database for storing application data.

Getting Started
---------------

To get started with the DotNet Ecommerce API, follow these steps:

1.  Clone the Repository: Clone the repository to your local machine.
2.  Configure the Database: Update the connection string in the `appsettings.json` file to point to your SQL Server instance.
3.  Run Migrations: Execute Entity Framework Core migrations to create the database schema.
4.  Start the API: Run the application and navigate to `/swagger` site to explore the available endpoints.

API Endpoints
-------------

### Users

-   POST /api/register: Register a new user.
-   POST /login: Authenticate and log in a user.
-   POST /refresh
-   GET /confirmEmail
-   POST /resendConfirmationEmail
-   POST /forgotPassword
-   POST /resetPassword
-   POST /manage/2fa
-   GET /manage/info
-   POST /manage/info

### Products

-   GET /api/products: Retrieve a list of products.
-   GET /api/products/{id}: Retrieve details of a specific product.
-   POST /api/products: Create a new product.
-   PUT /api/products/{id}: Update an existing product.
-   DELETE /api/products/{id}: Delete a product.

### Orders

-   GET /api/orders: Retrieve a list of orders.
-   GET /api/orders/{id}: Retrieve details of a specific order.
-   POST /api/orders: Place a new order.
-   PUT /api/orders/{id}: Update the status of an existing order.
-   DELETE /api/orders/{id}: Delete an order.

Dependencies
------------

-   Microsoft.AspNetCore.Identity.EntityFrameworkCore: Provides Identity API functionality with Entity Framework support.
-   Microsoft.EntityFrameworkCore.SqlServer: EF Core database provider for SQL Server.
-   Swashbuckle.AspNetCore: Used to generate Swagger documentation and UI.

Contributors
------------

-   M Salman Haider
