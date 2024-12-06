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
   - Configure dapr statestore using CosmosDB

## Prerequisites

Create your own repository in Github and add the solution provided in [ext_AdvancedAzureArchitecture](https://github.com/KeyTicket-Solutions/ext_AdvancedAzureArchitecture)

The following steps are written for Bash using variables and parameters to make the scripts flexible.

Throughout this exercise you will be required to name some of your resources and for this we advise you to read this [Azure documentation](https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-naming) regarding Azure resources naming conventions.

During the entire exercise you can check your deployments or settings made from certain commands in the [Azure Portal](https://portal.azure.com/). We encourage
you to use the portal to monitor your deployments at any point.

## Step 1: Setting Up the Environment

1. **Install Required Tools**
  - Download and install **az cli** from [here](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli#install-or-update).
  - Verify if `az` command is installed:

```bash
   az --help
```

2. **Login to your Azure account and select your subscription**
   
  - Login to your Azure subscription

```bash
   az login
```


3. **Select your subscription by typing the number assigned to it**

## Step 2: Create the resource group and deploy the arm
1. **Create your resource-group for APIs**


```bash
   export APIResourceGroup="<resource-group-name>"
```

```bash
   export Location="<resource-group-location>"
```


- Use the command bellow to list all Azure locations

```bash
   az account list-locations -o table
``` 
   
   - `<resource-group-name> = Your resource group name`
   - `<resource-group-location> = Your resource group location`


  ```bash
   az group create --name $APIResourceGroup --location $Location
   ```


2. **Create a resource-group for the database**

```bash
export DBResourceGroup="<resource-group-name>"
```


```bash
   az group create --name $DBResourceGroup --location $Location
```

3. **Set the variables used on deployment**


```bash
   export DatabaseAccount="<db-account-name>"
```


```bash
   export BotApi="<botapi-container-name>"
```

 ```bash
   export GameApi="<gameapi-container-name>"
 ```

```bash
   export ManagedEnvironment="<managed-environment>"
```

   - `<db-account-name> = Your Cosmos Database Account name`
   - `<botapi-container-name> = Your Bot container name`
   - `<gameapi-container-name> = Your Game container name`
   - `<managed-environment> = Your Managed Environment resource name`

1. **Deploy the ARM using configured variables**
- Set the path to your ARM deploy

```bash
   cd "<path-to-arm-deploy>"
```


- `<path-to-arm-deploy> = Local path to the API arm deployment file from the infra folder`
  
- Deploy the API arm


```bash
   az deployment group create --resource-group $APIResourceGroup --template-file azuredeployAPI.json --parameters containerapps_bot_api_name=$BotApi containerapps_game_api_name=$GameApi managedEnvironments_env_name=$ManagedEnvironment location=$Location
   ```


- Deploy the database arm

```bash
   az deployment group create --resource-group$DBResourceGroup --template-file azuredeployDB.json --parameters databaseAccounts_db_name=$DatabaseAccount location=$Location
   ```

At the end of this step you can open [Azure Portal](https://portal.azure.com/) and see your deployed resources: Bot Container API, Game Container API and a Cosmos DB

## Step 3: Create an Azure Static Web App 

1. **Deploy your Static Web Ap in the same Resource Group with the APIs**

```bash
   export StaticWeb="<Static-Web-App-Name>"
   ```


```bash
   az staticwebapp create --name $StaticWeb --resource-group $APIResourceGroup --source <github-repository-url> --branch <branch> --app-location "/module-1-azure-architecture-introduction/src/Exercise_1/RockPaperScissors" --api-location "/module-1-azure-architecture-introduction/src/Exercise_1/RockPaperScissorsAPI" --output-location "wwwroot" --login-with-github
   ```

   - `<Static-Web-App-Name> = Your Static Web App name`
   - `<github-repository-url> = Your Github Repository url`
   - `<branch> = The branch you want to use for deployment`
2. **Configure an environment variable to connect your static web app with your game Container Api**


```bash
   export GameContainerUrl="<game-api-container-url>"
   ```

```bash
   export BotContainerUrl="<bot-container-url>"
   ```


```bash
   export BotContainerHN="<bot-container-host-name>"
   ```


```bash
   export GameContainerHN="<game-container-host-name>"
   ```


```bash
   az staticwebapp appsettings set --name $StaticWeb --setting-names "GAMEAPI_URL=$GameContainerUrl" "BOTAPI_URL=$BotContainerUrl"
   ```


   - `<bot-container-url> = Your Bot container api url`
   - `<game-api-container-url> = Url created on game api container`
   - `<bot-container-host-name> = Hostname of bot container url`
   - `<game-container-host-name> = Hostname of game container url`

  
3. **At the end of this step you will be able to see your Static Web app deployed in [Azure Portal](https://portal.azure.com/) and you can even open your web app and it should look like this:** ![](https://github.com/KeyTicket-Solutions/ext_AdvancedAzureArchitecture/blob/dev/module-1-azure-architecture-introduction/images/image3.png)
   

## Step 4: Configure dapr statestore using Cosmos DB
1. **Install az containerapp extension**

 ```powershell
   az extension add --name containerapp --upgrade
   ```


2. **Configuring statestore using **statestore.yaml** file from the local *infra* folder**


```bash
   cd "<your-statestore.yaml-path>"
   ```


   - `<your-statestore.yaml-path> = The path for statestore.yaml file that you cloned from github repository`
3. **Open the file and edit the following variables: `<cosmos-url>` and `<cosmos-primary-key>`**
    - `<cosmos-url> = Your Cosmos DB url`
    - `<cosmos-primary-key> = Your Cosmos DB primary key`
4. **Update the Managed Environment**


```bash
   az containerapp env dapr-component set --name $ManagedEnvironment --resource-group $APIResourceGroup --dapr-component-name statestore --yaml statestore.yaml
```

## Step 5: Configure environment variables for Azure Container Apps
1. **Configure environment variable for Game Container Api**


```bash
   az containerapp up --name $GameApi --resource-group $APIResourceGroup --image casianbara/gameapi-rockpaperscissors:latest --env-vars GAME_API_BOTAPI="$BotContainerUrl"
   ```


2. **Configure environment variable for Bot Container Api**


```bash
   az containerapp up --name $BotApi --resource-group $APIResourceGroup --image casianbara/botapi-rockpaperscissors:latest --env-vars BOT_API_SESSION_URL=$GameContainerUrl
   ```


## Step 6: Deploy the second Container App on another region
1. **Create the Resource Group**


```bash
   export ResourceGroup2="<resource-group-name>"
```

```bash
   export Location2="<location-name>"
```

   - `<resource-group-name> = Second resource group name`
   - `<location-name> = Second location for resource group`
 
2. **Run the create command**
   

```bash
   az group create --name $ResourceGroup2 --location $Location2
```


3. **Create the environment**


```bash
   export ManagedEnvironment2="<environment-name>"
   ```

```bash
   az containerapp env create --name $ManagedEnvironment2 --resource-group $ResourceGroup2 --location $Location2
   ```


- `<environment-name> = Your second managed environment name`
  
4. **Update the Managed Environment**


```bash
   az containerapp env dapr-component set --name $ManagedEnvironment2 --resource-group $ResourceGroup2 --dapr-component-name statestore --yaml statestore.yaml
   ```


5. **Create your second container app and save its host name in a variable for later**
   

```bash
   export BotApi2="<container-name>"
```

```bash
   az containerapp create --name $BotApi2 --resource-group $ResourceGroup2 --environment $ManagedEnvironment2 --image  casianbara/botapi-rockpaperscissors:latest --target-port 8080 --ingress external --query properties.configuration.ingress.fqdn --env-vars BOT_API_SESSION_URL=$GameContainerUrl --enable-dapr --dapr-app-id botapi --dapr-app-port 8080
```


```bash
   export BotContainerHN2="<second-bot-container-host-name>"
```

- `<container-name> = Your second container name`
- `<second-bot-container-host-name> = Second bot container hostname`

## Step 7: Configure front door to connect both regions from bot container api
1. **Create a new resource-group for front door**

```bash
   export NetworkResourceGroup="<resource-group-name>"
```


- `<resource-group-name> = Your network resource group name`



```bash
   az group create --name $NetworkResourceGroup --location $Location
```


2. **Create Azure Front Door profile**
   

```bash
   export ProfileName="<profile-name>"
   ```

```bash
   az afd profile create --profile-name $ProfileName --resource-group $NetworkResourceGroup --sku Premium_AzureFrontDoor
```

   - `<profile-name> = Name your profile`
  
3. **Create Azure Front Door endpoint for the bot containers**


```bash
   export EndpointName="<endpoint-name>"
```


```bash
   az afd endpoint create --resource-group $NetworkResourceGroup  --endpoint-name $EndpointName --profile-name $ProfileName --enabled-state Enabled
```


   - `<endpoint-name> = Name your endpoint`
4. **Create an origin group**


```bash
   export OriginGroupName="<origin-group-name>"
```


 ```bash
   az afd origin-group create --resource-group $NetworkResourceGroup --origin-group-name $OriginGroupName --profile-name $ProfileName --probe-request-type GET --probe-protocol HTTPS --probe-interval-in-seconds 10 --probe-path "/" --sample-size 4 --successful-samples-required 3 --additional-latency-in-milliseconds 50 --enable-health-probe true
```


   
   - `<origin-group-name> = Name your origin group`
5. **Create origins**
- Create first origin


 ```bash
   az afd origin create --resource-group $NetworkResourceGroup --host-name $BotContainerHN --profile-name $ProfileName --origin-group-name $OriginGroupName --origin-name <first-origin-name> --origin-host-header $BotContainerHN --priority 1 --weight 1000 --enabled-state Enabled --http-port 8080 --https-port 443 --enable-private-link false
   ```


   - `<first-origin-name> = First origin name`
  
- Create second origin


```bash
   az afd origin create --resource-group $NetworkResourceGroup --host-name $BotContainerHN2 --profile-name $ProfileName 
--origin-group-name $OriginGroupName --origin-name <second-origin-name> --origin-host-header $BotContainerHN2 --priority 2 --weight 1000 --enabled-state Enabled --http-port 8080 --https-port 443 --enable-private-link false
   ```


   - `<second-origin-name> = Second origin name`
6. **Create Front Door route**


```bash
   az afd route create --resource-group $NetworkResourceGroup --profile-name $ProfileName --endpoint-name $EndpointName  --forwarding-protocol MatchRequest --route-name route --https-redirect Enabled --origin-group $OriginGroupName --supported-protocols Http Https --link-to-default-domain Enabled
   ```


7. **List endpoint to get the Front Door link and save it on a variable**
   

```bash
   az afd endpoint show --resource-group $NetworkResourceGroup --profile-name $ProfileName --endpoint-name $EndpointName
   ```

```bash
   export Endpoint="https://<endpoint-url>"
   ```


   - `<endpoint-url> = Front Door endpoint url for game`
   - Add `https://` to Front Door endpoint hostname

## Step 8: Configure Front Door to connect both regions from game Container Api

1. **Create gameapi container on second region**


```bash
   export GameApi2="<container-name>"
```


```bash
   az containerapp create --name $GameApi2 --resource-group $ResourceGroup2 --environment $ManagedEnvironment2 --image  casianbara/gameapi-rockpaperscissors:latest --target-port 8080 --ingress external --query properties.configuration.ingress.fqdn --env-vars GAME_API_BOTAPI="$BotContainerUrl" --enable-dapr --dapr-app-id gameapi --dapr-app-port 8080
```


```bash
   export GameContainerHN2="<second-bot-container-host-name>"
```

   - `<container-name> = Your second container name`
   - `<second-bot-container-host-name> = Second game container hostname`

2. **Create a second endpoint for the game containers**


```bash
   export EndpointName2="<endpoint-name>"
```


```bash
   az afd endpoint create --resource-group $NetworkResourceGroup  --endpoint-name $EndpointName2 --profile-name $ProfileName --enabled-state Enabled
```


   - `<endpoint-name> = Name your endpoint`
  
3. **Create a second origin group**

```bash
   export OriginGroupName2="<origin-group-name>"
```


```bash
   az afd origin-group create --resource-group $NetworkResourceGroup --origin-group-name $OriginGroupName2 --profile-name $ProfileName --probe-request-type GET --probe-protocol HTTPS --probe-interval-in-seconds 10 --probe-path "/" --sample-size 4 --successful-samples-required 3 --additional-latency-in-milliseconds 50 --enable-health-probe true
```

   - `<origin-group-name> = Name your second origin group for game`

4. **Create origins**
   
- Create first game origin


 ```bash
   az afd origin create --resource-group $NetworkResourceGroup --host-name $GameContainerHN --profile-name $ProfileName --origin-group-name $OriginGroupName2 --origin-name <first-origin-name> --origin-host-header $GameContainerHN --priority 1 --weight 1000 --enabled-state Enabled --http-port 8080 --https-port 443 --enable-private-link false
```


   - `<first-origin-name> = First origin name for game`

- Create second game origin


```bash
   az afd origin create --resource-group $NetworkResourceGroup --host-name $GameContainerHN2 --profile-name $ProfileName --origin-group-name $OriginGroupName2 --origin-name <second-origin-name> --origin-host-header $GameContainerHN2 --priority 2 --weight 1000 --enabled-state Enabled --http-port 8080 --https-port 443 --enable-private-link false
   ```


   - `<second-origin-name> = Second origin name for game`
5. **Create Front Door route for game**


```bash
   az afd route create --resource-group $NetworkResourceGroup --profile-name $ProfileName --endpoint-name $EndpointName2  --forwarding-protocol MatchRequest --route-name route --https-redirect Enabled --origin-group $OriginGroupName2 --supported-protocols Http Https --link-to-default-domain Enabled
   ```


6. **List second endpoint to get the Front Door link and save it on a variable**


```bash
   az afd endpoint show --resource-group $NetworkResourceGroup --profile-name $ProfileName --endpoint-name $EndpointName2
   ```


```bash
   export Endpoint2="https://<endpoint-url>"
   ```


   - `<endpoint-url> = Front Door endpoint url for game`
   - Add `https://` to endpoint hostname

## Step 9: Use the endpoints to configure Azure Container Apps and Static Web

1. **Modify environment variables for Azure Container Apps**


```bash
   az containerapp up --name $GameApi --resource-group $APIResourceGroup --image casianbara/gameapi-rockpaperscissors:latest --env-vars GAME_API_BOTAPI=$Endpoint
   ```


```bash
   az containerapp up --name $BotApi --resource-group $APIResourceGroup --image casianbara/botapi-rockpaperscissors:latest --env-vars BOT_API_SESSION_URL=$Endpoint2
   ```


```bash
   az containerapp up --name $GameApi2 --resource-group $ResourceGroup2 --image casianbara/gameapi-rockpaperscissors:latest --env-vars GAME_API_BOTAPI=$Endpoint
   ```


```bash
   az containerapp up --name $BotApi2 --resource-group $ResourceGroup2 --image casianbara/botapi-rockpaperscissors:latest --env-vars BOT_API_SESSION_URL=$Endpoint2
   ```


2. **Modify environment variables for Static Web App**

```bash
   az staticwebapp appsettings set --name $StaticWeb --setting-names "GAMEAPI_URL=$Endpoint2" "BOTAPI_URL=$Endpoint"
   ```


3. Add `*` to **CORS** manually under **Settings** tab for Azure Container Apps created on second region from [Azure Portal](https://portal.azure.com/)

![](https://github.com/KeyTicket-Solutions/ext_AdvancedAzureArchitecture/blob/dev/module-1-azure-architecture-introduction/images/image4.png)


## Testing your deployment

#### After deploying all the necessary resources, you can start testing your web application using the [Azure Portal](https://portal.azure.com/).

#### You can play a game of rock, paper, scissors with a bot or invite someone to play against you.

#### To test Azure Front Door, stop an API container in your first region (to simulate a regional failure) and then access the application, the web application should work using the API container in your second region.