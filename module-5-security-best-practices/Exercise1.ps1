# ----------------------------------------------------------------------------------------------------------------------

# Module 5: Security Best Practices

# ----------------------------------------------------------------------------------------------------------------------

# Exercise 1

# ----------------------------------------------------------------------------------------------------------------------

# Step 1: Create a Private Endpoint for CosmosDB

# 1. Update the subnet to disable private endpoint network policies. You first need to define your Default Subnet name, which will be "default" as defined in the ARM from Module 1, so you shouldn't change it.

$DefaultSubnetName="default"

az network vnet subnet update -n $DefaultSubnetName -g $APIResourceGroup --vnet-name $VnetName  --disable-private-endpoint-network-policies true


# 2. You need to save the Resource ID for the CosmosDB in order to create a private endpoint.

$DbResourceId = az cosmosdb show -n $DatabaseAccount -g $DBResourceGroup --query id --output tsv

# 3. Create the Private Endpoint for the CosmosDB.

$PrivateEndpointName="<private-endpoint-name>"

$ConnectionName="<private-link-service-connection-name>"

az network private-endpoint create -n $PrivateEndpointName -g $APIResourceGroup --vnet-name $VnetName --subnet $DefaultSubnetName --private-connection-resource-id $DbResourceId --group-ids Sql --connection-name $ConnectionName

# 4. To use the newly created Private Endpoint, you have to create a Private DNS Zone resource.
 
$ZoneName="privatelink.documents.azure.com"

az network private-dns zone create -g $APIResourceGroup -n $ZoneName

# 5. The DNS Zone needs to be linked the to Virtual Network. 

$ZoneLinkName="<zone-link-name>"

az network private-dns link vnet create -g $APIResourceGroup -n $ZoneLinkName --zone-name $ZoneName --virtual-network $VnetName --registration-enabled false

# 6. Now you can create a DNS Zone Group associated with the Private Endpoint.

$ZoneGroupName="<zone-group-name>"

az network private-endpoint dns-zone-group create -g $APIResourceGroup --endpoint-name $PrivateEndpointName -n $ZoneGroupName --private-dns-zone $ZoneName --zone-name "zone"

# ----------------------------------------------------------------------------------------------------------------------

# Step 2: Test the application

# ----------------------------------------------------------------------------------------------------------------------