FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update && apt-get install -y gss-ntlmssp

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["StatsAPI.csproj", "."]
RUN dotnet restore "./StatsAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "StatsAPI.csproj" -c Debug -o /app/build 

FROM build AS publish
RUN dotnet publish "StatsAPI.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV STATS_API_DB_CONNECTION_STRING="Address"
ENV STATS_API_CACHE_MINUTES="Address"
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StatsAPI.dll"]