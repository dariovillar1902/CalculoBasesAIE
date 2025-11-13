# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos del proyecto
COPY ./CalculoBasesAIE/CalculoBasesAIE.csproj ./CalculoBasesAIE/
RUN dotnet restore ./CalculoBasesAIE/CalculoBasesAIE.csproj

# Copiar el resto del código
COPY ./CalculoBasesAIE/ ./CalculoBasesAIE/
WORKDIR /src/CalculoBasesAIE
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Exponer el puerto por defecto
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

# Ejecutar la aplicación
ENTRYPOINT ["dotnet", "CalculoBasesAIE.dll"]