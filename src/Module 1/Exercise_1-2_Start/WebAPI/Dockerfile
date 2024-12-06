FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update && apt-get install -y gss-ntlmssp

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["GameAPI.csproj", "."]
RUN dotnet restore "./GameAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "GameAPI.csproj" -c Debug -o /app/build 

FROM build AS publish
RUN dotnet publish "GameAPI.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV GAME_API_BOTAPI="Address"
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameAPI.dll"]