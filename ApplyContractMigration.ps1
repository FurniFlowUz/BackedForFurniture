# Apply Contract Refactoring Migration Script
# This script applies the database migration for the Contract table refactoring

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Contract Refactoring Migration Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Check if API is running
Write-Host "Step 1: Checking if API is running..." -ForegroundColor Yellow
$apiProcess = Get-Process -Name "FurniFlowUz.API" -ErrorAction SilentlyContinue

if ($apiProcess) {
    Write-Host "WARNING: API is currently running!" -ForegroundColor Red
    Write-Host "You need to stop the API before applying the migration." -ForegroundColor Red
    Write-Host ""
    $response = Read-Host "Do you want to stop the API now? (Y/N)"

    if ($response -eq "Y" -or $response -eq "y") {
        Write-Host "Stopping API..." -ForegroundColor Yellow
        Stop-Process -Name "FurniFlowUz.API" -Force
        Start-Sleep -Seconds 2
        Write-Host "API stopped successfully!" -ForegroundColor Green
    } else {
        Write-Host "Migration cancelled. Please stop the API manually and run this script again." -ForegroundColor Red
        exit
    }
} else {
    Write-Host "API is not running. Good to proceed!" -ForegroundColor Green
}

Write-Host ""

# Step 2: Get database connection details
Write-Host "Step 2: Database Connection" -ForegroundColor Yellow
Write-Host "Reading connection string from appsettings.json..." -ForegroundColor Gray

$appsettingsPath = ".\src\FurniFlowUz.API\appsettings.json"
if (Test-Path $appsettingsPath) {
    $appsettings = Get-Content $appsettingsPath | ConvertFrom-Json
    $connectionString = $appsettings.ConnectionStrings.DefaultConnection

    # Extract database name from connection string
    if ($connectionString -match "Database=([^;]+)") {
        $databaseName = $matches[1]
        Write-Host "Database: $databaseName" -ForegroundColor Green
    } else {
        $databaseName = "FurniFlowUz"
        Write-Host "Could not extract database name, using default: $databaseName" -ForegroundColor Yellow
    }
} else {
    Write-Host "appsettings.json not found. Using default database name: FurniFlowUz" -ForegroundColor Yellow
    $databaseName = "FurniFlowUz"
}

Write-Host ""

# Step 3: Apply migration
Write-Host "Step 3: Applying Migration" -ForegroundColor Yellow
Write-Host "This will execute: ContractRefactoring_Migration.sql" -ForegroundColor Gray
Write-Host ""
Write-Host "WARNING: This will modify your database schema!" -ForegroundColor Red
Write-Host "Actions to be performed:" -ForegroundColor Yellow
Write-Host "  1. Add new columns (CategoryIds, ProductionDurationDays, DeliveryTerms, etc.)" -ForegroundColor Gray
Write-Host "  2. Migrate existing data to new columns" -ForegroundColor Gray
Write-Host "  3. Drop old columns (CategoryId, AdvancePaymentPercentage, Deadline, etc.)" -ForegroundColor Gray
Write-Host ""
$response = Read-Host "Do you want to proceed? (Y/N)"

if ($response -ne "Y" -and $response -ne "y") {
    Write-Host "Migration cancelled by user." -ForegroundColor Red
    exit
}

Write-Host ""
Write-Host "Applying migration..." -ForegroundColor Yellow

# Use sqlcmd to apply the migration
$migrationFile = ".\ContractRefactoring_Migration.sql"

if (-not (Test-Path $migrationFile)) {
    Write-Host "ERROR: Migration file not found: $migrationFile" -ForegroundColor Red
    exit
}

try {
    # Try to execute with sqlcmd (using Windows Authentication)
    $output = sqlcmd -S "localhost" -d $databaseName -E -i $migrationFile 2>&1

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Migration applied successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Output:" -ForegroundColor Gray
        Write-Host $output -ForegroundColor Gray
    } else {
        Write-Host "ERROR: Migration failed!" -ForegroundColor Red
        Write-Host "Error details:" -ForegroundColor Red
        Write-Host $output -ForegroundColor Red
        Write-Host ""
        Write-Host "Please check the error and try again." -ForegroundColor Yellow
        exit
    }
} catch {
    Write-Host "ERROR: Failed to execute sqlcmd" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternative: You can apply the migration manually using SQL Server Management Studio" -ForegroundColor Yellow
    Write-Host "File location: $migrationFile" -ForegroundColor Gray
    exit
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Migration completed successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Start the API again" -ForegroundColor Gray
Write-Host "2. Test the contract endpoints" -ForegroundColor Gray
Write-Host ""
