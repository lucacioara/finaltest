# Module 8: Observability and Monitoring
This module focuses on observability and monitoring in Azure. Learn about the importance of monitoring, how to set up autoscaling, and diagnosing application performance with Azure Monitor and Application Insights.
# Exercise 1
In this exercise you will deploy an **Azure Application Insights** resource that will monitor your Web Application. offering logs about users, sessions, events and more.

## Estimated time: TODO minutes

## Learning objectives
   - Use Azure Application Insights for Web App monitoring
   
## Prerequisites
To begin this module you will need the Azure resources that you deployed in the previous modules.

During this module you will also need 5 of the PowerShell variables used previously:
 - $Location - location of the first region deployed in Module 1
 - $APIResourceGroup  - name of your API Resource Group deployed in Module 1
 - $StaticWeb - name of your Azure Static Web App resource
 - $GameContainerUrl - url of your Game API container
 - $APIMUrl - endpoint for your Azure API Management resource 


## Step 1: Deploy an Azure Application Insights resource
First, we need a name for our Application Insights Resource.
```powershell
$InsightsName=<application-insights-name>
```
Then, you can create the resource by running the command below.
```powershell
az monitor app-insights component create --app $InsightsName --location $Location --resource-group $APIResourceGroup --application-type "web"
```
We will use "web" for the application-type parameter as we will use this resource to monitor a web application.
> [!IMPORTANT]  
> If prompted inside your terminal to install the extension *application-insights*, press **y**.
## Step 2: Redeploy the Static Web App with the new version
Now you will need to redeploy the Web App with the new code in order to communicate with the Application Insights resource.

First, you need to copy the Connection String of your resource. You can find it in the [Azure portal](https://portal.azure.com), by navigating to your newly created Application Insights resource, next to **Connection String**.

Then, you can redeploy your Static Web App, and add the **INSIGHTS_CONNECTION_STRING** Environment Variable. Set its value to be the Connection String you just copied.
```powershell
$AppInsightsConnectionString="<application-insights-connection-string>"
```
```powershell
az staticwebapp appsettings set --name $StaticWeb --setting-names "GAMEAPI_URL=$GameContainerUrl" "APIM_URL=$APIMUrl" INSIGHTS_CONNECTION_STRING=$AppInsightsConnectionString
```

## Step 3: Test the application and check Application Insights for events

Now you can play around with your web application and it should automatically record events inside Application Insights.

Open the [Azure portal](https://portal.azure.com) and navigate to your Application Insights Resource. It is recommended to link your resource to a Log Analytics Workspace. You can do that on the Overview page of the resource, and choose an existent Workplace, or create a new one.

You can now check the usage of the application, under the Usage tab. You can see the number of users of your application, or events regarding how they use the app, what pages they access etc.

