# Use the official .NET Core SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory to /app
WORKDIR /app

# Copy the current directory contents into the container at /app
COPY . .

# Restore the project dependencies
RUN dotnet restore

# Build the project and generate the output files
RUN dotnet publish -c Release -o out Aidoneus

# Use the official .NET Core runtime image as the base image
FROM mcr.microsoft.com/dotnet/runtime:7.0

# Set the working directory to /app
WORKDIR /app

# Copy the output files from the build stage into the container
COPY --from=build /app/out .

# Run the application
ENTRYPOINT ["dotnet", "Aidoneus.dll"]
