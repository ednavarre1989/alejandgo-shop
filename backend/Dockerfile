# Etapa 1: compilar y publicar
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore src/AlejandgoShop.Api/AlejandgoShop.API.csproj
RUN dotnet publish src/AlejandgoShop.Api/AlejandgoShop.API.csproj -c Release -o /app

# Etapa 2: imagen final, solo runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "AlejandgoShop.API.dll"]