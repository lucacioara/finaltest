# Module 6: Microservices Arhitecture
# Exercise 1

# ---------------------------------------------------------------------------------------------------------------------

# Step 1: Create an Azure Key Vault 

$KeyVault="<enter KeyVault name>"

az keyvault create --name $KeyVault --resource-group $APIResourceGroup

# ---------------------------------------------------------------------------------------------------------------------

# Step 2: Assign Roles for Key Vault Access

# 2.1 Define your subscription information:

$SubscriptionUPN="<subscription-upn>"

$SubscriptionId="<subscription-id>"

# 2.2 Create a role assignment for Key Vault Secrets Officer:

 az role assignment create --assignee $SubscriptionUPN --role "Key Vault Secrets Officer" --scope "/subscriptions/$SubscriptionId/resourceGroups/$APIResourceGroup/providers/Microsoft.KeyVault/vaults/$KeyVault"

# ----------------------------------------------------------------------------------------------------------------------

# Step 3: Add Your Endpoints as Secrets to the Key Vault

# 3.1 Add the SignalR endpoint as a secret:

az keyvault secret set --name SignalR --vault-name $KeyVault --value $SignalREndpoint

# 3.2 Add the ACS (SMTP) endpoint as a secret:

az keyvault secret set --name ACS --vault-name $KeyVault --value $SMTP

# ----------------------------------------------------------------------------------------------------------------------

# Step 4: Create a Managed Identity and Assign Roles

# 4.1 Create a Managed Identity

$Identity="<managed-identity-name>"
az identity create -g $APIResourceGroup -n $Identity

# ----------------------------------------------------------------------------------------------------------------------

# Step 5: Use Key Vault Secrets in the Game API Container

# 5.2 Apply the secrets as environment variables for your game API container:

```powershell
 az containerapp up --name $GameApi --resource-group $APIResourceGroup --image ghcr.io/$GitRepositoryOwner/gameapi-rockpaperscissors:module2-signalr --registry-server ghcr.io --registry-username $GitRepositoryOwner --registry-password $GitPAT --env-vars GAME_API_SIGNALR="secretref:signalrconnectionstring" GAME_API_BOTAPI=$BotContainerUrl GAME_API_HOST=$GameContainerUrl GAME_API_SMTPSERVER="secretref:acsconnectionstring" GAME_API_SMTP_SENDER=$Sender
```

# ----------------------------------------------------------------------------------------------------------------------