# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solución y proyectos
COPY CalculoBasesAIE.sln ./
COPY CalculoBasesAIE/ ./CalculoBasesAIE/
COPY CalculoBasesAIE.Tests/ ./CalculoBasesAIE.Tests/

# Restaurar dependencias
RUN dotnet restore

# Publicar solo el proyecto principal
WORKDIR /src/CalculoBasesAIE
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiar archivos publicados
COPY --from=build /app/publish .

# Configurar entorno y puerto
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

# Ejecutar la API
ENTRYPOINT ["dotnet", "CalculoBasesAIE.dll"]