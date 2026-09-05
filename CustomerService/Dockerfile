# ==========================================
# Build Stage
# ==========================================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy CustomerService project
COPY ["CustomerService/CustomerService.csproj", "CustomerService/"]

# Copy Shared.Contracts project
COPY ["Shared.Contracts/Shared.Contracts.csproj", "Shared.Contracts/"]

# Restore dependencies
RUN dotnet restore "CustomerService/CustomerService.csproj"

# Copy remaining source code
COPY CustomerService/ CustomerService/
COPY Shared.Contracts/ Shared.Contracts/

# Publish
WORKDIR "/src/CustomerService"

RUN dotnet publish \
    "CustomerService.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# ==========================================
# Runtime Stage
# ==========================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CustomerService.dll"]