# Module 7: Message Brokers
Let's dive into more advanced concepts and use cases associated with Azure Service Bus and Azure Event Grid. This module covers topics such as handling large message volumes, advanced routing, message deduplication, handling message failures, and integrating third-party services
# Exercise 1
In this exercise you will use **Azure Event Grid** for sending messages between the GameAPI and the StatsAPI. Azure Event Grid is a very good message broker when it comes to handling a large number of messages. By using it, the data of finished games will transfer through the Event Grid, instead of the standard HTTP requests. 

## Estimated time: TODO minutes

## Learning objectives
   - Use Azure Event Grid for sending messages between resources.
   
## Prerequisites

To begin this module you will need the Azure resources that you deployed in the previous modules.

During this module you will also need 8 of the PowerShell variables used previously:
 - $Location - location of the first region deployed in Module 1
 - $APIResourceGroup  - name of your API Resource Group deployed in Module 1
 - $DB_Connection - CosmosDB Connection String
 - $BotContainerUrl - url of your Bot API container
 - $SignalREndpoint - endpoint for your Azure SignalR Resource
 - $StatsContainerUrl - url of your Stats API container
 - $SMTP - connection string of your Azure Communication Service deployed in Module 2
 - $Sender - your noreply email from the Email Communication Service Domain resource deployed in Module 2
	
## Step 1: Deploy an Azure Event Grid Topic

1. Create a new Resource Group for your Event Grid
```powershell
$GridRGName = <event-grid-resource-group-name>
```

```powershell
az group create --name $GridRGName --location $Location
```
2. Enable event grid resource provider for your subscription if it was not enabled before

```powershell
az provider register --namespace Microsoft.EventGrid
```
Keep in mind that this action might take a while to finish.

3.Create your Event Grid Topic 
```powershell
$TopicName = <your-topic-name>
```
```powershell
az eventgrid topic create --name $TopicName -l $Location -g $GridRGName
```

## Step 2: Deploy the new versions of your containers

1. Redeploy the StatsAPI
 
You need to update the StatsAPI with the new image. This includes an endpoint that we will use later to establish an Event Grid Subscription
```powershell
az containerapp up -n $StatsApi --resource-group $APIResourceGroup --image sergiu017/stats-api:latest --env-vars STATS_API_DB_CONNECTION_STRING=$DB_Connection STATS_API_TTL=$TTL
```
2. Store the Event Grid Credentials

In order to use the Event Grid inside the WebAPI, you need to store a couple of variables inside your terminal.

First, you need to store the URL of the Event Grid. You can find it in your [Azure Portal](https://portal.azure.com/) by navigating to your Event Grid Topic resource, on the **Overview** tab, next to **Topic Endpoint**

```powershell
$EventGridEndpoint=<topic-endpoint>
```
Then, you need to store the Key of the Event Grid Topic. You can find it in the **Access keys** tab, under **Settings**, next to **Key 1** or **Key 2**.

```powershell
$EventGridKey=<topic-key>
```
3. Redeploy the GameAPI

Then, you need to update the GameAPI with the new image. The GameAPI will be able to send events to the Event Grid after updating the container.
```powershell
az containerapp up --name $GameApi --resource-group $APIResourceGroup --image sergiu017/game-api:latest --env-vars GAME_API_SIGNALR=$SignalREndpoint GAME_API_BOTAPI=$BotContainerUrl GAME_API_HOST=$GameContainerUrl GAME_API_SMTPSERVER=$SMTP GAME_API_SMTP_SENDER=$Sender GAME_API_STATSAPI=$StatsContainerUrl GAME_API_EVENT_GRID_ENDPOINT=$EventGridEndpoint GAME_API_EVENT_GRID_KEY=$EventGridKey
```
## Step 3: Create a subscription for your StatsAPI
For the StatsAPI to be able to recieve events from the Event Grid, it needs to be subscribed to the Event Grid Topic you created moments ago.
The StatsAPI app is configured so that the path where the container will listen is `/api/eventhandler`

First, you need to define your subscription endpoint and the Resource ID of your Topic.

```powershell
$SubEndpoint=<stats-api-container-url>/api/eventhandler
```
```powershell
$TopicResourceId=az eventgrid topic show --resource-group $GridRGName --name $topicname --query "id" --output tsv
```
Then, you can create a Subscription for the topic.
```powershell
$SubName=<event-subscription-name>
```
```powershell
az eventgrid event-subscription create --source-resource-id $topicresourceid --name $SubName --endpoint $SubEndpoint
```
## Step 4: Test the application

Now you can test the application by playing a game inside your Static Web App.
The leaderboard will now update on every game that is played, updating the cache of the CosmosDB. The data will still be cached the same way, but not you won't have that problem of cache invalidation you had at Module 4.

