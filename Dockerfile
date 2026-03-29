# Use the official .NET runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["ai-image-generator-backend.csproj", "./"]
RUN dotnet restore "ai-image-generator-backend.csproj"

# Copy the rest of the files and build
COPY . .
WORKDIR "/src/."
RUN dotnet build "ai-image-generator-backend.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ai-image-generator-backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ai-image-generator-backend.dll"]
