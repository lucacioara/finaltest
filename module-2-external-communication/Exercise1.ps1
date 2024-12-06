# Module 2: External Communication

# Exercise 1

## Step 1: Create the Azure SignalR Service Resource
 #To use SignalR in your applications, you need to first deploy an Azure SignalR Service so that the hub will be hosted on the cloud.
 
 #1. Give a name to your SignalR Resource.
 
 $Signalr="<signalr-name>" # <signalr-name> = Name your signalR resource
 
 # 2 Create the SignalR resource.

az signalr create --name $Signalr --resource-group $EmailResourceGroup --sku "Free_F1" --unit-count "1" --service-mode "Default"
 
## Step 2: Copy the Connection String of the SignalR Service
# In order to get the connection string of your SignalR Service, you need to access the newly created resource in the Azure Portal.

 # 1. Navigate to your SignalR Service Resource. You should find it in the resource group where you created it.

 # 2 In the side menu, under Settings, you should find the Connection Strings tab.

 # 3 Copy the Primary Connection String from the For access key section.

 # 4 Set the Connection String inside your console.
   
$SignalREndpoint="<singalr-connection-string>" # <singalr-connection-string> = Your signalR connection string

## Step 3: Create an Azure Communication Service resource
 # 1. In order to send e-mails to the users of the application, you will need an Azure Communication Service with SMTP capabilities. 

 $ACSName="<communication-service-name>" # <communication-service-name> = Name your azure communication service

#If you want to choose a different region for your Azure Communication Service "--data-location", you can review the available regions on https://learn.microsoft.com/en-us/azure/communication-services/concepts/privacy#data-residency
 
az communication create --name $ACSName --location "Global" --data-location "europe" --resource-group $EmailResourceGroup
 
> #[NOTE]  
> #On the first use of this az command, the terminal might request to install the communication extension, if so, please agree with the install and the command will continue to run after the extension is installed.

 
$EmailService="<email-service-name>" # <email-service-name> = Name your email service

# 2 Azure Communication Service has multiple ways of client communication. In order to use the email functionality, you will need an Azure Email Communication Service. 
 
az communication email create --name $EmailService --location "Global" --data-location "europe" --resource-group $EmailResourceGroup
 
# 3 The Email Communication Service also needs a Email Communication Services Domain in order to use it for sending emails.
 
az communication email domain create --domain-name AzureManagedDomain --email-service-name $EmailService --location "Global" --resource-group $EmailResourceGroup --domain-management AzureManaged

## Step 4: Set the Connection String and the Sender Address inside your terminal and redeploy the apps
#To see the changes of the application, you will have to redeploy the API's container and the Web Application.

# 1. You can find the Connection String in the Azure Portal, if you navigate to your Azure Communication Service resource, on the side menu, under Settings you will find the Keys tab

$SMTP="<SMTP-connection-string>" # <SMTP-connection-string> = your connection string from SMTP
 
# 2 Save the DoNotReply email address from which the emails will be sent. You should find it in the Email Communication Services Domain resource, under the MailFrom addresses tab.

$Sender="<Sender>" # <Sender> = your noreply email

# 3 To redeploy the API's with the new Environment Variables, run the following commands:

az containerapp up --name $GameApi --resource-group $APIResourceGroup --image ghcr.io/$GitRepositoryOwner/gameapi-rockpaperscissors:module2-ex1 --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --env-vars GAME_API_SIGNALR=$SignalREndpoint GAME_API_BOTAPI=$BotContainerUrl GAME_API_HOST=$GameContainerUrl GAME_API_SMTPSERVER=$SMTP GAME_API_SMTP_SENDER=$Sender
 
az containerapp up --name $BotApi --resource-group $APIResourceGroup --image ghcr.io/$GitRepositoryOwner/botapi-rockpaperscissors:module2-ex1 --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --env-vars BOT_API_SESSION_URL=$GameContainerUrl
 
# 4  To be able to deploy the version of the app with the SignalR functionality, you need to change the deployment workflow under ./github/workflows, more exactly, you have to change

# 5. Update the Environment Variables of the Static Web App in order to use SignalR

az staticwebapp appsettings set --name $StaticWeb --setting-names "GAMEAPI_URL=$GameContainerUrl" "BOTAPI_URL=$BotContainerUrl"

## Step 5: Add your domain to Communication Service

## Step 6: Test the apps
