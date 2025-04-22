# Base image para execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image para compilação e publicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src

# Copia apenas o arquivo de projeto e restaura as dependências
COPY ["UnitSaude.csproj", "./"]
RUN dotnet restore

# Copia o restante do código e realiza o build
COPY . .
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image para execução
FROM base AS final
ENV ASPNETCORE_ENVIRONMENT=Development
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UnitSaude.dll"]
