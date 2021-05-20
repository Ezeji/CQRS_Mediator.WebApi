# CQRS with Mediator in ASP.NET Core 5.0
This sample project shows the implementation of the CQRS(Command and Query Responsibility Segregation) architecture and Mediator design pattern for a significant improvement on performance of a web service.
The idea of CQRS is to have separate databases(SQL and NOSQL) for the write and read operations, hence for this sample project, MS SQL database(SQL) for write operations and Redis database(NOSQL) for read operations were used.
