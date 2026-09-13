A high-performance, scalable E-Commerce RESTful Web API engineered with
\*\*.NET 8\*\*, 
\*\*Entity Framework Core\*\*,
and \*\*SQL Server\*\*.
Built following
\*\*Clean Architecture\*\* and \*\*Vertical Slice Architecture\*\* 
principles using the \*\*CQRS (Command Query Responsibility Segregation)\*\* pattern with \*\*MediatR\*\*. --- ##  
Key Features \* \*\*Vertical Slice / CQRS Pattern\*\*:
Features organized by business capabilities using \*\*MediatR\*\* 
for complete separation of concerns and maintainability.
\* \*\*MediatR Pipeline Behaviors\*\*:
Centralized cross-cutting concerns: \* \`ValidationBehavior\`: Automatic payload validation via \*\*FluentValidation\*\*.
\* \`LoggingBehavior\` \`PerformanceBehavior\`: 
Structured request logging and detection of long-running operations (&gt;3000ms). 
\* \`TransactionBehavior\`: Automated ACID database transaction lifecycle management (\`Commit\`/\`Rollback\`) for command execution.
\* \*\*Concurrency 
Race Condition Safety\*\*: Atomic SQL updates (\`ExecuteUpdateAsync\`) for product stock management during high-traffic checkout flows. 
\* \*\*Optimized Database Access\*\*: 
High-performance querying using \`.AsNoTracking()\`, server-side projection, sargable prefix searches, and single-roundtrip pagination.
\* \*\*Security
Authentication\*\*: 
\* \*\*JWT (JSON Web Token)\*\* authentication with Role-Based Access Control (RBAC).
\* Rate limiting policies (\`FixedWindowRateLimiter\`) on sensitive auth endpoints to prevent brute-force attacks.
\* \*\*Global Exception Handling\*\*:
Custom \`ApiException\` framework integrated with ASP.NET Core \`ProblemDetails\` middleware for RFC 7807 compliant error responses.

