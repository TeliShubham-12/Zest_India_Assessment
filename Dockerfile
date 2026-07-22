# Use official .NET SDK image to build app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["StudentManagement.API/StudentManagement.API.csproj", "StudentManagement.API/"]
COPY ["StudentManagement.Application/StudentManagement.Application.csproj", "StudentManagement.Application/"]
COPY ["StudentManagement.Infrastructure/StudentManagement.Infrastructure.csproj", "StudentManagement.Infrastructure/"]
COPY ["StudentManagement.Domain/StudentManagement.Domain.csproj", "StudentManagement.Domain/"]

RUN dotnet restore "StudentManagement.API/StudentManagement.API.csproj"

# Copy source code and build
COPY . .
WORKDIR "/src/StudentManagement.API"
RUN dotnet build "StudentManagement.API.csproj" -c Release -o /app/build

# Publish application
FROM build AS publish
RUN dotnet publish "StudentManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudentManagement.API.dll"]
