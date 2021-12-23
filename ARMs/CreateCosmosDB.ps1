# Set variables for the new SQL API account, database, and container
resourceGroupName='myResourceGroup'
location='southcentralus'

# The Azure Cosmos account name must be globally unique, make sure to update the `mysqlapicosmosdb` value before you run the command
accountName='mysqlapicosmosdb'

# Create a resource group
New-AzResourceGroup -Name $resourceGroupName -Location $location -Force

New-AzCosmosDBAccount -ResourceGroupName $resourceGroupName -Name $accountName -Location "South Central US"