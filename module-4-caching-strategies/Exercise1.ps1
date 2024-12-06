# ----------------------------------------------------------------------------------------------------------------------

# Exercise 1

# ----------------------------------------------------------------------------------------------------------------------

# Step 1: Deploy StatsAPI

$StatsApi="<stats-container-name>"

$DB_Connection="<DB-connection-string>" 

$TTL="<data-time-to-live-seconds>"

az containerapp create -n $StatsApi --resource-group $APIResourceGroup --image ghcr.io/$GitRepositoryOwner/statsapi-rockpaperscissors:module4-ex1 --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --environment $ManagedEnvironment --ingress external --target-port 8080 --query properties.configuration.ingress.fqdn --env-vars STATS_API_DB_CONNECTION_STRING=$DB_Connection STATS_API_TTL=$TTL

# ----------------------------------------------------------------------------------------------------------------------

# Step 2: Add StatsAPI to Azure API Management

# ----------------------------------------------------------------------------------------------------------------------

# Step 3: Redeploy GameAPI and the web app

$StatsContainerUrl="<stats-container-url>"

az containerapp up --name $GameApi --resource-group $APIResourceGroup --image ghcr.io/$GitRepositoryOwner/gameapi-rockpaperscissors:module4-ex1 --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --env-vars GAME_API_SIGNALR=$SignalREndpoint GAME_API_BOTAPI=$BotContainerUrl GAME_API_HOST=$GameContainerUrl GAME_API_SMTPSERVER=$SMTP GAME_API_SMTP_SENDER=$Sender GAME_API_STATSAPI=$StatsContainerUrl

# ----------------------------------------------------------------------------------------------------------------------

# Step 4: Test the leaderboard

# ----------------------------------------------------------------------------------------------------------------------
