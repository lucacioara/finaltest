# Exercise 2

## Step 1: Redeploy the apps

#You will need to deploy the new version of the APIs. This version contains a schema with the OpenAPI specifications of the API's so that when you'll import them into the APIM, the endpoints will be automatically generated for you without the need of importing or manually adding them.

#The Web Application will also change as it will now call the common endpoint of the APIM resource, instead of using the respective endpoints of each API.


az containerapp up --name $GameApi --resource-group $APIResourceGroup --image ghcr.io/$GitRepositoryOwner/gameapi-rockpaperscissors:module2-ex2 --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --env-vars GAME_API_SIGNALR=$SignalREndpoint GAME_API_BOTAPI=$BotContainerUrl GAME_API_HOST=$GameContainerUrl GAME_API_SMTPSERVER=$SMTP GAME_API_SMTP_SENDER=$Sender


az containerapp up --name $BotApi --resource-group $APIResourceGroup --image ghcr.io/$GitRepositoryOwner/botapi-rockpaperscissors:module2-ex2 --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --env-vars BOT_API_SESSION_URL=$GameContainerUrl


## Step 2: Provision the APIs to the APIM resource
#For this step you will need to open Azure Portal, on the APIM resource that you created on Step 1. 
# 1. Open the **APIs** tab, where you can add an API to your APIM. Select Container App as your option.

# 2. Browse and select the bot container API you created in Module 1. The fields should auto-populate, except for **API URL suffix**, where you should enter the value `bot`

# 3. After adding your bot API container app, select it and go to the **Settings** tab where you need to disable **Subscription Required**


# 4. Repeat the steps to add your game API container as well. For the **"API URL suffix"** field, enter `game` for the game container API.
## Step 3: Add CORS to the APIM resource
#To ensure that only your Web Application will be able to access the APIs, you should add CORS to the APIM resource.

# 1. Add CORS to APIM by selecting **All APIs** and adding a policy on the **Inbound Processing section**


# 2. Select **CORS** policy, go to the **Full** tab, and add your static web url to **"Allowed Origins"** field. Make sure to enable **GET** and **POST** methods, set the allowed headers and exposed headers to *, then press the save button.


## Step 4: Redeploy static web app with APIM url

$APIMUrl="<your-apim-url>" # <your-apim-url> = your Gateway url from APIM


az staticwebapp appsettings set --name $StaticWeb --setting-names "GAMEAPI_URL=$GameContainerUrl" "APIM_URL=$APIMUrl"


## Step 5: Implement SSL Termination 
#Now you can secure the traffic of your API by enabling SSL Termination for the APIM.
#1. Navigate to your APIM resource

#2. Go to **Protocols + ciphers** under **Security** tab and **enable SSL** for both **Client protocol** and **Backend protocol**. Make sure to click **Enable** before saving. After that, press the **Save** button.


#Now the traffic will be secure, making the data unreadable if it's intercepted.

## Step 6: Implement rate throttling

#In order to implement rate throttling for the APIs inside your APIM resource, you need to add a policy.

#- On the **APIs** tab, select either the bot or game API, or select both by choosing **All APIs**. Here, you will click **Add policy** under **Inbound processing**. Then, add a **rate-limit-by-key** policy, completing the fields with values for rate throttling as shown in this example:

#After implementing rate throttling, the endpoint(s) of the API(s) you selected will have a limit of how many requests can be made by someone in the period of time that you set.
