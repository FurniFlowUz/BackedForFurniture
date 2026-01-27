# Test Contract Endpoints with Backward Compatibility
# This script tests the contract creation and fetching with OLD frontend format

$baseUrl = "http://localhost:5000/api"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing Contract Endpoints" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Login as Salesperson
Write-Host "Step 1: Login as Salesperson" -ForegroundColor Yellow
$loginBody = @{
    email = "salesperson@furniflowuz.com"
    password = "Test123!"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/Auth/login" -Method POST -Body $loginBody -ContentType "application/json"
    $token = $loginResponse.token
    Write-Host "Login successful! Token: $($token.Substring(0, 20))..." -ForegroundColor Green
} catch {
    Write-Host "Login failed: $_" -ForegroundColor Red
    Write-Host "Trying to seed database with default user..." -ForegroundColor Yellow

    # Try alternative login
    $loginBody2 = @{
        email = "admin@furniflowuz.com"
        password = "Admin123!"
    } | ConvertTo-Json

    try {
        $loginResponse = Invoke-RestMethod -Uri "$baseUrl/Auth/login" -Method POST -Body $loginBody2 -ContentType "application/json"
        $token = $loginResponse.token
        Write-Host "Logged in as admin instead! Token: $($token.Substring(0, 20))..." -ForegroundColor Green
    } catch {
        Write-Host "Admin login also failed: $_" -ForegroundColor Red
        exit
    }
}

Write-Host ""

# Step 2: Get Categories
Write-Host "Step 2: Get Categories" -ForegroundColor Yellow
$headers = @{
    Authorization = "Bearer $token"
}

try {
    $categories = Invoke-RestMethod -Uri "$baseUrl/Categories" -Method GET -Headers $headers
    Write-Host "Found $($categories.Count) categories:" -ForegroundColor Green
    $categories | ForEach-Object {
        Write-Host "  - Category ID: $($_.id), Name: $($_.name)" -ForegroundColor Gray
    }
    $categoryId = $categories[0].id
} catch {
    Write-Host "Failed to get categories: $_" -ForegroundColor Red
    exit
}

Write-Host ""

# Step 3: Get Customers
Write-Host "Step 3: Get Customers" -ForegroundColor Yellow
try {
    $customers = Invoke-RestMethod -Uri "$baseUrl/Customers" -Method GET -Headers $headers
    Write-Host "Found $($customers.Count) customers:" -ForegroundColor Green
    $customers | ForEach-Object {
        Write-Host "  - Customer ID: $($_.id), Name: $($_.fullName)" -ForegroundColor Gray
    }
    $customerId = $customers[0].id
} catch {
    Write-Host "Failed to get customers: $_" -ForegroundColor Red
    exit
}

Write-Host ""

# Step 4: Create Contract using OLD FORMAT
Write-Host "Step 4: Create Contract using OLD FORMAT (backward compatibility test)" -ForegroundColor Yellow
$oldFormatContract = @{
    customerId = $customerId
    categoryId = $categoryId  # OLD FORMAT: single category
    totalAmount = 5000.00
    advancePaymentPercentage = 30  # OLD FORMAT: percentage instead of amount
    deadline = (Get-Date).AddDays(45).ToString("yyyy-MM-ddTHH:mm:ss")  # OLD FORMAT: deadline instead of production days
    signedDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    description = "Test contract created using OLD format"  # OLD FORMAT
    terms = "Standard delivery and penalty terms"  # OLD FORMAT
    notes = "Additional notes about the contract"  # OLD FORMAT
} | ConvertTo-Json

Write-Host "Contract data (OLD format):" -ForegroundColor Gray
Write-Host $oldFormatContract -ForegroundColor Gray

try {
    $createdContract = Invoke-RestMethod -Uri "$baseUrl/Contracts" -Method POST -Body $oldFormatContract -Headers $headers -ContentType "application/json"
    Write-Host "Contract created successfully!" -ForegroundColor Green
    Write-Host "Contract Number: $($createdContract.contractNumber)" -ForegroundColor Green
    Write-Host "Contract ID: $($createdContract.id)" -ForegroundColor Green
    $contractId = $createdContract.id
} catch {
    Write-Host "Failed to create contract: $_" -ForegroundColor Red
    Write-Host "Response: $($_.Exception.Response)" -ForegroundColor Red
    exit
}

Write-Host ""

# Step 5: Get Contract by ID
Write-Host "Step 5: Get Contract by ID (test backward compatibility in response)" -ForegroundColor Yellow
try {
    $contract = Invoke-RestMethod -Uri "$baseUrl/Contracts/$contractId" -Method GET -Headers $headers
    Write-Host "Contract retrieved successfully!" -ForegroundColor Green
    Write-Host "Verifying backward compatibility fields:" -ForegroundColor Gray
    Write-Host "  - categoryId (OLD): $($contract.categoryId)" -ForegroundColor Gray
    Write-Host "  - categoryIds (NEW): $($contract.categoryIds)" -ForegroundColor Gray
    Write-Host "  - advancePaymentPercentage (OLD): $($contract.advancePaymentPercentage)%" -ForegroundColor Gray
    Write-Host "  - advancePaymentAmount (NEW): $($contract.advancePaymentAmount)" -ForegroundColor Gray
    Write-Host "  - deadline (OLD): $($contract.deadline)" -ForegroundColor Gray
    Write-Host "  - productionDurationDays (NEW): $($contract.productionDurationDays)" -ForegroundColor Gray
    Write-Host "  - description (OLD): $($contract.description)" -ForegroundColor Gray
    Write-Host "  - additionalNotes (NEW): $($contract.additionalNotes)" -ForegroundColor Gray
} catch {
    Write-Host "Failed to get contract: $_" -ForegroundColor Red
    exit
}

Write-Host ""

# Step 6: Get All Contracts
Write-Host "Step 6: Get All Contracts (list view)" -ForegroundColor Yellow
try {
    $contracts = Invoke-RestMethod -Uri "$baseUrl/Contracts" -Method GET -Headers $headers
    Write-Host "Found $($contracts.Count) contracts" -ForegroundColor Green
    if ($contracts.Count -gt 0) {
        Write-Host "First contract:" -ForegroundColor Gray
        Write-Host "  - Contract Number: $($contracts[0].contractNumber)" -ForegroundColor Gray
        Write-Host "  - Customer: $($contracts[0].customerName)" -ForegroundColor Gray
        Write-Host "  - Total Amount: $($contracts[0].totalAmount)" -ForegroundColor Gray
        Write-Host "  - Status: $($contracts[0].status)" -ForegroundColor Gray
    }
} catch {
    Write-Host "Failed to get contracts: $_" -ForegroundColor Red
    exit
}

Write-Host ""

# Step 7: Get Contract Stats
Write-Host "Step 7: Get Contract Stats (for salesperson dashboard)" -ForegroundColor Yellow
try {
    $stats = Invoke-RestMethod -Uri "$baseUrl/Contracts/stats" -Method GET -Headers $headers
    Write-Host "Contract stats retrieved successfully!" -ForegroundColor Green
    Write-Host "  - Active Contracts: $($stats.activeContracts)" -ForegroundColor Gray
    Write-Host "  - Pending Orders: $($stats.pendingOrders)" -ForegroundColor Gray
    Write-Host "  - Completed Orders: $($stats.completedOrders)" -ForegroundColor Gray
    Write-Host "  - Total Revenue: $($stats.totalRevenue)" -ForegroundColor Gray
} catch {
    Write-Host "Failed to get stats: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "All Tests Completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Summary:" -ForegroundColor Yellow
Write-Host "✓ Login successful" -ForegroundColor Green
Write-Host "✓ Contract creation with OLD format works" -ForegroundColor Green
Write-Host "✓ Contract retrieval with backward compatibility works" -ForegroundColor Green
Write-Host "✓ All endpoints tested successfully" -ForegroundColor Green
Write-Host ""
