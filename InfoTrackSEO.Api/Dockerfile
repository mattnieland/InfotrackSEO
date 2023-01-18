FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["InfoTrackSEO.Api/InfoTrackSEO.Api.csproj", "InfoTrackSEO.Api/"]
RUN dotnet restore "InfoTrackSEO.Api/InfoTrackSEO.Api.csproj"
COPY . .
WORKDIR "/src/InfoTrackSEO.Api"
RUN dotnet build "InfoTrackSEO.Api.csproj" -c Prod -o /app/build

FROM build AS publish
RUN dotnet publish "InfoTrackSEO.Api.csproj" -c Prod -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InfoTrackSEO.Api.dll"]