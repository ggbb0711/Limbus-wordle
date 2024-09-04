FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore any dependencies
COPY ["Limbus-wordle.csproj", "./"]
RUN dotnet restore "Limbus-wordle.csproj"

# Copy the rest of your application code
COPY . .
WORKDIR "/src/."

# Build the application
RUN dotnet build "Limbus-wordle.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "Limbus-wordle.csproj" -c Release -o /app/publish

# Build a runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY Content /app/Content
ENTRYPOINT ["dotnet", "Limbus-wordle.dll", "--urls", "http://0.0.0.0:8080"]
