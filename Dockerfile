# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solo el proyecto actual (ya estás dentro de CalculoBasesAIE/)
COPY CalculoBasesAIE.csproj ./
RUN dotnet restore CalculoBasesAIE.csproj

COPY . ./
RUN dotnet publish CalculoBasesAIE.csproj -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "CalculoBasesAIE.dll"]