FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV StorageEndpoint=host.docker.internal:29092

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PixelService/Pixel/Pixel.csproj", "PixelService/Pixel/"]
RUN dotnet restore "PixelService/Pixel/Pixel.csproj"
COPY . .
WORKDIR "/src/PixelService/Pixel"
RUN dotnet build "Pixel.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pixel.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pixel.dll"]
