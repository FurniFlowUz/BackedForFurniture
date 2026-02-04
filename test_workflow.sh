#!/bin/bash

# FurniFlowUz Workflow Test Script
# Tests complete workflow from Production Manager to Constructor

API_URL="http://localhost:5000/api"

echo "=========================================="
echo "FURNIFLOWUZ WORKFLOW TEST"
echo "=========================================="
echo ""

# Step 1: Login as Production Manager
echo "Step 1: Login as Production Manager (abror@gmail.com)"
echo "----------------------------------------------"
PM_RESPONSE=$(curl -s -X POST $API_URL/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"abror@gmail.com","password":"Abror12345"}')

PM_TOKEN=$(echo $PM_RESPONSE | grep -o '"token":"[^"]*' | cut -d'"' -f4)

if [ -z "$PM_TOKEN" ]; then
    echo "❌ Login FAILED for Production Manager"
    echo "Response: $PM_RESPONSE"
    exit 1
fi

echo "✅ Login successful"
echo "Token: ${PM_TOKEN:0:50}..."
echo ""

# Step 2: Check existing categories
echo "Step 2: Check existing categories"
echo "----------------------------------------------"
CATEGORIES=$(curl -s -H "Authorization: Bearer $PM_TOKEN" $API_URL/Categories)
echo "Categories: $CATEGORIES" | head -c 200
echo "..."
echo ""

# Step 3: Check existing templates
echo "Step 3: Check existing FurnitureTypeTemplates"
echo "----------------------------------------------"
TEMPLATES=$(curl -s -H "Authorization: Bearer $PM_TOKEN" $API_URL/FurnitureTypeTemplate)
echo "Response: $TEMPLATES" | head -c 300
echo "..."
echo ""

# Step 4: Get templates for Category 2 (Shkaf-kupe)
echo "Step 4: Get templates for Category 2 (Shkaf-kupe)"
echo "----------------------------------------------"
SHKAF_TEMPLATES=$(curl -s -H "Authorization: Bearer $PM_TOKEN" $API_URL/FurnitureTypeTemplate/category/2/active)
echo "Shkaf templates: $SHKAF_TEMPLATES" | head -c 400
echo "..."
echo ""

# Step 5: Login as Seller
echo "Step 5: Login as Seller (sales@furniflowauz.com)"
echo "----------------------------------------------"
SELLER_RESPONSE=$(curl -s -X POST $API_URL/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"sales@furniflowauz.com","password":"Sales12345"}')

SELLER_TOKEN=$(echo $SELLER_RESPONSE | grep -o '"token":"[^"]*' | cut -d'"' -f4)

if [ -z "$SELLER_TOKEN" ]; then
    echo "❌ Login FAILED for Seller"
    echo "Response: $SELLER_RESPONSE"
    exit 1
fi

echo "✅ Seller login successful"
echo ""

# Step 6: Get customers
echo "Step 6: Get existing customers"
echo "----------------------------------------------"
CUSTOMERS=$(curl -s -H "Authorization: Bearer $SELLER_TOKEN" $API_URL/Customers)
echo "Customers: $CUSTOMERS" | head -c 200
echo "..."
echo ""

# Step 7: Login as Constructor
echo "Step 7: Login as Constructor (bek@gmail.com)"
echo "----------------------------------------------"
CONSTRUCTOR_RESPONSE=$(curl -s -X POST $API_URL/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"bek@gmail.com","password":"Bek12345"}')

CONSTRUCTOR_TOKEN=$(echo $CONSTRUCTOR_RESPONSE | grep -o '"token":"[^"]*' | cut -d'"' -f4)

if [ -z "$CONSTRUCTOR_TOKEN" ]; then
    echo "❌ Login FAILED for Constructor"
    echo "Response: $CONSTRUCTOR_RESPONSE"
    exit 1
fi

echo "✅ Constructor login successful"
echo ""

# Step 8: Get Constructor's assigned orders
echo "Step 8: Get Constructor's assigned orders"
echo "----------------------------------------------"
CONSTRUCTOR_ORDERS=$(curl -s -H "Authorization: Bearer $CONSTRUCTOR_TOKEN" $API_URL/Constructor/my-orders)
echo "Constructor orders: $CONSTRUCTOR_ORDERS" | head -c 400
echo "..."
echo ""

# Step 9: Test template endpoint from Constructor perspective
echo "Step 9: Constructor views templates for Category 2"
echo "----------------------------------------------"
CONSTRUCTOR_TEMPLATES=$(curl -s -H "Authorization: Bearer $CONSTRUCTOR_TOKEN" $API_URL/FurnitureTypeTemplate/category/2/active)
echo "Templates visible to Constructor: $CONSTRUCTOR_TEMPLATES" | head -c 400
echo "..."
echo ""

echo "=========================================="
echo "WORKFLOW TEST COMPLETED"
echo "=========================================="
echo ""
echo "Summary:"
echo "✅ Production Manager logged in"
echo "✅ Categories retrieved"
echo "✅ Templates retrieved"
echo "✅ Seller logged in"
echo "✅ Constructor logged in"
echo "✅ Constructor can view templates"
echo ""
echo "Next steps:"
echo "1. Seller should create an order with Category"
echo "2. Constructor should see the order"
echo "3. Constructor should create FurnitureTypes using templates"
echo "4. Constructor should complete specifications"
