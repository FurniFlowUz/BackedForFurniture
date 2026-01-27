@echo off
echo ========================================
echo   FurniFlowUz - Starting All Servers
echo ========================================
echo.

echo [1/2] Starting Backend API...
start "FurniFlowUz Backend" cmd /k "cd /d %~dp0src\FurniFlowUz.API && dotnet run --launch-profile http"

echo [2/2] Waiting 10 seconds for backend to initialize...
timeout /t 10 /nobreak >nul

echo [2/2] Starting Frontend...
start "FurniFlowUz Frontend" cmd /k "cd /d %~dp0client && npm run dev"

echo.
echo ========================================
echo   All Servers Started!
echo ========================================
echo.
echo Backend:  http://localhost:5000
echo Swagger:  http://localhost:5000/swagger
echo Frontend: http://localhost:3000
echo.
echo Press any key to close this window...
echo (The servers will keep running in their own windows)
pause >nul
