---
applyTo: '**/*.sql'

description: 'This file contains instructions for T-SQL development.
  It includes guidelines for database schema design, query optimization, and following SQL coding standards.
  Ensure to follow the practices outlined in this file to maintain code quality and consistency.'
---

# T-SQL Development Instructions

## General

* MUST follow standard T-SQL coding conventions for naming, indentation, and spacing.
* MUST implement robust error handling with TRY-CATCH blocks and meaningful error messages.
* MUST always review AI-generated SQL code for correctness, security, and performance.
* MUST parameterize all queries to prevent SQL injection attacks.
* MUST consider performance, maintainability, and business impact when designing database solutions.

## Database Schema Design

Follow these database design principles for consistency across the project.

* MUST use singular form for all table names (e.g., `Customer`, not `Customers`).
* MUST use singular form for all column names (e.g., `Name`, not `Names`).
* MUST include a primary key column named `Id` in all tables.
* MUST include a `CreatedAt` column to store the creation timestamp in all tables.
* MUST include an `UpdatedAt` column to store the last update timestamp in all tables.

## Constraints and Relationships

* MUST define primary key constraints on all tables.
* MUST provide explicit names for all foreign key constraints.
* MUST define foreign key constraints inline with column definitions.
* MUST use `ON DELETE CASCADE` option for foreign key constraints where appropriate.
* MUST use `ON UPDATE CASCADE` option for foreign key constraints where appropriate.
* MUST ensure foreign key constraints reference the primary key of the parent table.

## Code Style

Follow these T-SQL coding conventions for consistency across the project.

* MUST use UPPERCASE for SQL keywords (`SELECT`, `FROM`, `WHERE`, `INSERT`, `UPDATE`, `DELETE`).
* MUST use consistent indentation (4 spaces) for nested queries and conditional statements.
* MUST qualify column names with table name or alias when using multiple tables.
* MUST organize query clauses consistently: `SELECT`, `FROM`, `JOIN`, `WHERE`, `GROUP BY`, `HAVING`, `ORDER BY`.
* MUST break long queries into multiple lines for improved readability.
* MUST quote identifiers (schema names, table names, column names and alias names) with square brackets `[]` when necessary to avoid conflicts with reserved keywords.
* MUST use meaningful table aliases (e.g., `cust` for Customer, `ord` for Order) instead of single letters.
* MUST align commas consistently (either leading or trailing, but be consistent within the project).
* MUST use proper spacing around operators (`=`, `<>`, `>`, `<`, etc.).

## Query Structure and Performance

* MUST use explicit column names in `SELECT` statements instead of `SELECT *`.
* MUST limit the use of subqueries when `JOIN` operations can be used instead.
* MUST include `TOP` or `OFFSET`/`FETCH` clauses to restrict result sets when appropriate.
* MUST avoid using functions on indexed columns in `WHERE` clauses.
* MUST use appropriate indexing strategies for frequently queried columns.
* MUST use `EXISTS` instead of `IN` when checking for existence with subqueries.
* MUST use `UNION ALL` instead of `UNION` when duplicates are acceptable for better performance.
* MUST consider query execution plans and optimize based on actual performance metrics.
* MUST use Common Table Expressions (CTEs) for complex queries to improve readability.
* MUST implement pagination using `OFFSET` and `FETCH NEXT` for modern T-SQL versions.

## Stored Procedures

### Naming Conventions

* MUST prefix stored procedure names with `usp_` (e.g., `usp_GetCustomerOrders`).
* MUST use PascalCase for stored procedure names.
* MUST use descriptive names that clearly indicate the procedure's purpose.
* MUST use plural nouns when returning multiple records (e.g., `usp_GetProducts`).
* MUST use singular nouns when returning a single record (e.g., `usp_GetProduct`).
* MUST include the entity name in the procedure name for clarity (e.g., `usp_CreateCustomer`, `usp_UpdateOrder`).

### Parameter Handling

* MUST prefix all parameters with `@` symbol.
* MUST use camelCase for parameter names (e.g., `@customerId`, `@orderDate`).
* MUST provide default values for optional parameters.
* MUST validate parameter values before processing.
* MUST document all parameters with descriptive comments.
* MUST arrange parameters consistently (required parameters first, optional parameters later).

### Structure and Documentation

* MUST include a header comment block with procedure description, parameter details, and return values.
* MUST return standardized error codes and meaningful error messages.
* MUST return result sets with consistent column ordering.
* MUST use `OUTPUT` parameters for returning status information when appropriate.
* MUST prefix temporary tables with `tmp_` (e.g., `tmp_CustomerData`).

## Security Best Practices

* MUST parameterize all dynamic queries to prevent SQL injection vulnerabilities.
* MUST use prepared statements when executing dynamic SQL.
* MUST avoid embedding credentials or sensitive information in SQL scripts.
* MUST implement proper error handling without exposing internal system details.
* MUST minimize the use of dynamic SQL within stored procedures.

## Transaction Management

* MUST explicitly begin and commit transactions using `BEGIN TRANSACTION` and `COMMIT`.
* MUST use appropriate isolation levels based on business requirements.
* MUST avoid long-running transactions that could cause table locking issues.
* MUST use batch processing techniques for large data operations.
* MUST include `SET NOCOUNT ON` in stored procedures that modify data.
* MUST implement proper rollback logic in error handling scenarios.

## Comments and Documentation

* MUST include comments to explain complex business logic and query operations.
* MUST document any non-obvious performance optimizations or workarounds.
* MUST provide examples of expected input parameters and result sets in procedure headers.