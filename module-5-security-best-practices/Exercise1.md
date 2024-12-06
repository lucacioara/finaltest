# Module 5: Security Best Practices
This module covers advanced security architectures, including Zero Trust, data encryption, and threat protection. 

# Exercise 1
In this exercise you will deploy a Private Endpoint for the CosmosDB resource so it can only be accessed privately.

## Estimated time: TODO minutes

## Learning objectives
   - Secure your CosmosDB by implementing Private Endpoints

## Prerequisites


To begin this module you will need the Azure resources that you deployed in the previous modules.

During this module you will also need 5 of the PowerShell variables used previously:
 - $Location - location of the first region deployed in Module 1
 - $APIResourceGroup  - name of your API Resource Group deployed in Module 1
 - $VnetName - name of your VNET deployed in Module 1
 - $DatabaseAccount - name of your CosmosDB resource deployed in Module 1
 - $DBResourceGroup - name of your Database Resource Group deployed in Module 1

## Step 1: Create a Private Endpoint for CosmosDB
1. First, create a resource group for this module and exercise.

```powershell
$DefaultSubnetName="default"
```
```powershell
$PrivateLinkResourceGroup="<private-link-resource-group-name>"
```
```powershell
az group create -n $PrivateLinkResourceGroup --location $Location
```

2. Update the subnet to disable private endpoint network policies.
```powershell
az network vnet subnet update -n $DefaultSubnetName -g $APIResourceGroup --vnet-name $VnetName  --disable-private-endpoint-network-policies true
```

3. You need to save the Resource ID for the CosmosDB in order to create a private endpoint.
```powershell
$DbResourceId = az cosmosdb show -n $DatabaseAccount -g $DBResourceGroup --query id --output tsv
```
4. Create the Private Endpoint for the CosmosDB.
```powershell
$PrivateEndpointName="<private-endpoint-name>"
```

```powershell
$ConnectionName="<private-link-service-connection-name>"
```

```powershell
az network private-endpoint create -n $PrivateEndpointName -g $PrivateLinkResourceGroup --vnet-name $VnetName --subnet $DefaultSubnetName --private-connection-resource-id $DbResourceId --group-ids Sql --connection-name $ConnectionName
```
5. To use the newly created Private Endpoint, you have to create a Private DNS Zone resource.
```powershell
$ZoneName="privatelink.documents.azure.com"
```

```powershell
az network private-dns zone create -g $PrivateLinkResourceGroup -n $ZoneName
```
6. The DNS Zone needs to be linked the to Virtual Network. 
```powershell
$ZoneLinkName="<zone-link-name>"
```

```powershell
az network private-dns link vnet create -g $PrivateLinkResourceGroup -n $ZoneLinkName --zone-name $ZoneName --virtual-network $VnetName --registration-enabled false
```
7. Now you can create a DNS Zone Group associated with the Private Endpoint.
```powershell
$ZoneGroupName="<zone-group-name>"
```

```powershell
az network private-endpoint dns-zone-group create -g $PrivateLinkResourceGroup --endpoint-name $PrivateEndpointName -n $ZoneGroupName --private-dns-zone $ZoneName --zone-name "zone"
```
## Step 2: Test the application

You can test the web app and see that it's working the same way that it did before. The difference now is that the CosmosDB resource is much more secure, knowing that only resources inside the virtual network can access it. Not only that your leaderboard data will be secured, but also the statestore data used by dapr.

All traffic between the database and other resources don't leave the virtual network when transferred to the Container Apps. Without the private endpoint, the data passes through the internet, posing a higher security risk.