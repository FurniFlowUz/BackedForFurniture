# Contract CreateContract Refactoring Test Suite
# Tests both EXISTING and NEW customer flows

$baseUrl = "http://localhost:5000/api"
$Global:ErrorCount = 0
$Global:SuccessCount = 0

function Write-TestHeader($title) {
    Write-Host "`n============================================" -ForegroundColor Cyan
    Write-Host "  $title" -ForegroundColor Cyan
    Write-Host "============================================`n" -ForegroundColor Cyan
}

function Write-Success($message) {
    Write-Host "  ✓ $message" -ForegroundColor Green
    $Global:SuccessCount++
}

function Write-Failure($message) {
    Write-Host "  ✗ $message" -ForegroundColor Red
    $Global:ErrorCount++
}

function Write-Info($message) {
    Write-Host "  → $message" -ForegroundColor Yellow
}

# ============================================================================
# TEST 1: INVALID INPUT - No CustomerId, No NewCustomer
# ============================================================================
Write-TestHeader "TEST 1: INVALID - No CustomerId, No NewCustomer"

$body = @{
    categoryIds = @(1)
    totalAmount = 5000.00
    advancePaymentAmount = 1500.00
    productionDurationDays = 30
    signedDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/Contracts" -Method POST -Body $body -ContentType "application/json" -ErrorAction Stop
    Write-Failure "Expected 400 Bad Request but got success"
} catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Success "Returned 400 Bad Request (Expected)"
        $errorBody = $_.ErrorDetails.Message | ConvertFrom-Json
        Write-Info "Error: $($errorBody.message)"
    } else {
        Write-Failure "Expected 400 but got $($_.Exception.Response.StatusCode)"
    }
}

# ============================================================================
# TEST 2: INVALID INPUT - CustomerId = 0
# ============================================================================
Write-TestHeader "TEST 2: INVALID - CustomerId = 0 (Old bug scenario)"

$body = @{
    customerId = 0
    categoryIds = @(1)
    totalAmount = 5000.00
    advancePaymentAmount = 1500.00
    productionDurationDays = 30
    signedDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/Contracts" -Method POST -Body $body -ContentType "application/json" -ErrorAction Stop
    Write-Failure "Expected 400 Bad Request but got success"
} catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Success "Returned 400 Bad Request (Expected - No more CustomerId=0 bug!)"
        $errorBody = $_.ErrorDetails.Message | ConvertFrom-Json
        Write-Info "Error: $($errorBody.message)"
    } else {
        Write-Failure "Expected 400 but got $($_.Exception.Response.StatusCode)"
    }
}

# ============================================================================
# TEST 3: INVALID INPUT - Non-existent CustomerId
# ============================================================================
Write-TestHeader "TEST 3: INVALID - Non-existent CustomerId"

$body = @{
    customerId = 99999
    categoryIds = @(1)
    totalAmount = 5000.00
    advancePaymentAmount = 1500.00
    productionDurationDays = 30
    signedDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/Contracts" -Method POST -Body $body -ContentType "application/json" -ErrorAction Stop
    Write-Failure "Expected 404 Not Found but got success"
} catch {
    if ($_.Exception.Response.StatusCode -eq 404) {
        Write-Success "Returned 404 Not Found (Expected)"
        $errorBody = $_.ErrorDetails.Message | ConvertFrom-Json
        Write-Info "Error: $($errorBody.message)"
    } else {
        Write-Failure "Expected 404 but got $($_.Exception.Response.StatusCode)"
    }
}

# ============================================================================
# TEST 4: INVALID INPUT - Both CustomerId AND NewCustomer provided
# ============================================================================
Write-TestHeader "TEST 4: INVALID - Both CustomerId AND NewCustomer"

$body = @{
    customerId = 1
    newCustomer = @{
        fullName = "Test Customer"
        phoneNumber = "+998901234567"
        address = "Test Address"
    }
    categoryIds = @(1)
    totalAmount = 5000.00
    advancePaymentAmount = 1500.00
    productionDurationDays = 30
    signedDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/Contracts" -Method POST -Body $body -ContentType "application/json" -ErrorAction Stop
    Write-Failure "Expected 400 Bad Request but got success"
} catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Success "Returned 400 Bad Request (Expected)"
        $errorBody = $_.ErrorDetails.Message | ConvertFrom-Json
        Write-Info "Error: $($errorBody.message)"
    } else {
        Write-Failure "Expected 400 but got $($_.Exception.Response.StatusCode)"
    }
}

# ============================================================================
# TEST 5: INVALID INPUT - NewCustomer with missing required fields
# ============================================================================
Write-TestHeader "TEST 5: INVALID - NewCustomer missing required fields"

$body = @{
    newCustomer = @{
        fullName = "Test"
    }
    categoryIds = @(1)
    totalAmount = 5000.00
    advancePaymentAmount = 1500.00
    productionDurationDays = 30
    signedDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/Contracts" -Method POST -Body $body -ContentType "application/json" -ErrorAction Stop
    Write-Failure "Expected 400 Bad Request but got success"
} catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Success "Returned 400 Bad Request (Expected)"
        $errorBody = $_.ErrorDetails.Message | ConvertFrom-Json
        Write-Info "Error: $($errorBody.message)"
    } else {
        Write-Failure "Expected 400 but got $($_.Exception.Response.StatusCode)"
    }
}

# ============================================================================
# TEST 6: VALID - Existing CustomerId
# ============================================================================
Write-TestHeader "TEST 6: VALID - Existing Customer Flow"

$body = @{
    customerId = 1
    categoryIds = @(1, 2)
    totalAmount = 8000.00
    advancePaymentAmount = 2400.00
    productionDurationDays = 45
    signedDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    deliveryTerms = "Delivery within 45 days"
    penaltyTerms = "1% per day penalty for delays"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/Contracts" -Method POST -Body $body -ContentType "application/json" -ErrorAction Stop
    Write-Success "Contract created with existing customer!"
    Write-Info "Contract Number: $($response.data.contractNumber)"
    Write-Info "Customer ID: $($response.data.customerId)"
    Write-Info "Total Amount: $($response.data.totalAmount)"
} catch {
    Write-Failure "Failed: $($_.Exception.Message)"
    if ($_.ErrorDetails.Message) {
        $errorBody = $_.ErrorDetails.Message | ConvertFrom-Json
        Write-Info "Error: $($errorBody.message)"
    }
}

# ============================================================================
# TEST 7: VALID - New Customer Inline Creation
# ============================================================================
Write-TestHeader "TEST 7: VALID - New Customer Inline Creation"

$randomPhone = "+99890" + (Get-Random -Minimum 1000000 -Maximum 9999999)
$body = @{
    newCustomer = @{
        fullName = "John Doe Test"
        phoneNumber = $randomPhone
        address = "123 Main Street, Tashkent"
        email = "john.doe$(Get-Random)@test.com"
        notes = "Created inline during contract creation"
    }
    categoryIds = @(1)
    totalAmount = 12000.00
    advancePaymentAmount = 3600.00
    productionDurationDays = 60
    signedDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    deliveryTerms = "Express delivery within 60 days"
    penaltyTerms = "2% per day penalty for delays"
    additionalNotes = "High priority customer"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/Contracts" -Method POST -Body $body -ContentType "application/json" -ErrorAction Stop
    Write-Success "Contract created with NEW customer!"
    Write-Info "Contract Number: $($response.data.contractNumber)"
    Write-Info "Customer ID: $($response.data.customerId)"
    Write-Info "Customer Name: John Doe Test"
    Write-Info "Total Amount: $($response.data.totalAmount)"
    Write-Info "Phone: $randomPhone"
} catch {
    Write-Failure "Failed: $($_.Exception.Message)"
    if ($_.ErrorDetails.Message) {
        $errorBody = $_.ErrorDetails.Message | ConvertFrom-Json
        Write-Info "Error: $($errorBody.message)"
    }
}

# ============================================================================
# TEST 8: INVALID - Duplicate Phone Number
# ============================================================================
Write-TestHeader "TEST 8: INVALID - Duplicate Phone Number"

$body = @{
    newCustomer = @{
        fullName = "Jane Smith"
        phoneNumber = $randomPhone  # Use same phone from Test 7
        address = "456 Another St"
    }
    categoryIds = @(1)
    totalAmount = 5000.00
    advancePaymentAmount = 1500.00
    productionDurationDays = 30
    signedDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/Contracts" -Method POST -Body $body -ContentType "application/json" -ErrorAction Stop
    Write-Failure "Expected 400 Bad Request but got success"
} catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Success "Returned 400 Bad Request - Duplicate phone detected! (Expected)"
        $errorBody = $_.ErrorDetails.Message | ConvertFrom-Json
        Write-Info "Error: $($errorBody.message)"
    } else {
        Write-Failure "Expected 400 but got $($_.Exception.Response.StatusCode)"
    }
}

# ============================================================================
# SUMMARY
# ============================================================================
Write-Host "`n============================================" -ForegroundColor White
Write-Host "  TEST SUMMARY" -ForegroundColor White
Write-Host "============================================" -ForegroundColor White
Write-Host "  Total Tests: $($Global:SuccessCount + $Global:ErrorCount)" -ForegroundColor White
Write-Host "  Passed: $Global:SuccessCount" -ForegroundColor Green
Write-Host "  Failed: $Global:ErrorCount" -ForegroundColor Red
Write-Host "============================================`n" -ForegroundColor White

if ($Global:ErrorCount -eq 0) {
    Write-Host "🎉 ALL TESTS PASSED! Contract refactoring is production-ready!" -ForegroundColor Green
} else {
    Write-Host "⚠ Some tests failed. Please review the errors above." -ForegroundColor Yellow
}
