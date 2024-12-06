FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update && apt-get install -y gss-ntlmssp

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BotAPI.csproj", "."]
RUN dotnet restore "./BotAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "BotAPI.csproj" -c Debug -o /app/build 

FROM build AS publish
RUN dotnet publish "BotAPI.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV BOT_API_SESSION_URL="Address"
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BotAPI.dll"]