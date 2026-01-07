#!/bin/bash

echo "========================================"
echo "PPM Backend Quick Start Script"
echo "========================================"
echo ""

# Check if .NET 9 is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ ERROR: .NET SDK is not installed or not in PATH"
    echo "Please install .NET 9 SDK from: https://dotnet.microsoft.com/download/dotnet/9.0"
    exit 1
fi

echo "✓ .NET SDK found:"
dotnet --version
echo ""

# Check if we're in the backend directory
if [ ! -f "PPM.sln" ]; then
    echo "❌ ERROR: Please run this script from the backend directory"
    exit 1
fi

echo "Step 1: Restoring NuGet packages..."
if ! dotnet restore; then
    echo "❌ ERROR: Failed to restore packages"
    exit 1
fi
echo "✓ Packages restored successfully"
echo ""

echo "Step 2: Building solution..."
if ! dotnet build; then
    echo "❌ ERROR: Build failed"
    exit 1
fi
echo "✓ Build successful"
echo ""

echo "Step 3: Checking for EF Core tools..."
dotnet tool install --global dotnet-ef &> /dev/null
dotnet tool update --global dotnet-ef &> /dev/null
echo "✓ EF Core tools ready"
echo ""

echo "Step 4: Creating database migration..."
cd PPM.API
dotnet ef migrations add InitialCreate --project ../PPM.Infrastructure --startup-project . --force &> /dev/null
echo "✓ Migration created"
echo ""

echo "Step 5: Updating database..."
echo "This will create the database and seed initial data..."
if ! dotnet ef database update --project ../PPM.Infrastructure --startup-project .; then
    echo ""
    echo "⚠️  WARNING: Database update failed. This might be because:"
    echo "   1. PostgreSQL is not running"
    echo "   2. Connection string in appsettings.json is incorrect"
    echo "   3. Database already exists"
    echo ""
    echo "Please check the connection string and try running manually:"
    echo "   cd PPM.API"
    echo "   dotnet ef database update --project ../PPM.Infrastructure --startup-project ."
    echo ""
else
    echo "✓ Database created and seeded successfully"
    echo ""
fi

echo ""
echo "========================================"
echo "🎉 Setup Complete!"
echo "========================================"
echo ""
echo "To start the API, run:"
echo "   cd PPM.API"
echo "   dotnet run"
echo ""
echo "The API will be available at:"
echo "   - HTTP:  http://localhost:5000"
echo "   - HTTPS: https://localhost:5001"
echo "   - Swagger: https://localhost:5001"
echo ""
echo "Demo Login Credentials:"
echo "   Super Admin: superadmin / Admin@123"
echo "   Owner: owner / Owner@123"
echo "   Manager: manager / Manager@123"
echo "   Worker: ramesh / Worker@123"
echo ""
echo "For more information, see README.md"
echo "========================================"
echo ""
