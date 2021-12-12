# az login
# Connect-AzAccount
# Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
# Install-Module -Name Az -AllowClobber

$projectName = "CloudXAssociateMSAzureDeveloperEast" #Read-Host -Prompt "Enter a project name that is used for generating resource names"
$location = "eastus" #Read-Host -Prompt "Enter an Azure location (i.e. eastus)"
$adminUser = "AdminLioshyn" #Read-Host -Prompt "Enter the SQL server administrator username"
$adminPassword = convertto-securestring "AdminAdmin_11" -asplaintext -force  #Read-Host -Prompt "Enter the SQl server administrator password" -AsSecureString
$resourceGroupName = "${projectName}rg"

New-AzResourceGroup -Name $resourceGroupName -Location $location -Force

New-AzResourceGroupDeployment `
    -ResourceGroupName $resourceGroupName `
    -AdministratorLogin $adminUser `
    -AdministratorLoginPassword $adminPassword `
    -TemplateFile .\CreateDatabase.json 
#-TemplateFile .\CreateServicePlan.json

New-AzResourceGroupDeployment `
    -ResourceGroupName $resourceGroupName `
    -TemplateFile .\CreateServicePlan.json 

New-AzResourceGroupDeployment `
    -ResourceGroupName $resourceGroupName `
    -TemplateFile .\CreateStorageAccount.json 

# Read-Host -Prompt "Press [ENTER] to continue ..."