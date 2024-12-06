# Exercise 1

## Prerequisites
- Before starting, set the following variables to deploy your resource group:
   ```bash
   $ResourceGroup="<resource-group-name>"
   $Location="<resource-group-location>"
  ```
  - `<resource-group-name> = Your resource group name`
  - `<resource-group-location> = Your resource group location`
---
Clone Github repository using the template provided on [ext_AdvancedAzureArchitecture](https://github.com/KeyTicket-Solutions/ext_AdvancedAzureArchitecture)

## Step 1: Setting Up the Environment

1. **Install Required Tools**
  - Download and install **az cli** from [here](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli#install-or-update).
  - Verify if `az` command is installed:

    ```bash
    az --help
    ```
2. **Login to your azure account and select your subscription**
  - Login to your azure subscription
    ```bash
     az login
     ```
3. Select your subscription by typing the number assigned to it

## Step 2: Create the resource group and deploy the arm
1. **Create your resource-group**

   ```bash
   az group create --name $ResourceGroup --location $Location
   ```
2. **Set the variables used on deployment**

   ```bash
   $BotApi="<botapi-container-name>"
   $GameApi="<gameapi-container-name>"
   $StatsApi="<statsapi-container-name>"
   $PlayerApi="<playerapi-container-name>"
   $ManagedEnvironment="<managed-environment>"
   $DatabaseAccount="<db-account-name>"
   ```
   - `<botapi-container-name> = Your Bot container name`
   - `<gameapi-container-name> = Your Game container name`
   - `<statsapi-container-name> = Your Stats container name`
   - `<playerapi-container-name> = Your Player container name`
   - `<managed-environment> = Your Managed Environment resource name`
   - `<frontdoor-name> = Your Front Door resource name`
   - `<db-account-name> = Your Cosmos Database Account name`
3. **Deploy the arm using configured variables**
- Set the path to your arm deploy
   ```bash
   cd <path-to-arm-deploy>
   ```
   - `<path-to-arm-deploy> = Path to your arm deploy in infra file`
- Deploy the arm
   ```bash
   az deployment group create --resource-group $ResourceGroup --template-file azuredeploy.json --parameters containerapps_bot_api_name=$BotApi containerapps_game_api_name=$GameApi containerapps_stats_api_name=$StatsApi containerapps_player_api_name=$PlayerApi managedEnvironments_env_name=$ManagedEnvironment databaseAccounts_db_name=$DatabaseAccount location=$Location
   ```
## Step 3: Create an azure static web app 

1. Deploy your static web app
   ```bash
   $StaticWeb="<Static-Web-App-Name>"
   az staticwebapp create --name $StaticWeb --resource-group $ResourceGroup --source <github-repository-url> --branch <branch> --app-location "/src/Module 1/Exercise_1-2_Start/RockPaperScissors" --api-location "/src/Module 1/Exercise_1-2_Start/RockPaperScissorsAPI" --output-location "wwwroot" --login-with-github
   ```
   - `<Static-Web-App-Name> = Your Static Web App name`
   - `<github-repository-url> = Your Github Repository url`
   - `<branch> = The branch you want to use for deployment`
2. Configure an environment variable to connect your static web app with your game container api
   ```bash
   $GameContainerUrl="<game-api-container-url>"
   az staticwebapp appsettings set --name $StaticWeb --setting-names "GAMEAPI_URL=$GameContainerUrl"
   ```
   - `<game-api-container-url> = Url created on game api container`
## Step 4: Configure dapr statestore using Cosmos DB
1. Install az containerapp extension
   ```bash
   az extension add --name containerapp --upgrade
   ```
2. Configuring statestore using **statestore.yaml** file from *infra* folder
   ```bash
   cd <your-statestore.yaml-path>
   ```
   - `<your-statestore.yaml-path> = The path for statestore.yaml file that you cloned from github repository`
3. Open the file and edit the following variables: `<cosmos-url>` and `<cosmos-primary-key>`
    - `<cosmos-url> = Your Cosmos DB url`
    - `<cosmos-primary-key> = Your Cosmos DB primary key`
4. Update the Managed Environment
    ```bash
    az containerapp env dapr-component set --name $ManagedEnvironment --resource-group $ResourceGroup --dapr-component-name statestore --yaml statestore.yaml
    ```
## Step 5: Configure environment variables for Azure container apps
1. Configure environment variable for Game container api
   ```bash
   $BotContainerUrl="<bot-container-url>"
   $BotContainerHN="<bot-container-host-name>"
   az containerapp up --name $GameApi --resource-group $ResourceGroup --image casianbara/gameapi-rockpaperscissors:latest --env-vars GAME_API_BOTAPI="$BotContainerUrl"
   ```
   - `<bot-container-url> = Your Bot container api url`
   - `<bot-container-host-name> = Hostname of bot container url`
2. Configure environment variable for Bot container api
   ```bash
   az containerapp up --name $BotApi --resource-group $ResourceGroup --image casianbara/botapi-rockpaperscissors:latest --env-vars BOT_API_SESSION_URL=$GameContainerUrl
   ```
## Step 6: Deploy the second container app on another region
1. Create the resource group
    ```bash
    $ResourceGroup2="<resource-group-name>"
    $Location2="<location-name>"
    ```
    - `<resource-group-name> = Second resource group name`
    - `<location-name> = Second location for resource group`
2. Run the create command
   ```bash
   az group create --name $ResourceGroup2 --location $Location2
   ```
3. Create the environment
   ```bash
   $ManagedEnvironment2="<environment-name>"
   az containerapp env create --name $ManagedEnvironment2 --resource-group $ResourceGroup2 --location $Location2
   ```
     - `<environment-name> = Your second managed environment name`
4. Create your second container app
   ```bash
   $BotApi2="<container-name>"
   az containerapp create --name $BotApi2 --resource-group $ResourceGroup2 --environment $ManagedEnvironment2 --image  casianbara/botapi-rockpaperscissors:latest --target-port 8080 --ingress external --query properties.configuration.ingress.fqdn --env-vars BOT_API_SESSIONAPI_URL=$GameContainerUrl --enable-dapr --dapr-app-id api --dapr-app-port 8080
   $BotContainerHN2="<second-bot-container-host-name>"
    ```
    - `<container-name> = Your second container name`
    - `<second-bot-container-host-name> = Second bot container hostname`

## Step 7: Configure front door to connect both regions from bot container api
1. Create azure front door profile
   ```bash
   $ProfileName="<profile-name>"
   az afd profile create --profile-name $ProfileName --resource-group $ResourceGroup --sku Premium_AzureFrontDoor
   ```
   - `<profile-name> = Name your profile`
2. Create azure front door endpoint
   ```bash
   $EndpointName="<endpoint-name>"
   az afd endpoint create --resource-group $ResourceGroup  --endpoint-name $EndpointName --profile-name $ProfileName --enabled-state Enabled
   ```
   - `<endpoint-name> = Name your endpoint`
3. Create an origin group
   ```bash
   $OriginGroupName="<origin-group-name>"
   az afd origin-group create --resource-group $ResourceGroup --origin-group-name $OriginGroupName --profile-name $ProfileName --probe-request-type GET --probe-protocol HTTPS --probe-interval-in-seconds 10 --probe-path "/" --sample-size 4 --successful-samples-required 3 --additional-latency-in-milliseconds 50 --enable-health-probe true
   ```
   - `<origin-group-name> = Name your origin group`
4. Create origins
- Create first origin
   ```bash
   az afd origin create --resource-group $ResourceGroup --host-name $BotContainerHN --profile-name $ProfileName --origin-group-name $OriginGroupName --origin-name <first-origin-name> --origin-host-header $BotContainerHN --priority 1 --weight 1000 --enabled-state Enabled --http-port 8080 --https-port 443 --enable-private-link false
   ```
   - `<first-origin-name> = First origin name`
- Create second origin
   ```bash
   az afd origin create --resource-group $ResourceGroup --host-name $BotContainerHN2 --profile-name $profileName --origin-group-name $OriginGroupName --origin-name <second-origin-name> --origin-host-header $BotContainerHN2 --priority 2 --weight 1000 --enabled-state Enabled --http-port 8080 --https-port 443 --enable-private-link false
   ```
   - `<second-origin-name> = Second origin name`
5. Create front door route
   ```bash
   az afd route create --resource-group $ResourceGroup --profile-name $ProfileName --endpoint-name $EndpointName  --forwarding-protocol MatchRequest --route-name route --https-redirect Enabled --origin-group $OriginGroupName --supported-protocols Http Https --link-to-default-domain Enabled
   ```
6. List endpoint to get the front door link
   ```bash
   az afd endpoint show --resource-group $ResourceGroup --profile-name $ProfileName --endpoint-name $EndpointName
   ```
