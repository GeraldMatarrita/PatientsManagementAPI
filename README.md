# PatientsManagementAPI

PatientsManagementAPI is a RESTful API built with ASP.NET Core 8.0 for managing patients, doctors, and medical histories. It uses Oracle Database Express Edition(XE) as the database, JWT authentication for security, and Docker for containerized deployment. The API includes centralized error handling, pagination, filtering, and input sanitization.

## Technologies Used
- ASP.NET Core 8.0
- Entity Framework Core
- Oracle Database Express Edition 21c
- Docker & Docker Compose
- JWT Authentication
- AutoMapper
- xUnit (for unit tests)

## Project Structure

| Directory/File | Description |
|----------------|-------------|
| `Application/` | DTOs and AutoMapper profiles for data mapping |
| `Controllers/` | API controllers (Patients, Doctors, MedicalHistories, Auth) |
| `Domain/` | Entity models representing database tables |
| `Infrastructure/` | Database context and repository implementations |
| `Middleware/` | Custom middleware for centralized error handling |
| `GestionPacientesApi.Tests/` | Unit tests for API functionality |
| `Dockerfile` | Docker configuration for the API |
| `docker-compose.yml` | Docker Compose for API and Oracle XE containers |
| `appsettings.json` | Non-sensitive configuration settings |
| `appsettings.secrets.json` | Sensitive configuration (ignored by `.gitignore`) |
| `.dockerignore` | Files ignored by Docker during build |
| `.gitignore` | Files ignored by Git for version control |

## Setup Instructions

### Local Development (Without Docker)

1. **Clone the Repository**
   git clone [<repository-url>](https://github.com/GeraldMatarrita/GestionPacientesApi.git)
   cd GestionPacientesApi

2. **Configure Sensitive Settings**  
   Create an appsettings.secrets.json file in the project root:
   ```
   {
     "ConnectionStrings": {
       "DefaultConnection": "User Id=admin1;Password=admin1234;Data Source=localhost:1522/XE"
     },
     "Jwt": {
       "Key": "mysuperlongsupersecretkey_1234567890!!",
       "Issuer": "PatientManagementApi",
       "Audience": "PatientManagementApi"
     }
   }
   ```
   Ensure appsettings.secrets.json is listed in .gitignore.

3. **Set Up Oracle Database**  
   Install Oracle Database Express Edition 21c locally or use a running Oracle instance.  
   Connect to the database using SQL Developer:
   - Host: localhost
   - Port: 1522
   - Service: XE
   - User: SYS
   - Password: admin1234
   - Role: SYSDBA

4. **Create the user `admin1`** 
   ``` 
   ALTER SESSION SET "_ORACLE_SCRIPT"=true;
   CREATE USER admin1 IDENTIFIED BY admin1234;
   GRANT CONNECT, RESOURCE, UNLIMITED TABLESPACE TO admin1;
   ```

5. **Apply Database Migrations**
   ```
   dotnet ef database update
   ```
6. **Run the API**
   ```
   dotnet run
   ```
   The API will be available at http://localhost:8080/swagger (or the configured port).

### Docker Deployment

1. **Clone the Repository**
   ```
   git clone [<repository-url>](https://github.com/GeraldMatarrita/GestionPacientesApi.git)
   ```
   cd GestionPacientesApi

2. Configure Environment Variables
   Create a .env file in the project root:
   ```
   DB_USER=admin1
   DB_PASSWORD=admin1234
   DB_DATA_SOURCE=oracledb:1521/XE
   JWT_KEY=mysuperlongsupersecretkey_1234567890!!
   JWT_ISSUER=PatientManagementApi
   JWT_AUDIENCE=PatientManagementApi
   ORACLE_PWD=admin1234
   ```
   Ensure .env is listed in .gitignore.

4. **Log in to Oracle Container Registry**
   ```
   docker login container-registry.oracle.com
   ```

5. **Start the Containers**
   ```
   docker-compose up --build
   ```
   The API will be available at http://localhost:8080/swagger.  
   The Oracle database is accessible at localhost:1522/XE.

6. **Set Up the Database in Container**  
   Connect to the Oracle container using SQL Developer:
   - Host: localhost
   - Port: 1522
   - Service: XE
   - User: SYS
   - Password: admin1234
   - Role: SYSDBA

7. **Create the user `admin1`**
   ```
   ALTER SESSION SET "_ORACLE_SCRIPT"=true;
   CREATE USER admin1 IDENTIFIED BY admin1234;
   GRANT CONNECT, RESOURCE, UNLIMITED TABLESPACE TO admin1;
   ```

8. **Apply Migrations (run locally)**
   ```
   dotnet ef database update
   ```

9. **Stop the Containers**
   ```
   docker-compose down
   ```

### API Usage

#### Authentication
Use POST /api/Auth/login to obtain a JWT token:
```
{
  "username": "admin",
  "password": "password123"
}
```

Include the token in the Authorization header for protected endpoints: Bearer <token>.

#### Endpoints (All they need Authorization)

**Patients:**
- GET /api/Patients?pageNumber=1&pageSize=5 - List patients with pagination and filtering.
- GET /api/Patients/{id} - Get a patient by ID.
- POST /api/Patients - Create a patient.
- PUT /api/Patients/{id} - Update a patient.
- DELETE /api/Patients/{id} - Delete a patient.

**Doctors:**
- GET /api/Doctors?pageNumber=1&pageSize=5 - List doctors with pagination and filtering.
- GET /api/Doctors/{id} - Get a doctor by ID.
- POST /api/Doctors - Create a doctor.
- PUT /api/Doctors/{id} - Update a doctor.
- DELETE /api/Doctors/{id} - Delete a doctor.

**Medical Histories:**
- GET /api/MedicalHistories?pageNumber=1&pageSize=5 - List medical histories with pagination and filtering.
- GET /api/MedicalHistories/{id} - Get a medical history by ID.
- POST /api/MedicalHistories - Create a medical history.
- PUT /api/MedicalHistories/{id} - Update a medical history.
- DELETE /api/MedicalHistories/{id} - Delete a medical history.

#### Error Handling
Errors return a consistent JSON response:
```
{
  "statusCode": 404,
  "message": "Patient not found.",
  "errors": null
}
```

#### Testing
- Use Postman or Swagger (/swagger) to test endpoints.  
- Run unit tests:
  ```
   dotnet test
  ```
### Troubleshooting

- Slow Oracle Startup: Oracle XE may take 10-20 minutes to initialize the first time. Check logs with docker-compose logs oracledb.
- Connection Errors: Ensure the user admin1 is created and the connection string is correct.  
- Port Conflicts: If ports 8080 or 1522 are in use, change them in docker-compose.yml.  

### Security Notes

- Sensitive Data: Credentials are stored in `appsettings.secrets.json` (ignored by `.gitignore`) and `.env` (for Docker).
- JWT Key: Ensure the Jwt:Key is a strong, unique value in production.  
- HTTPS: For production, enable HTTPS by uncommenting app.UseHttpsRedirection() in Program.cs and configuring a certificate.

### License
This project is for demonstration purposes and is not licensed for production use.

