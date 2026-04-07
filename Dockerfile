FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/FurniFlowUz.API/FurniFlowUz.API.csproj", "FurniFlowUz.API/"]
COPY ["src/FurniFlowUz.Application/FurniFlowUz.Application.csproj", "FurniFlowUz.Application/"]
COPY ["src/FurniFlowUz.Domain/FurniFlowUz.Domain.csproj", "FurniFlowUz.Domain/"]
COPY ["src/FurniFlowUz.Infrastructure/FurniFlowUz.Infrastructure.csproj", "FurniFlowUz.Infrastructure/"]
RUN dotnet restore "FurniFlowUz.API/FurniFlowUz.API.csproj"
COPY src/ .
RUN dotnet publish "FurniFlowUz.API/FurniFlowUz.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "FurniFlowUz.API.dll"]
