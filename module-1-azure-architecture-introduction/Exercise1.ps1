# Module 1: Azure Architecture Introduction

#----------------------------------------------------------------------------------------------------------------------

# Exercise 1

#----------------------------------------------------------------------------------------------------------------------

## Step 1: Create the resource group and deploy the ARM

#1.1 Create your resource-group for APIs

$APIResourceGroup="<resource-group-name>" # <resource-group-name> = Your resource group name

$Location="<resource-group-location>" # <resource-group-location> = Your resource group location

#Use the command bellow to list all Azure locations

az account list-locations -o table

az group create --name $APIResourceGroup --location $Location

#1.2 Create a resource-group for the database

$DBResourceGroup="<resource-group-name>"  # <resource-group-name> = Your resource group name

az group create --name $DBResourceGroup --location $Location

#1.3 Set the variables used on deployment

$GitRepositoryOwner="<owner-repository-github>" # <owner-repository-github> = Your Github owner name (lowercase)

$GitPAT="<github-PAT>" # <github-PAT> = Your Github token (PAT) value

$DatabaseAccount="<db-account-name>" # <db-account-name> = Your Cosmos Database Account name

$BotApi="<botapi-container-name>" # <botapi-container-name> = Your Bot container name

$GameApi="<gameapi-container-name>" # <gameapi-container-name> = Your Game container name

$ManagedEnvironment="<managed-environment-name>" # <managed-environment-name> = Your Managed Environment resource name

$VnetName="<vnet-name>" # <vnet-name> = Your Vnet name for managed environment

$EnvironmentSubnet="<environment-subnet-name>" # <environment-subnet-name> = Your subnet name for managed environment

#1.4 Deploy the ARM using configured variables

#Set the path to your ARM deploy
  
cd "<path-to-arm-deploy>" # <path-to-arm-deploy> = Local path to the azuredeployAPI.json file located in the infra/arm folder.

#Deploy the API ARM

az deployment group create --resource-group $APIResourceGroup --template-file azuredeployAPI.json --parameters containerapps_bot_api_name=$BotApi containerapps_game_api_name=$GameApi managedEnvironments_env_name=$ManagedEnvironment location=$Location virtualNetworks_vnet_name=$VnetName vnet_subnet_name="default" environment-subnet-name=$EnvironmentSubnet
   
#Deploy the database ARM

az deployment group create --resource-group $DBResourceGroup --template-file azuredeployDB.json --parameters databaseAccounts_db_name=$DatabaseAccount location=$Location
   
#----------------------------------------------------------------------------------------------------------------------

## Step 2: Create an Azure Static Web App 

#----------------------------------------------------------------------------------------------------------------------

#2.1 Deploy your static web app in the same Resource Group with the APIs

$StaticWeb="<Static-Web-App-Name>" # <Static-Web-App-Name> = Your Static Web App name

az staticwebapp create --name $StaticWeb --resource-group $APIResourceGroup --source <github-repository-url> --branch <branch> --app-location "/module-1-azure-architecture-introduction/src/Exercise_1/RockPaperScissors" --api-location "/module-1-azure-architecture-introduction/src/Exercise_1/RockPaperScissorsAPI" --output-location "wwwroot" --login-with-github
   
   # <github-repository-url> = Your Github Repository url
   # <branch> = The branch you want to use for deployment
#2.2 Configure an environment variable to connect your Static Web App with your game Container Api

$GameContainerUrl="<game-api-container-url>"  # <game-api-container-url> = Url created on game api container

$GameContainerHN="<game-container-host-name>" # <game-container-host-name> = Hostname of game container url

$BotContainerUrl="<bot-container-url>" # <bot-container-url> = Your Bot container api url

$BotContainerHN="<bot-container-host-name>" # <bot-container-host-name> = Hostname of bot container url

az staticwebapp appsettings set --name $StaticWeb --setting-names "GAMEAPI_URL=$GameContainerUrl" "BOTAPI_URL=$BotContainerUrl"
   
#2.3 At the end of this step you will be able to see your Static Web app deployed in Azure Portal
   
#----------------------------------------------------------------------------------------------------------------------

## Step 3: Configure dapr statestore using Cosmos DB

#----------------------------------------------------------------------------------------------------------------------

#3.1 Install az containerapp extension

az extension add --name containerapp --upgrade

#3.2 Configuring statestore using statestore.yaml file from the local *infra* folder

cd "<your-statestore.yaml-path>" # <your-statestore.yaml-path> = The path for statestore.yaml file that you cloned from Github repository

#3.3 Open the file and edit the following variables: `<cosmos-url>` and `<cosmos-primary-key>`

#3.4 Update the Managed Environment

az containerapp env dapr-component set --name $ManagedEnvironment --resource-group $APIResourceGroup --dapr-component-name statestore --yaml statestore.yaml
 
#----------------------------------------------------------------------------------------------------------------------

## Step 4: Configure environment variables for Azure Container Apps

#----------------------------------------------------------------------------------------------------------------------

#4.1 Configure environment variable for Game Container Api

az containerapp up --name $GameApi --resource-group $APIResourceGroup --image ghcr.io/$GitRepositoryOwner/gameapi-rockpaperscissors:latest --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --env-vars GAME_API_BOTAPI="$BotContainerUrl"

#4.2 Configure environment variable for Bot Container Api

az containerapp up --name $BotApi --resource-group $APIResourceGroup --image ghcr.io/$GitRepositoryOwner/botapi-rockpaperscissors:latest --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --env-vars BOT_API_SESSION_URL=$GameContainerUrl

#----------------------------------------------------------------------------------------------------------------------

## Step 5: Deploy the second Container App on another region

#----------------------------------------------------------------------------------------------------------------------

#5.1 Create the resource group

$ResourceGroup2="<resource-group-name>" # <resource-group-name> = Second resource group name

$Location2="<location-name>" # <location-name> = Second location for resource group

#5.2 Run the create command

az group create --name $ResourceGroup2 --location $Location2

#5.3 Create the environment

$ManagedEnvironment2="<second-managed-environment-name>" # <second-managed-environment-name> = Your second managed environment name

az containerapp env create --name $ManagedEnvironment2 --resource-group $ResourceGroup2 --location $Location2

#5.4 Update the Managed Environment

az containerapp env dapr-component set --name $ManagedEnvironment2 --resource-group $ResourceGroup2 --dapr-component-name statestore --yaml statestore.yaml

#5.5 Create your second Container App and save its host name in a variable for later

$BotApi2="<second-botapi-container-name>" # <second-botapi-container-name> = Your second container name

az containerapp create --name $BotApi2 --resource-group $ResourceGroup2 --environment $ManagedEnvironment2 --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --image  ghcr.io/$GitRepositoryOwner/botapi-rockpaperscissors:latest --target-port 8080 --ingress external --query properties.configuration.ingress.fqdn --env-vars BOT_API_SESSION_URL=$GameContainerUrl --enable-dapr --dapr-app-id botapi --dapr-app-port 8080

$BotContainerHN2="<second-bot-container-host-name>" # <second-bot-container-host-name> = Second bot container hostname

#----------------------------------------------------------------------------------------------------------------------

## Step 6: Configure Front Door to connect both regions from bot Container Api

#----------------------------------------------------------------------------------------------------------------------

#6.1. Create a new resource-group for Front Door

$NetworkResourceGroup="<resource-group-name>" # resource-group-name> = Your network resource group name

az group create --name $NetworkResourceGroup --location $Location

#6.2 Create Azure Front Door profile

$ProfileName="<profile-name>" # <profile-name> = Name your azure front door

az afd profile create --profile-name $ProfileName --resource-group $NetworkResourceGroup --sku Standard_AzureFrontDoor

#6.3 Create Azure Front Door endpoint

$EndpointName="<endpoint-name>" # <endpoint-name> = Name your endpoint

az afd endpoint create --resource-group $NetworkResourceGroup  --endpoint-name $EndpointName --profile-name $ProfileName --enabled-state Enabled

#6.4 Create an origin group

$OriginGroupName="<origin-group-name>" # <origin-group-name> = Name your origin group

az afd origin-group create --resource-group $NetworkResourceGroup --origin-group-name $OriginGroupName --profile-name $ProfileName --probe-request-type GET --probe-protocol HTTPS --probe-interval-in-seconds 10 --probe-path "/" --sample-size 4 --successful-samples-required 3 --additional-latency-in-milliseconds 50 --enable-health-probe true

#6.5 Create origins
#Create first origin

az afd origin create --resource-group $NetworkResourceGroup --host-name $BotContainerHN --profile-name $ProfileName --origin-group-name $OriginGroupName --origin-name <first-origin-name> --origin-host-header $BotContainerHN --priority 1 --weight 1000 --enabled-state Enabled --http-port 8080 --https-port 443 --enable-private-link false

# <first-origin-name> = First origin name
#Create second origin

az afd origin create --resource-group $NetworkResourceGroup --host-name $BotContainerHN2 --profile-name $ProfileName --origin-group-name $OriginGroupName --origin-name <second-origin-name> --origin-host-header $BotContainerHN2 --priority 2 --weight 1000 --enabled-state Enabled --http-port 8080 --https-port 443 --enable-private-link false
 
# <second-origin-name> = Second origin name
#6.6 Create Front Door route

az afd route create --resource-group $NetworkResourceGroup --profile-name $ProfileName --endpoint-name $EndpointName  --forwarding-protocol MatchRequest --route-name route --https-redirect Enabled --origin-group $OriginGroupName --supported-protocols Http Https --link-to-default-domain Enabled

#6.7 List endpoint to get the Front Door link and save it on a variable

az afd endpoint show --resource-group $NetworkResourceGroup --profile-name $ProfileName --endpoint-name $EndpointName

$Endpoint="https://<endpoint-url>" # <endpoint-url> = Front Door endpoint url for game

#----------------------------------------------------------------------------------------------------------------------

## Step 7: Configure Front Door to connect both regions from game Container Api

#----------------------------------------------------------------------------------------------------------------------

#7.1 Create gameapi container on second region

$GameApi2="<second-gameapi-container-name>" # <second-gameapi-container-name> = Your second container name

az containerapp create --name $GameApi2 --resource-group $ResourceGroup2 --environment $ManagedEnvironment2 --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --image  ghcr.io/$GitRepositoryOwner/gameapi-rockpaperscissors:latest --target-port 8080 --ingress external --query properties.configuration.ingress.fqdn --env-vars GAME_API_BOTAPI="$BotContainerUrl" --enable-dapr --dapr-app-id gameapi --dapr-app-port 8080

$GameContainerHN2="<second-bot-container-host-name>" # <second-bot-container-host-name> = Second game container hostname

#7.2 Create another endpoint

$EndpointName2="<second-endpoint-name>" # <second-endpoint-name> = Name your endpoint

az afd endpoint create --resource-group $NetworkResourceGroup  --endpoint-name $EndpointName2 --profile-name $ProfileName --enabled-state Enabled

#7.3 Create a second origin group

$OriginGroupName2="<second-origin-group-name>" # <second-origin-group-name> = Name your second origin group for game

az afd origin-group create --resource-group $NetworkResourceGroup --origin-group-name $OriginGroupName2 --profile-name $ProfileName --probe-request-type GET --probe-protocol HTTPS --probe-interval-in-seconds 10 --probe-path "/" --sample-size 4 --successful-samples-required 3 --additional-latency-in-milliseconds 50 --enable-health-probe true

#7.4 Create origins
   
#Create first game origin

az afd origin create --resource-group $NetworkResourceGroup --host-name $GameContainerHN --profile-name $ProfileName --origin-group-name $OriginGroupName2 --origin-name <first-origin-name> --origin-host-header $GameContainerHN --priority 1 --weight 1000 --enabled-state Enabled --http-port 8080 --https-port 443 --enable-private-link false
# <first-origin-name> = First origin name for game
#Create second game origin

az afd origin create --resource-group $NetworkResourceGroup --host-name $GameContainerHN2 --profile-name $ProfileName --origin-group-name $OriginGroupName2 --origin-name <second-origin-name> --origin-host-header $GameContainerHN2 --priority 2 --weight 1000 --enabled-state Enabled --http-port 8080 --https-port 443 --enable-private-link false
# <second-origin-name> = Second origin name for game
#7.5 Create Front Door route for game

az afd route create --resource-group $NetworkResourceGroup --profile-name $ProfileName --endpoint-name $EndpointName2  --forwarding-protocol MatchRequest --route-name route --https-redirect Enabled --origin-group $OriginGroupName2 --supported-protocols Http Https --link-to-default-domain Enabled

#7.6 List second endpoint to get the Front Door link and save it on a variable

az afd endpoint show --resource-group $NetworkResourceGroup --profile-name $ProfileName --endpoint-name $EndpointName2

$Endpoint2="https://<endpoint-url>" # <endpoint-url> = Front Door endpoint url for game

#----------------------------------------------------------------------------------------------------------------------

## Step 8: Use the endpoints to configure Azure Container Apps and Static Web

#----------------------------------------------------------------------------------------------------------------------

#8.1 Modify environment variables for Azure Container Apps

az containerapp up --name $GameApi --resource-group $APIResourceGroup --image ghcr.io/$GitRepositoryOwner/gameapi-rockpaperscissors:latest --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --env-vars GAME_API_BOTAPI=$Endpoint

az containerapp up --name $BotApi --resource-group $APIResourceGroup --image ghcr.io/$GitRepositoryOwner/botapi-rockpaperscissors:latest --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --env-vars BOT_API_SESSION_URL=$Endpoint2

az containerapp up --name $GameApi2 --resource-group $ResourceGroup2 --image ghcr.io/$GitRepositoryOwner/gameapi-rockpaperscissors:latest --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --env-vars GAME_API_BOTAPI=$Endpoint

az containerapp up --name $BotApi2 --resource-group $ResourceGroup2 --image ghcr.io/$GitRepositoryOwner/botapi-rockpaperscissors:latest --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --env-vars BOT_API_SESSION_URL="$Endpoint2"

#8.2 Modify environment variables for Static Web App

az staticwebapp appsettings set --name $StaticWeb --setting-names "GAMEAPI_URL=$Endpoint2" "BOTAPI_URL=$Endpoint"

#8.3 Add `*` to CORS manually under Settings tab for Azure Container Apps created on second region from Azure Portal