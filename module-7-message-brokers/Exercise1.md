# Module 7: Message Brokers
Let's dive into more advanced concepts and use cases associated with Azure Service Bus and Azure Event Grid. This module covers topics such as handling large message volumes, advanced routing, message deduplication, handling message failures, and integrating third-party services
# Exercise 1
In this exercise you will use **Azure Event Grid** for sending messages between the GameAPI and the StatsAPI. Azure Event Grid is a very good message broker when it comes to handling a large number of messages. By using it, the data of finished games will transfer through the Event Grid, instead of the standard HTTP requests. 

## Estimated time: TODO minutes

## Learning objectives
   - Use Azure Event Grid for sending messages between resources.
   
## Prerequisites

To begin this module you will need the Azure resources that you deployed in the previous modules.

During this module you will also need the following PowerShell variables used previously:
 - $GitPAT - your GitHub token (PAT) value
 - $GitRepositoryOwner - your GitHub owner name (lowercase)
 - $Location - location of the first region deployed in Module 0
 - $APIResourceGroup  - name of your API Resource Group deployed in Module 1
 - $GameApi - name of your Game API resource
 - $StatsApi - name of your Stats API resource
 - $DB_Connection - CosmosDB Connection String
 - $GameContainerUrl - URL for your Game Container API
 - $BotContainerUrl - url of your Bot API container
 - $SignalREndpoint - endpoint for your Azure SignalR Resource
 - $StatsContainerUrl - url of your Stats API container
 - $SMTP - connection string of your Azure Communication Service deployed in Module 2
 - $Sender - your noreply email from the Email Communication Service Domain resource deployed in Module 2
 - $TTL - time-to-live for your database documents
	
## Step 1: Deploy an Azure Event Grid Topic

1.1 Create a new Resource Group for your Event Grid

1.2 Enable Event Grid resource provider for your subscription if it was not enabled before

> [!NOTE]  
> Keep in mind that this action might take a while to finish.

1.3 Create your Event Grid Topic 

## Step 2: Deploy the new versions of your containers

2.1 Redeploy the StatsAPI

Update the StatsAPI with the new image. This includes an endpoint that we will use later to establish an Event Grid Subscription

2.2 Store the Event Grid Credentials

In order to use the Event Grid inside the WebAPI, you need to store a couple of variables inside your terminal.

   1. Store the URL of the Event Grid. You can find it in your [Azure Portal](https://portal.azure.com/) by navigating to your Event Grid Topic resource, on the **Overview** tab, next to **Topic Endpoint**

   2. Store the Key of the Event Grid Topic. You can find it in the **Access keys** tab, under **Settings**, next to **Key 1** or **Key 2**.

2.3 Redeploy the GameAPI

Then, you need to update the GameAPI with the new image. The GameAPI will be able to send events to the Event Grid after updating the container.

## Step 3: Create a subscription for your StatsAPI
For the StatsAPI to be able to receive events from the Event Grid, it needs to be subscribed to the Event Grid Topic you created moments ago.
The StatsAPI app is configured so that the path where the container will listen is `/api/eventhandler`

3.1 Define your subscription endpoint and the Resource ID of your Topic

3.2 Create a Subscription for the Topic

## Step 4: Test the application

Now you can test the application by playing a game inside your Static Web App.
The leaderboard will now update on every game that is played, updating the cache of the CosmosDB. The data will still be cached the same way, but you won't have that problem of cache invalidation you had at Module 4. By rising the value of TTL, it also means that the calculations needed for updated the leaderboard will be made less frequently, essentially saving Request Units.

