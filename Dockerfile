# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution and project files
COPY FunPokedexApi.slnx .
COPY FunPokedexApi/FunPokedexApi.csproj FunPokedexApi/
COPY FunPokedexApi.Application/FunPokedexApi.Application.csproj FunPokedexApi.Application/
COPY FunPokedexApi.Infrastructure/FunPokedexApi.Infrastructure.csproj FunPokedexApi.Infrastructure/

# Restore dependencies
RUN dotnet restore FunPokedexApi/FunPokedexApi.csproj

# Copy everything else
COPY FunPokedexApi/ FunPokedexApi/
COPY FunPokedexApi.Application/ FunPokedexApi.Application/
COPY FunPokedexApi.Infrastructure/ FunPokedexApi.Infrastructure/

# Build and publish
RUN dotnet publish FunPokedexApi/FunPokedexApi.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "FunPokedexApi.dll"]