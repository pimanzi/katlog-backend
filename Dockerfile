# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj from Katlog.Api folder specifically
COPY Katlog.Api/*.csproj Katlog.Api/
COPY Katlog.Shared/*.csproj Katlog.Shared/

RUN dotnet restore Katlog.Api/

# Copy ALL files from root (all projects!)
COPY . .

# Publish specifically the Katlog.Api project
RUN dotnet publish Katlog.Api/ -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "katlog-backend.dll"]