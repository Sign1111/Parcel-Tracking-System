
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY ["ParcelTracking/ParcelTracking.csproj", "ParcelTracking/"]
RUN dotnet restore "ParcelTracking/ParcelTracking.csproj"

# copy everything else and build
COPY . .
WORKDIR "/src/ParcelTracking"
RUN dotnet publish "ParcelTracking.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ParcelTracking.dll"]
