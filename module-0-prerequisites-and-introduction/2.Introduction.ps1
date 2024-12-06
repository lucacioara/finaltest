## Deploying Resources

# To prepare for the next exercises, you will need to deploy a resource group and an Azure API Management (APIM) resource. This process can take some time, so it's best to start it early.

# Step 1: Create a Resource Group
# Use the following command to create a new resource group. Replace `<email-resource-group>` with your desired name and `<resource-group-location>` with the appropriate Azure region (e.g., `westeurope`):

$EmailResourceGroup="<email-resource-group>"

$Location="<resource-group-location>"

# - Use the command bellow to list all Azure locations

az account list-locations -o table

az group create --name $EmailResourceGroup --location $Location


# Step 2: Deploy an Azure API Management Resource
# Use the following commands to deploy an APIM resource within your resource group. Replace the placeholders with your desired values:

$PublisherName="<publisher-name>"

$APIM="<apim-name>" 

$PublisherEmail="<publisher-email>" 

- `<publisher-name> = Name your publisher`
- `<apim-name> = Name your api management`
- `<publisher-email> = Enter your publisher email`

# The publisher email is used for receiving notifications about API subscriptions. It won't be used for this exercise but you need to set it in order to create the resource.


az apim create --name $APIM --resource-group $EmailResourceGroup --publisher-name $PublisherName --publisher-email $PublisherEmail

---