
# Use the .NET 7.0 SDK image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src

# copy csproj and restore
COPY ParcelTracking.csproj ./
RUN dotnet restore "ParcelTracking.csproj"

# copy everything else
COPY . .

RUN dotnet build "ParcelTracking.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ParcelTracking.csproj" -c Release -o /app/publish

# runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ParcelTracking.dll"]
