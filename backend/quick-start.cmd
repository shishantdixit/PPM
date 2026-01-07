@echo off
echo ========================================
echo PPM Backend Quick Start Script
echo ========================================
echo.

REM Check if .NET 9 is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET SDK is not installed or not in PATH
    echo Please install .NET 9 SDK from: https://dotnet.microsoft.com/download/dotnet/9.0
    pause
    exit /b 1
)

echo ✓ .NET SDK found:
dotnet --version
echo.

REM Check if we're in the backend directory
if not exist "PPM.sln" (
    echo ERROR: Please run this script from the backend directory
    pause
    exit /b 1
)

echo Step 1: Restoring NuGet packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)
echo ✓ Packages restored successfully
echo.

echo Step 2: Building solution...
dotnet build
if %errorlevel% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)
echo ✓ Build successful
echo.

echo Step 3: Checking for EF Core tools...
dotnet tool install --global dotnet-ef >nul 2>&1
dotnet tool update --global dotnet-ef >nul 2>&1
echo ✓ EF Core tools ready
echo.

echo Step 4: Creating database migration...
cd PPM.API
dotnet ef migrations add InitialCreate --project ..\PPM.Infrastructure --startup-project . --force >nul 2>&1
echo ✓ Migration created
echo.

echo Step 5: Updating database...
echo This will create the database and seed initial data...
dotnet ef database update --project ..\PPM.Infrastructure --startup-project .
if %errorlevel% neq 0 (
    echo.
    echo WARNING: Database update failed. This might be because:
    echo   1. PostgreSQL is not running
    echo   2. Connection string in appsettings.json is incorrect
    echo   3. Database already exists
    echo.
    echo Please check the connection string and try running manually:
    echo   cd PPM.API
    echo   dotnet ef database update --project ..\PPM.Infrastructure --startup-project .
    echo.
    pause
) else (
    echo ✓ Database created and seeded successfully
    echo.
)

echo.
echo ========================================
echo 🎉 Setup Complete!
echo ========================================
echo.
echo To start the API, run:
echo   cd PPM.API
echo   dotnet run
echo.
echo The API will be available at:
echo   - HTTP:  http://localhost:5000
echo   - HTTPS: https://localhost:5001
echo   - Swagger: https://localhost:5001
echo.
echo Demo Login Credentials:
echo   Super Admin: superadmin / Admin@123
echo   Owner: owner / Owner@123
echo   Manager: manager / Manager@123
echo   Worker: ramesh / Worker@123
echo.
echo For more information, see README.md
echo ========================================
echo.

pause
