# Module 1: Azure Architecture Introduction
This module provides an overview of Azure's global infrastructure and architectural building blocks. 
We delve into the complexity of Azure's region pairs, availability zones, and the architectural design 
principles that ensure high availability, fault tolerance, and disaster recovery.


# Exercise 1

In this exercise you will deploy a game of Rock, Paper, Scissors on a web app with two APIs, one for the game itself and
one for the bot that will be used to play the game with. The APIs will be deployed in two different regions and you will
connect both regions using a Front Door. At the end of the exercise you will be able to stop the resources from one of 
the regions and the app will automatically reconnect to the other region. 

## Estimated time: 50 minutes

## Learning objectives
   - Deploy Container Apps and a Web App using ARM templates 
   - Deploy and Configure a Front Door resource to connect two different regions to increase availability
   - Configure environment variables for Web Apps and Azure Container Apps
   - Configure Dapr statestore using CosmosDB

## Prerequisites

The following steps are written for Azure PowerShell using variables and parameters to make the scripts flexible.

Throughout this exercise you will be required to name some of your resources and for this we advise you to read this [Azure documentation](https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-naming) regarding Azure resources naming conventions.

During the entire exercise you can check your deployments or settings made from certain commands in the [Azure Portal](https://portal.azure.com/). We encourage
you to use the portal to monitor your deployments at any point.

## Step 1: Create the resource group and deploy the ARM
1. Create your resource-group for APIs
2. Create a resource-group for the database
3. Set the variables used on deployment
4. Deploy the ARM using configured variables
   - Set the path to your ARM deploy
   - Deploy the API ARM
   - Deploy the database ARM
At the end of this step you can open [Azure Portal](https://portal.azure.com/) and see your deployed resources: Bot Container API, Game Container API, Container App Environment, Virtual Network and a Cosmos DB.

## Step 2: Create an Azure Static Web App 

1. Deploy your static web app in the same Resource Group with the APIs
2. Configure an environment variable to connect your Static Web App with your game Container Api
3. At the end of this step you will be able to see your Static Web app deployed in [Azure Portal](https://portal.azure.com/) and you can even open your web app and it should look like this: ![](../module-1-azure-architecture-introduction/images/image3.png)
   
## Step 3: Configure dapr statestore using Cosmos DB
1. Install az containerapp extension
2. Configuring statestore using **statestore.yaml** file from the local *infra* folder``
3. Open the file and edit the following variables: `<cosmos-url>` and `<cosmos-primary-key>`
    - `<cosmos-url> = Your Cosmos DB url`
    - `<cosmos-primary-key> = Your Cosmos DB primary key`
4. Update the Managed Environment
## Step 4: Configure environment variables for Azure Container Apps
1. Configure environment variable for Game Container Api
2. Configure environment variable for Bot Container Api

## Step 5: Deploy the second Container App on another region
1. Create the resource group
2. Run the create command
3. Create the environment
4. Update the Managed Environment
5. Create your second Container App and save its host name in a variable for later
## Step 6: Configure Front Door to connect both regions from bot Container Api
1. Create a new resource-group for Front Door
2. Create Azure Front Door profile
3. Create Azure Front Door endpoint
4. Create an origin group
5. Create origins
   - Create first origin
   - Create second origin
6. Create Front Door route
7. List endpoint to get the Front Door link and save it on a variable
## Step 7: Configure Front Door to connect both regions from game Container Api

1. Create gameapi container on second region
2. Create another endpoint
3. Create a second origin group
4. Create origins  
   - Create first game origin
   - Create second game origin
5. Create Front Door route for game
6. List second endpoint to get the Front Door link and save it on a variable
## Step 8: Use the endpoints to configure Azure Container Apps and Static Web

1. Modify environment variables for Azure Container Apps
2. Modify environment variables for Static Web App
3. Add `*` to **CORS** manually under **Settings** tab for Azure Container Apps created on second region from [Azure Portal](https://portal.azure.com/)

![](../module-1-azure-architecture-introduction/images/image4.png)

## Deploy Button
If you get stuck during the exercise you can use this button to deploy all the resources from this exercise:

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2F{Owner}%2F{Repo}%2Frefs%2Fheads%2Fmaster%2Finfra%2Farm%2FazuredeployARM.json)

After deploying all resources using the button you need to go through **step 9** to update environment variables for api containers and also do **step 6, point 4.**

List of Instance details for deploying with button:

- Region1_containerapps_bot_api_name = $BotApi
- Region1_containerapps_game_api_name = $GameApi
- Region1_managed Environments_env_name = $ManagedEnvironment
- Location = $Location
- Region2_containerapps_bot_api_name = $BotApi2
- Region2_containerapps_game_api_name = $GameApi2
- Region2_managed Environments_env_name = $ManagedEnvironment2
- Location2 = $Location2
- API Resource Group = $APIResourceGroup
- API Resource Group2 = $ResourceGroup2
- Network Resource Group = $NetworkResourceGroup
- DB Resource Group = $DBResourceGroup
- Database Accounts_db_name = $DatabaseAccount
- Profiles_frontdoor_name = $ProfileName
- Static Sites_test_web_rps_we_name = $StaticWeb
- Repository Url = `Your Github Repository url`
- Branch = `The branch you want to use for deployment`
- Repository Token = `Your Github token (PAT) value`
- Endpointbot = $EndpointName
- Endpointgame = $EndpointName2
- Botgroup = $OriginGroupName
- Gamegroup = $OriginGroupName2
- Gameorigin1 = `First origin name for game`
- Gameorigin2 = `Second origin name for game`
- Botorigin1 = `First origin name for bot`
- Botorigin2 = `Second origin name for bot`
- Virtual Networks_vnet_name = $VnetName
- Environment-subnet-name = $EnvironmentSubnet
- Githubowner = $GitRepositoryOwner

## Testing your deployment

#### After deploying all the necessary resources, you can start testing your web application using the [Azure Portal](https://portal.azure.com/).

#### You can play a game of Rock, Paper, Scissors with a bot or invite someone to play against you.

#### To test Azure Front Door, stop an API container in your first region (to simulate a regional failure) and then access the application, the web application should work using the API container in your second region.
