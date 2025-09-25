
# Use the .NET 7.0 SDK image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build


WORKDIR /src
COPY ["ParcelTracking/ParcelTracking.csproj", "ParcelTracking/"]
RUN dotnet restore "ParcelTracking/ParcelTracking.csproj"
COPY . .
WORKDIR "/src/ParcelTracking"
RUN dotnet build "ParcelTracking.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ParcelTracking.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ParcelTracking.dll"]
