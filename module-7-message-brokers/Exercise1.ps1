# Module 7: Message Brokers
# Exercise 1

# ---------------------------------------------------------------------------------------------------------------------
	
# Step 1: Deploy an Azure Event Grid Topic

# 1.1 Create a new Resource Group for your Event Grid

$GridResourceGroup = "<event-grid-resour-group-name>"
az group create --name $GridResourceGroup --location $Location

# 1.2 Enable Event Grid resource provider for your subscription if it was not enabled before

az provider register --namespace Microsoft.EventGrid

# Keep in mind that this action might take a while to finish!

# 1.3 Create your Event Grid Topic 

$TopicName = "<topic-name>"
az eventgrid topic create --name $TopicName -l $Location -g $GridResourceGroup

# ---------------------------------------------------------------------------------------------------------------------

# Step 2: Deploy the new versions of your containers

# 2.1 Redeploy the StatsAPI

# You can set the TTL to a higher value as now it will also update on game finish.

$TTL = 86400 # one day

az containerapp up -n $StatsApi --resource-group $APIResourceGroup --image ghcr.io/$GitRepositoryOwner/statsapi-rockpaperscissors:module7-ex1 --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --env-vars STATS_API_DB_CONNECTION_STRING=$DB_Connection STATS_API_TTL=$TTL

# 2.2 Store the Event Grid Credentials

$EventGridEndpoint = "<topic-endpoint>"
$EventGridKey = "<topic-key>"

# 2.3 Redeploy the GameAPI

az containerapp up --name $GameApi --resource-group $APIResourceGroup --image ghcr.io/$GitRepositoryOwner/gameapi-rockpaperscissors:module7-ex1 --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --env-vars GAME_API_SIGNALR=$SignalREndpoint GAME_API_BOTAPI=$BotContainerUrl GAME_API_HOST=$GameContainerUrl GAME_API_SMTPSERVER=$SMTP GAME_API_SMTP_SENDER=$Sender GAME_API_STATSAPI=$StatsContainerUrl GAME_API_EVENT_GRID_ENDPOINT=$EventGridEndpoint GAME_API_EVENT_GRID_KEY=$EventGridKey

# ---------------------------------------------------------------------------------------------------------------------

# Step 3: Create a subscription for your StatsAPI

# 3.1 Define your subscription endpoint and the Resource ID of your Topic
$SubEndpoint = "<stats-api-container-url>/api/eventhandler"

$TopicResourceId=az eventgrid topic show --resource-group $GridResourceGroup --name $topicname --query "id" --output tsv

# 3.2 Create a Subscription for the Topic

$SubName = "<event-subscription-name>"

az eventgrid event-subscription create --source-resource-id $topicresourceid --name $SubName --endpoint $SubEndpoint

# ---------------------------------------------------------------------------------------------------------------------
