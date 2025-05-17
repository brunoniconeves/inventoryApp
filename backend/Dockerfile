FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["InventoryApp.Api/InventoryApp.Api.csproj", "InventoryApp.Api/"]
RUN dotnet restore "InventoryApp.Api/InventoryApp.Api.csproj"
COPY . .
WORKDIR "/src/InventoryApp.Api"
RUN dotnet build "InventoryApp.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InventoryApp.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryApp.Api.dll"] 