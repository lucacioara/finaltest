# Module 6: Microservices Arhitecture
# Exercise 2

# ----------------------------------------------------------------------------------------------------------------------

# Step 1: Create a Storage Account and Update the Container App Environment

# 1.1 Define your storage account name

$StorageAccount="<enter Storage Account name>"

# 1.2 Create the storage account

az storage account create --name $StorageAccount --resource-group $APIResourceGroup --location $Location --sku Standard_RAGRS --kind StorageV2 --min-tls-version TLS1_2 --allow-blob-public-access true

# 1.3. Update the Container App environment to send logs to Azure Monitor

az containerapp env update --name $ManagedEnvironment --resource-group $APIResourceGroup --logs-destination azure-monitor

# ----------------------------------------------------------------------------------------------------------------------
