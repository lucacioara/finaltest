# Module 8: Observability and Monitoring
# Exercise 1

# ---------------------------------------------------------------------------------------------------------------------

# Step 1: Deploy an Azure Application Insights resource
# 1.1 Name your Application Insights Resource

$InsightsName = "<name your Applciation Insights resource>"

# 1.2 Create the Application Insights resource

az monitor app-insights component create --app $InsightsName --location $Location --resource-group $APIResourceGroup --application-type "web"

# If prompted inside your terminal to install the extension *application-insights*, press **y**!

# ---------------------------------------------------------------------------------------------------------------------

## Step 2: Redeploy the Static Web App with the new version

# 2.1 Redeploy your Static Web App, and add the **INSIGHTS_CONNECTION_STRING** Environment Variable. Set its value to be the Connection String you just copied.

$AppInsightsConnectionString = "<application-insights-connection-string>"
az staticwebapp appsettings set --name $StaticWeb --setting-names "GAMEAPI_URL=$GameContainerUrl" "APIM_URL=$APIMUrl" "INSIGHTS_CONNECTION_STRING=$AppInsightsConnectionString"

# ---------------------------------------------------------------------------------------------------------------------