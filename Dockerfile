# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the .csproj file using the correct relative path
COPY src/NotificationSystem/NotificationSystem.csproj ./ 

# Restore dependencies
RUN dotnet restore "NotificationSystem.csproj"

# Copy all other source files
COPY src/NotificationSystem/. ./

# Build the project
RUN dotnet publish "NotificationSystem.csproj" -c Release -o /app/publish

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "NotificationSystem.dll"]
