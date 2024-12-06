# Module 8: Observability and Monitoring
# Exercise 1

# ---------------------------------------------------------------------------------------------------------------------

# Step 1: Deploy an Azure Application Insights resource
# 1.1 Deploy a Log Analytics Workspace Resource and save its ID.

$WorkspaceName = "<analytics-workspace-name>"

az monitor log-analytics workspace create --resource-group $APIResourceGroup --workspace-name $WorkspaceName --location $Location

$WorkspaceId=az monitor log-analytics workspace show --resource-group $APIResourceGroup --workspace-name $WorkspaceName --query id -o tsv

#1.2 Name your Application Insights Resource.

$InsightsName = "<application-insights-name>"

# 1.3 Create the Application Insights resource

az monitor app-insights component create --app $InsightsName --location $Location --resource-group $APIResourceGroup --workspace $WorkspaceID

# If prompted inside your terminal to install the extension *application-insights*, press **y**!

# ---------------------------------------------------------------------------------------------------------------------

## Step 2: Redeploy the Static Web App with the new version

# 2.1 Redeploy your Static Web App, and add the **INSIGHTS_CONNECTION_STRING** Environment Variable. Set its value to be the Connection String you just copied.

$AppInsightsConnectionString = "<application-insights-connection-string>"

az staticwebapp appsettings set --name $StaticWeb --setting-names "GAMEAPI_URL=$GameContainerUrl" "APIM_URL=$APIMUrl" "INSIGHTS_CONNECTION_STRING=$AppInsightsConnectionString"

# ---------------------------------------------------------------------------------------------------------------------